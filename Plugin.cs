using System;
using BepInEx;
using BepInEx.IL2CPP;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using qol_core;
using SocketIOClient;
using SteamworksNative;
using UnityEngine;

namespace party_crab
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("qol-core")]
    public class Plugin : BasePlugin
    {
        public static Mod modInstance;
        public static SocketIO client;

        public static bool client_ready;

        public static string party_server = "http://localhost:8080";
        public static Party current_party;
        public static bool party_chat;
        public static string warp_lobby;
        public static List<Tuple<string, int>> chat_buffer;

        public override void Load()
        {
            Harmony.CreateAndPatchAll(typeof(Plugin));

            modInstance = Mods.RegisterMod(PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION, "*party crab noise*");

            Commands.RegisterCommand("party", "party *", "The party command.", modInstance, PartyCommand);
            Commands.RegisterCommand("p", "p *", "The party command.", modInstance, PartyCommand);

            Commands.RegisterCommand("ac", "ac", "Go into all chat.", modInstance, AllChatCommand);
            Commands.RegisterCommand("pc", "pc", "Go into party chat.", modInstance, PartyChatCommand);

            Log.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }

        public static bool AllChatCommand(List<string> arguments)
        {
            do {
                if (arguments.Count >= 2)
                {
                    string message = String.Join(" ", arguments.Skip(1).ToArray());

                    if (message == "")
                        break;

                    party_chat = false;
                    ChatBox.Instance.AppendMessage(0ul, message, SteamManager.Instance.field_Private_String_0);
                    ClientSend.SendChatMessage(message);
                    party_chat = true;

                    return true;
                }
            } while ( false );

            party_chat = false;
            SendMessage("you are now in all chat", 1);
            return true;
        }
        public static bool PartyChatCommand(List<string> arguments)
        {
            if (current_party == null)
            {
                SendMessage("you aren't in a party", 2);
                return true;
            }

            do {
                if (arguments.Count >= 2)
                {
                    string message = String.Join(" ", arguments.Skip(1).ToArray());

                    if (message == "")
                        break;

                    var data = new MessageDTO
                    {
                        username = SteamManager.Instance.field_Private_String_0,
                        message = message
                    };

                    party_chat = true;
                    SendMessage($"{data.username}: {Markup.MarkupChange(message)}", 0);
                    client.EmitAsync("message", data);
                    party_chat = false;

                    return true;
                }
            } while ( false );

            party_chat = true;
            SendMessage("you are now in party chat", 1);
            return true;
        }

        public static bool PartyCommand(List<string> arguments)
        {
            if (arguments[1] == "server")
            {
                if (arguments.Count >= 3)
                {
                    party_server = arguments[2];
                }
                if (client_ready)
                {
                    client_ready = false;
                    client.Dispose();
                }
                SendMessage($"party server -> {party_server}", 1);
                Client.Connect();
            }

            if (arguments[1] == "host")
            {
                var data = new HostDTO
                {
                    party_name = $"{SteamManager.Instance.field_Private_String_0}'s server",
                    party_max = 6,
                    party_public = true
                };

                client.EmitAsync("host", data);
            }
            if (arguments[1] == "disband")
            {
                if (current_party != null)
                {
                    var data = new PartyIDDTO()
                    {
                        party_id = current_party.party_id
                    };
                    var disbandData = new DisbandedDTO()
                    {
                        username = SteamManager.Instance.field_Private_String_0,
                        party_id = current_party.party_id
                    };
                    client.EmitAsync("disbanded",disbandData);
                    client.EmitAsync("disband", data);
                }
            }

            if (arguments[1] == "join")
            {
                var data = new PartyIDDTO()
                {
                    party_id = arguments[2]
                };
                client.EmitAsync("join", data);
            }
            if (arguments[1] == "leave")
            {
                if (current_party != null)
                {
                    var data = new PartyIDDTO()
                    {
                        party_id = current_party.party_id
                    };
                    var leftData = new JoinedDTO()
                    {
                        username = SteamManager.Instance.field_Private_String_0
                    };
                    if (current_party.party_host == client.Id)
                    {
                        client.EmitAsync("leave", data);
                        return true;
                    }
                    client.EmitAsync("left", leftData);
                    client.EmitAsync("leave", data);
                }
            }

            if (arguments[1] == "list")
            {
                var data = new PartyListDTO()
                {
                    page = 1
                };
                if (arguments.Count >= 3)
                {
                    data.page = int.Parse(arguments[2]);
                }
                client.EmitAsync("partylist", data);
            }

            if (arguments[1] == "users")
            {
                var data = new PartyListDTO()
                {
                    page = 1
                };
                if (arguments.Count >= 3)
                {
                    data.page = int.Parse(arguments[2]);
                }
                client.EmitAsync("userlist", data);
            }

            if (arguments[1] == "promote")
            {
                if (arguments.Count >= 3 && current_party != null)
                {
                    var data = new PromoteDTO()
                    {
                        party_id = current_party.party_id,
                        new_host = arguments[2]
                    };
                    client.EmitAsync("promote", data);
                }
            }

            if (arguments[1] == "warp")
            {
                var data = new WarpDTO()
                {
                    lobby_id = SteamManager.Instance.currentLobby.m_SteamID.ToString()
                };
                client.EmitAsync("warp", data);
            }

            return true;
        }

        public static void SendMessage(string message, int value = -1)
        {
            // purple
            string p_color = "<color=#e563ff>";

            // yellow
            if (value == 0)
            {
                p_color = "<color=#f7df25>";
            }
                // green
            else if (value == 1)
            {
                p_color = "<color=#09e827>";
            }
                // red
            else if (value == 2)
            {
                p_color = "<color=#fa2f28>";
            }

            /*if (!GameObject.Find("ChatBox")) {
                Tuple<string, int> message_value = new Tuple<string, int>(message, value);
                chat_buffer.Add(message_value);
            } else {
                ChatBox.Instance.ForceMessage($"<color=#e563ff>(</color>{p_color}P</color><color=#e563ff>) {message}</color>");
            }*/
            //if (client is { Connected: true }) client.EmitAsync("debug", $"{value} {message}");
            if (GameObject.Find("ChatBox")) {
                ChatBox.Instance.ForceMessage($"<color=#e563ff>(</color>{p_color}P</color><color=#e563ff>) {message}</color>");
            }
        }

        [HarmonyPatch(typeof(ChatBox), nameof(ChatBox.AppendMessage))]
        [HarmonyPriority(999)]
        [HarmonyPrefix]
        public static bool OnAppendMessage(ChatBox __instance)
        {
            if (client != null && party_chat)
                if (client.Connected)
                    return false;
            return true;
        }

        [HarmonyPatch(typeof(ClientSend), nameof(ClientSend.SendChatMessage))]
        [HarmonyPriority(999)]
        [HarmonyPrefix]
        public static bool OnSendMessage(ChatBox __instance)
        {
            if (client != null && party_chat)
                if (client.Connected)
                    return false;
            return true;
        }

        [HarmonyPatch(typeof(ChatBox), nameof(ChatBox.SendMessage))]
        [HarmonyPrefix]
        public static void OnMessage(ChatBox __instance, string __0)
        {
            if (__0 == "")
                return;
            if (client_ready && party_chat && current_party != null)
            {
                if ( __0.StartsWith("/pc"))
                    return;

                var data = new MessageDTO
                {
                    username = SteamManager.Instance.field_Private_String_0,
                    message = __0
                };
                __instance.inputField.text = "";
                __instance.inputField.interactable = false;
                SendMessage($"{data.username}: {Markup.MarkupChange(data.message)}", 0);
                client.EmitAsync("message", data);
            }
        }

        [HarmonyPatch(typeof(ChatBox), nameof(ChatBox.Awake))]
        [HarmonyPostfix]
        public static void ChatAwake(ChatBox __instance)
        {
            try {
                foreach(var message in chat_buffer)
                {
                    SendMessage(message.Item1, message.Item2);
                    Debug.Log(message.Item1);
                }
                chat_buffer = new List<Tuple<string, int>>();
            } catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        [HarmonyPatch(typeof(SteamManager), nameof(SteamManager.Update))]
        [HarmonyPostfix]
        public static void Update(SteamManager __instance)
        {
            try {
                if (warp_lobby != null)
                {
                    CSteamID lobby_id = new CSteamID
                    {
                        m_SteamID = ulong.Parse(warp_lobby)
                    };

                    SteamMatchmaking.RequestLobbyData(lobby_id);
                    string lobby_name = SteamMatchmaking.GetLobbyData(lobby_id, "LobbyName");

                    if (lobby_name == "")
                        lobby_name = "a practice lobby";

                    SendMessage($"warped too {lobby_name}", 1);

                    if (lobby_id == __instance.currentLobby) {
                        warp_lobby = null;
                        return;
                    }

                    __instance.LeaveLobby();
                    __instance.JoinLobby(lobby_id);

                    warp_lobby = null;
                }
            } catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        [HarmonyPatch(typeof(SteamManager), nameof(SteamManager.LeaveLobby))]
        [HarmonyPrefix]
        public static void OnLeave(SteamManager __instance)
        {
            if (warp_lobby == null)
            {
                var data = new JoinLeaveDTO()
                {
                  joinleave = false,
                  lobby_name = SteamMatchmaking.GetLobbyData(__instance.currentLobby, "LobbyName")
                };
                client.EmitAsync("joinleave", data);
            }
        }

        [HarmonyPatch(typeof(SteamManager), nameof(SteamManager.JoinLobby))]
        [HarmonyPostfix]
        public static void OnJoin(SteamManager __instance)
        {
            if (warp_lobby == null)
            {
                var data = new JoinLeaveDTO()
                {
                    joinleave = true,
                    lobby_name = SteamMatchmaking.GetLobbyData(__instance.currentLobby, "LobbyName")
                };
                client.EmitAsync("joinleave", data);
            }
        }
    }
}

// TODO: on force-quit game make sure to close socketio connection
// TODO: add chat buffering if there isnt a chat
// TODO: allow only 1 client and make sure its fully disconnected before connecting
// INFO: game crashes when warping people, and just randomly when receiving messages or something..?
// TODO: "warped too" is still an empty message unlike "joined too xxx"
// TOOD: /party kick, /party ban (and unban)
// TODO: /party set <public> false