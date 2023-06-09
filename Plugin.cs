using BepInEx;
using BepInEx.IL2CPP;
using HarmonyLib;
using System.Collections.Generic;
using qol_core;
using SocketIOClient;

namespace party_crab
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("qol-core")]
    public class Plugin : BasePlugin
    {
        public static Mod modInstance;
        public static SocketIO client;

        public static bool client_ready;

        public static string party_server = "localhost:8080";
        public static Party current_party;
        public static bool party_chat;

        public override void Load()
        {
            Harmony.CreateAndPatchAll(typeof(Plugin));

            modInstance = Mods.RegisterMod(PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION, "*party crab noise*");

            Commands.RegisterCommand("party", "party *", "The party command.", modInstance, PartyCommand);

            Commands.RegisterCommand("ac", "ac", "Go into all chat.", modInstance, AllChatCommand);
            Commands.RegisterCommand("pc", "pc", "Go into party chat.", modInstance, PartyChatCommand);

            Log.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }

        public static bool AllChatCommand(List<string> arguments)
        {
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
                    party_public = false
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
                client.EmitAsync("partylist");

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

            ChatBox.Instance.ForceMessage($"<color=#e563ff>(</color>{p_color}P</color><color=#e563ff>) {message}</color>");
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
            if (client_ready && party_chat && current_party != null)
            {
                var data = new MessageDTO
                {
                    username = SteamManager.Instance.field_Private_String_0,
                    message = __0
                };
                SendMessage($"{data.username}: {data.message}", 0);
                client.EmitAsync("message", data);
            }
        }
    }
}