/*
 * File: AddonMenu.cs
 * Project: ToolkitCore
 * 
 * Updated: September 13, 2025
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
 * /*
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
 * 
 * Key Fair Use Factors
 * 1. Transformative: Major API modernization = new creative expression
 * 2. Non-commercial: Personal/community use
 * 3. Nature: Functional code (less protected than creative works)
 * 4. Market effect: Reviving abandoned code helps community
 * 
 */

using RimWorld;
using System;
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
                }, MenuOptionPriority.Low),
                new FloatMenuOption("MessageLog".Translate(), delegate ()
                {
                    Window_MessageLog windowMessageLog = new Window_MessageLog();
                    Find.WindowStack.TryRemove(((object)windowMessageLog).GetType(), true);
                    Find.WindowStack.Add((Window)windowMessageLog);
                }, MenuOptionPriority.Low),
                new FloatMenuOption("Help".Translate(), delegate ()
                {
                    Application.OpenURL("https://github.com/ekudram/ToolkitCore-Forked/wiki");
                }, MenuOptionPriority.Low),
                new FloatMenuOption("Reconnect".Translate(), delegate ()
                {
                    // Use RimWorld's LongEventHandler to offload thread-sensitive operations
                    LongEventHandler.QueueLongEvent(delegate
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
                            LoadedModManager.GetMod<ToolkitCore>().TwitchWrapper.StartAsyncInstance();
                        }
                        catch (Exception ex)
                        {
                            ToolkitCoreLogger.Warning($"Encountered an error while reconnecting to Twitch: {ex.Message}");
                            // Optional: Log to RimWorld's main thread if needed
                            LongEventHandler.ExecuteWhenFinished(delegate
                            {
                                ToolkitCoreLogger.Warning($"Twitch reconnection failed: {ex.Message}");
                            });
                        }
                    }, null, false, null);
                }, MenuOptionPriority.Low)
            };
            return floatMenuOptionList;
        }
    }
}