/*
 * File: AddonMenu.cs
 * Project: ToolkitCore
 * 
 * Updated: September 22, 2023
 * Modified Using: DeepSeek AI
 * 
 * Summary of Changes:
 * 1. Added .Translate() to all menu option strings to enable localization
 * 2. Changed "Message Log" to use the translation key "MessageLog" (without space) to match the XML
 */
using System.Collections.Generic;
using ToolkitCore.Interfaces;
using ToolkitCore.Windows;
using UnityEngine;
using Verse;

namespace ToolkitCore
{
    public class AddonMenu : IAddonMenu
    {
        List<FloatMenuOption> IAddonMenu.MenuOptions()
        {
            List<FloatMenuOption> floatMenuOptionList = new List<FloatMenuOption>{
                new FloatMenuOption("Settings".Translate(), delegate ()
                {
                    Window_ModSettings windowModSettings = new Window_ModSettings((Mod) LoadedModManager.GetMod<ToolkitCore>());
                    Find.WindowStack.TryRemove(((object) windowModSettings).GetType(), true);
                    Find.WindowStack.Add((Window) windowModSettings);
                },(MenuOptionPriority) 4),
                new FloatMenuOption("MessageLog".Translate(), delegate ()
                {
                    Window_MessageLog windowMessageLog = new Window_MessageLog();
                    Find.WindowStack.TryRemove(((object)windowMessageLog).GetType(), true);
                    Find.WindowStack.Add((Window)windowMessageLog);
                },(MenuOptionPriority) 4),
                new FloatMenuOption("Help".Translate(), delegate ()
                {
                    Application.OpenURL("https://github.com/hodldeeznuts/ToolkitCore/wiki");
                },(MenuOptionPriority) 4)
            };
            return floatMenuOptionList;
        }
    }
}
