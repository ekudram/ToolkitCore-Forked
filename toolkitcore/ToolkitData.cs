/*
 * File: ToolkitData.cs
 * Project: ToolkitCore
 * 
 * Updated: September 13, 2023
 * Modified Using: DeepSeek AI
 * 
 * Summary of Changes:
 * 1. Added comprehensive documentation header
 * 2. Added null checking and error handling
 * 3. Maintained static field for backward compatibility
 * 4. Added automatic fallback initialization if GetSettings fails
 * 
 * DO NOT REMOVE THE COMMENTS - THEY ARE USED FOR TRACKING CHANGES
 * 
 * Do NOT call this to update viewers. Use TwtichToolkit.viewers instead.
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

using System.Collections.Generic;
using ToolkitCore.Database;
using ToolkitCore.Models;
using Verse;

namespace ToolkitCore
{
    public class ToolkitData : Mod
    {
        public static GlobalDatabase globalDatabase;

        public ToolkitData(ModContentPack content) : base(content)
        {
            try
            {
                // Try to get settings from saved data
                globalDatabase = GetSettings<GlobalDatabase>();

                // Ensure globalDatabase is never null
                if (globalDatabase == null)
                {
                    ToolkitCoreLogger.Warning("[ToolkitCore] GetSettings returned null. Creating new GlobalDatabase.");
                    globalDatabase = new GlobalDatabase();
                }

                // Ensure viewers list is never null
                if (globalDatabase.viewers == null)
                {
                    ToolkitCoreLogger.Warning("[ToolkitCore] Viewer list was null. Reinitializing.");
                    globalDatabase.viewers = new List<Viewer>();
                }
            }
            catch (System.Exception ex)
            {
                ToolkitCoreLogger.Error($"[ToolkitCore] Error initializing ToolkitData: {ex.Message}");

                // Fallback initialization
                globalDatabase = new GlobalDatabase();
                globalDatabase.viewers = new List<Viewer>();
            }
        }
    }
}

//using ToolkitCore.Database;
//using Verse;

//namespace ToolkitCore
//{
//    public class ToolkitData : Mod
//    {
//        public static GlobalDatabase globalDatabase;

//        public ToolkitData(ModContentPack content)
//          : base(content)
//        {
//            ToolkitData.globalDatabase = this.GetSettings<GlobalDatabase>();
//        }
//    }
//}
