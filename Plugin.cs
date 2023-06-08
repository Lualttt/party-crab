using BepInEx;
using BepInEx.IL2CPP;
using HarmonyLib;
using System.Collections.Generic;
using qol_core;

namespace party_crab
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BasePlugin
    {
        public static Mod modInstance;

        public override void Load()
        {
            Harmony.CreateAndPatchAll(typeof(Plugin));

            modInstance = Mods.RegisterMod(PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION, "*party crab noise*");

            Commands.RegisterCommand("party", "party *", "The party command.", modInstance, PartyCommand);

            Log.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }

        public static bool PartyCommand(List<string> arguments)
        {
            SendMessage("test");
            SendMessage("Lualt: Hello, world!", 1);
            SendMessage("created lobby", 2);
            SendMessage("could'nt find user", 3);

            return true;
        }

        public static void SendMessage(string message, int value = -1)
        {
            // purple
            string p_color = "<color=#bc2ed9>";

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

            ChatBox.Instance.ForceMessage($"<color=#bc2ed9>(</color>{p_color}P</color><color=#bc2ed9>{message}</color>");
        }
    }
}