/*
 * File: AddonMenu.cs
 * Project: ToolkitCore
 * 
 * Updated: September 13, 2025
 * Modified Using: DeepSeek AI
 * 
 * Summary of Changes:
 * 1. Added .Translate() to all menu option strings to enable localization
 * 2. Changed "Message Log" to use the translation key "MessageLog" (without space) to match the XML
 * 3. Add Reconnect option to menu
 * 4. Wrapped Twitch reconnect logic in Task.Run to avoid blocking the UI thread
 * 5. Added null check for TwitchWrapper.Client to prevent potential null reference exceptions
 * 6. Added exception handling around Twitch disconnect to log any errors
 * 7. Added message feedback on duplicate viewer removal
 * 
 */

using RimWorld;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ToolkitCore.Controllers;
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
                }, MenuOptionPriority.Low),
                new FloatMenuOption("MessageLog".Translate(), delegate ()
                {
                    Window_MessageLog windowMessageLog = new Window_MessageLog();
                    Find.WindowStack.TryRemove(((object)windowMessageLog).GetType(), true);
                    Find.WindowStack.Add((Window)windowMessageLog);
                }, MenuOptionPriority.Low),
                new FloatMenuOption("RemoveDuplicateViewers".Translate(), delegate ()
                {
                    int removedCount = ViewerController.RemoveDuplicateViewers();
                    Messages.Message($"Removed {removedCount} duplicate viewers.", MessageTypeDefOf.TaskCompletion);
                }, MenuOptionPriority.Low),
                new FloatMenuOption("Help".Translate(), delegate ()
                {
                    Application.OpenURL("https://github.com/ekudram/ToolkitCore-Forked/wiki");
                }, MenuOptionPriority.Low),
                new FloatMenuOption("Reconnect".Translate(), delegate ()
                {
                    Task.Run(() =>
                    {
                        try
                        {
                            // Disconnect if currently connected
                            if (TwitchWrapper.Client != null && TwitchWrapper.Client.IsConnected)
                            {
                                TwitchWrapper.Client.Disconnect();
                                // Add a small delay to ensure clean disconnect
                                System.Threading.Thread.Sleep(1000);
                            }
                            
                            // Start a new connection
                            LoadedModManager.GetMod<ToolkitCore>().TwitchWrapper.StartAsync();
                        }
                        catch (Exception ex)
                        {
                            Log.Warning($"Encountered an error while reconnecting to Twitch: {ex.Message}");
                        }
                    });
                }, MenuOptionPriority.Low)
            };
            return floatMenuOptionList;
        }
    }
}