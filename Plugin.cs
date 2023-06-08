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

        public static string party_server = "localhost:8080";

        public override void Load()
        {
            Harmony.CreateAndPatchAll(typeof(Plugin));

            modInstance = Mods.RegisterMod(PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION, "*party crab noise*");

            Commands.RegisterCommand("party", "party *", "The party command.", modInstance, PartyCommand);

            Log.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }

        public static bool PartyCommand(List<string> arguments)
        {
            if (arguments[1] == "server")
            {
                if (arguments.Count >= 3)
                {
                    party_server = arguments[2];
                }
                SendMessage($"party server -> {party_server}", 1);
                Client.Connect();
            }

            if (arguments[1] == "demo") {
                SendMessage("test");
                SendMessage("Lualt: Hello, world!", 0);
                SendMessage("created lobby", 1);
                SendMessage("couldn't find user", 2);
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

            ChatBox.Instance.ForceMessage($"<color=#e563ff>(</color>{p_color}P</color><color=#e563ff>) {message}</color>");
        }

        [HarmonyPatch(typeof(ChatBox), nameof(ChatBox.AppendMessage))]
        [HarmonyPriority(999)]
        [HarmonyPrefix]
        public static bool OnAppendMessage(ChatBox __instance)
        {
            if (client.Connected)
                return false;
            return true;
        }

        [HarmonyPatch(typeof(ClientSend), nameof(ClientSend.SendChatMessage))]
        [HarmonyPriority(999)]
        [HarmonyPrefix]
        public static bool OnSendMessage(ChatBox __instance)
        {
            if (client.Connected)
                return false;
            return true;
        }

        [HarmonyPatch(typeof(ChatBox), nameof(ChatBox.SendMessage))]
        [HarmonyPrefix]
        public static void OnMessage(ChatBox __instance, string __0)
        {
            if (client.Connected)
            {
                var data = new MessageDTO
                {
                    username = "Lualt",
                    message = __0
                };
                client.EmitAsync("message", data);
            }
        }
    }
}