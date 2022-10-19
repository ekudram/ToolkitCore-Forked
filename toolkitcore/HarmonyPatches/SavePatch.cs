using HarmonyLib;
using System;
using System.Reflection;
using ToolkitCore.Database;
using Verse;

namespace ToolkitCore.HarmonyPatches
{
    [StaticConstructorOnStartup]
    internal static class SavePatch
    {
        static SavePatch()
        {
            Harmony harmony = new Harmony("com.rimworld.mod.hodlhodl.toolkit.core");
            Harmony.DEBUG = true;
            harmony.Patch((MethodBase)AccessTools.Method(typeof(GameDataSaveLoader), "SaveGame", (Type[])null, (Type[])null), (HarmonyMethod)null, new HarmonyMethod(typeof(SavePatch), "SaveGame_PostFix", (Type[])null), (HarmonyMethod)null, (HarmonyMethod)null);
            harmony.Patch((MethodBase)AccessTools.Method(typeof(GameDataSaveLoader), "LoadGame", new Type[1]
            {
        typeof (string)
            }, (Type[])null), (HarmonyMethod)null, new HarmonyMethod(typeof(SavePatch), "LoadGame_PostFix", (Type[])null), (HarmonyMethod)null, (HarmonyMethod)null);
        }

        private static void SaveGame_PostFix()
        {
            DatabaseController.SaveToolkit();
            ToolkitData.globalDatabase.Write();
        }

        private static void LoadGame_PostFix()
        {
            Log.Message("Running ToolkitCore loadgame_postfix");
            DatabaseController.LoadToolkit();
        }
    }
}
