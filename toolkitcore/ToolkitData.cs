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
