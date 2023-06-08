using System;
using BepInEx;
using BepInEx.IL2CPP;
using HarmonyLib;
using System.Collections.Generic;
using qol_core;
using SocketIOSharp.Client;

namespace party_crab
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("qol-core")]
    public class Plugin : BasePlugin
    {
        public static Mod modInstance;
        public static SocketIOClient client;

        public static Tuple<string, ushort> party_server = new("localhost", 8080);

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
                if (arguments.Count >= 4)
                {
                    party_server = new(arguments[2], ushort.Parse(arguments[3]));
                }
                client.Close();
                SendMessage($"party server -> {party_server.Item1}:{party_server.Item2}", 1);
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
    }
}