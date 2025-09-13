/*
 * File: Viewers.cs
 * Project: ToolkitCore
 * 
 * Updated: September 13, 2023
 * Modified Using: DeepSeek AI
 * 
 * Summary of Changes:
 * 1. Added comprehensive null checking to prevent null reference exceptions
 * 2. Added automatic recreation of viewer list if it becomes null
 * 3. Maintained static property for backward compatibility
 * 4. Added thread-safe access to the viewer list
 * 5. Enhanced error handling with detailed logging
 */

using System;
using System.Collections.Generic;
using ToolkitCore.Database;
using Verse;

namespace ToolkitCore.Models
{
    public static class Viewers
    {
        private static readonly object _lock = new object();

        /// <summary>
        /// Gets all viewers from the global database with null protection
        /// </summary>
        public static List<Viewer> All
        {
            get
            {
                lock (_lock)
                {
                    try
                    {
                        // Ensure global database exists
                        if (ToolkitData.globalDatabase == null)
                        {
                            Log.Warning("[ToolkitCore] Global database was null. Recreating...");
                            ToolkitData.globalDatabase = new GlobalDatabase();
                        }

                        // Ensure viewers list exists
                        if (ToolkitData.globalDatabase.viewers == null)
                        {
                            Log.Warning("[ToolkitCore] Viewer list was null. Recreating...");
                            ToolkitData.globalDatabase.viewers = new List<Viewer>();
                        }

                        return ToolkitData.globalDatabase.viewers;
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"[ToolkitCore] Critical error accessing viewer database: {ex.Message}");
                        return new List<Viewer>(); // Return empty list instead of crashing
                    }
                }
            }
        }

        /// <summary>
        /// Safely adds a viewer to the list with null checking
        /// </summary>
        public static void AddViewer(Viewer viewer)
        {
            if (viewer == null)
            {
                Log.Warning("[ToolkitCore] Attempted to add null viewer");
                return;
            }

            lock (_lock)
            {
                try
                {
                    var viewers = All; // This will ensure the list exists
                    if (!viewers.Contains(viewer))
                    {
                        viewers.Add(viewer);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error($"[ToolkitCore] Error adding viewer {viewer.Username}: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Safely finds a viewer by username
        /// </summary>
        public static Viewer FindViewer(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                Log.Warning("[ToolkitCore] Attempted to find viewer with null/empty username");
                return null;
            }

            lock (_lock)
            {
                try
                {
                    var viewers = All; // This will ensure the list exists
                    return viewers.Find(v =>
                        v != null &&
                        v.Username != null &&
                        v.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
                }
                catch (Exception ex)
                {
                    Log.Error($"[ToolkitCore] Error finding viewer {username}: {ex.Message}");
                    return null;
                }
            }
        }
    }
}

//using System.Collections.Generic;

//namespace ToolkitCore.Models
//{
//    public static class Viewers
//    {
//        public static List<Viewer> All => ToolkitData.globalDatabase.viewers;
//    }
//}
