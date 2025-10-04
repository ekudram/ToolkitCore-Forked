/*
 * File: GlobalDatabase.cs
 * Project: ToolkitCore
 * 
 * Updated: September 13, 2023
 * Modified Using: DeepSeek AI
 * 
 * Summary of Changes:
 * 1. Added comprehensive documentation header
 * 2. Added null initialization for viewers list
 * 3. Enhanced error handling in ExposeData method
 * 4. Maintained backward compatibility with existing code
 * 
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

using System;
using System.Collections.Generic;
using ToolkitCore.Models;
using Verse;

namespace ToolkitCore.Database
{
    public class GlobalDatabase : ModSettings
    {
        public List<Viewer> viewers = new List<Viewer>();

        /// <summary>
        /// Saves and loads the global database using RimWorld's Scribe system
        /// </summary>
        public override void ExposeData()
        {
            try
            {
                Scribe_Collections.Look(ref viewers, "viewers", LookMode.Deep);

                // Ensure viewers list is never null after loading
                if (Scribe.mode == LoadSaveMode.PostLoadInit && viewers == null)
                {
                    viewers = new List<Viewer>();
                    ToolkitCoreLogger.Warning("Viewer list was null after loading. Reinitializing.");
                }
            }
            catch (Exception ex)
            {
                ToolkitCoreLogger.Error($"Error in GlobalDatabase.ExposeData: {ex.Message}");

                // Ensure we always have a valid viewers list
                if (viewers == null)
                {
                    viewers = new List<Viewer>();
                }
            }
        }
    }
}


