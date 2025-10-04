/*
 * Project: Toolkitcore
/*
 * COMMUNITY PRESERVATION NOTICE
 * 
 * Based on: ToolkitCore (https://github.com/harleyknd1/ToolkitCore)
 * License: MIT - Added by SirRandoo on October 4, 2025
 * Original Source: https://github.com/hodlhodl1132/ToolkitCore (abandoned)
 * 
 * MAJOR MODIFICATIONS © 2025 Captolamia:
 * - Complete rewrite of event handlers for TwitchLib 3.1.4 → 3.4.0
 * - Obsoleted deprecated interfaces and methods  
 * - Updated to modern C# patterns and practices
 * 
 * This file contains substantial original work representing a major
 * derivative work. Modifications offered under GNU GPL v3.
 * 
 * Community maintainers have approved continued development.
 */

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
