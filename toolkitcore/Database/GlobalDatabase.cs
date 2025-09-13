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
                    Log.Warning("[ToolkitCore] Viewer list was null after loading. Reinitializing.");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"[ToolkitCore] Error in GlobalDatabase.ExposeData: {ex.Message}");

                // Ensure we always have a valid viewers list
                if (viewers == null)
                {
                    viewers = new List<Viewer>();
                }
            }
        }
    }
}

//using System;
//using System.Collections.Generic;
//using ToolkitCore.Models;
//using Verse;

//namespace ToolkitCore.Database
//{
//    public class GlobalDatabase : ModSettings
//    {
//        public List<Viewer> viewers = new List<Viewer>();

//        public override void ExposeData() => Scribe_Collections.Look<Viewer>(ref viewers, "viewers", LookMode.Deep);
//    }
//}
