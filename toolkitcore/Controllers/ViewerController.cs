/*
 * File: ViewerController.cs
 * Project: ToolkitCore
 * 
 * Updated: [Current Date]
 * Modified Using: DeepSeek AI
 * 
 * Summary of Changes:
 * 1. Added comprehensive documentation header
 * 2. Maintained static methods for backward compatibility
 * 3. Added null checks and input validation
 * 4. Enhanced error handling with detailed logging
 * 5. Added case-insensitive username comparison for Twitch compatibility
 * 6. Added duplicate prevention and cleanup functionality
 * 7. Improved exception handling with more descriptive messages
 */

using System;
using System.Collections.Generic;
using System.Linq;
using ToolkitCore.Models;
using Verse;

namespace ToolkitCore.Controllers
{
    public static class ViewerController
    {
        private static readonly object _lock = new object();

        /// <summary>
        /// Creates a new viewer with the specified username
        /// </summary>
        /// <param name="username">The viewer's username</param>
        /// <returns>The created viewer instance</returns>
        /// <exception cref="ArgumentNullException">Thrown if username is null or empty</exception>
        /// <exception cref="InvalidOperationException">Thrown if viewer already exists</exception>
        public static Viewer CreateViewer(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                Log.Error("[ToolkitCore] Attempted to create viewer with null or empty username");
                throw new ArgumentNullException(nameof(username), "Username cannot be null or empty");
            }

            lock (_lock)
            {
                try
                {
                    if (ViewerExists(username))
                    {
                        Log.Warning($"[ToolkitCore] Attempted to create duplicate viewer: {username}");
                        throw new InvalidOperationException($"Viewer '{username}' already exists");
                    }

                    Viewer viewer = new Viewer(username);
                    Viewers.AddViewer(viewer);

                    Log.Message($"[ToolkitCore] Created new viewer: {username}");
                    return viewer;
                }
                catch (Exception ex)
                {
                    Log.Error($"[ToolkitCore] Error creating viewer {username}: {ex.Message}");
                    throw;
                }
            }
        }

        /// <summary>
        /// Retrieves a viewer by username (case-insensitive)
        /// </summary>
        /// <param name="username">The username to search for</param>
        /// <returns>The viewer instance or null if not found</returns>
        public static Viewer GetViewer(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                Log.Warning("[ToolkitCore] Attempted to get viewer with null or empty username");
                return null;
            }

            try
            {
                // Use case-insensitive comparison for Twitch usernames
                return Viewers.All.Find(v =>
                    v != null &&
                    v.Username != null &&
                    v.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            }
            catch (Exception ex)
            {
                Log.Error($"[ToolkitCore] Error getting viewer {username}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Checks if a viewer exists (case-insensitive)
        /// </summary>
        /// <param name="username">The username to check</param>
        /// <returns>True if the viewer exists, false otherwise</returns>
        public static bool ViewerExists(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                Log.Warning("[ToolkitCore] Attempted to check existence of viewer with null or empty username");
                return false;
            }

            try
            {
                return GetViewer(username) != null;
            }
            catch (Exception ex)
            {
                Log.Error($"[ToolkitCore] Error checking if viewer exists {username}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Removes duplicate viewers based on username (case-insensitive)
        /// </summary>
        /// <returns>The number of duplicates removed</returns>
        public static int RemoveDuplicateViewers()
        {
            lock (_lock)
            {
                try
                {
                    var viewers = Viewers.All;
                    if (viewers == null || viewers.Count == 0)
                        return 0;

                    var uniqueViewers = new Dictionary<string, Viewer>(StringComparer.OrdinalIgnoreCase);
                    var duplicates = new List<Viewer>();

                    foreach (var viewer in viewers)
                    {
                        if (viewer == null || string.IsNullOrEmpty(viewer.Username))
                            continue;

                        if (uniqueViewers.ContainsKey(viewer.Username))
                        {
                            duplicates.Add(viewer);
                        }
                        else
                        {
                            uniqueViewers[viewer.Username] = viewer;
                        }
                    }

                    // Remove duplicates
                    foreach (var duplicate in duplicates)
                    {
                        viewers.Remove(duplicate);
                    }

                    if (duplicates.Count > 0)
                    {
                        Log.Message($"[ToolkitCore] Removed {duplicates.Count} duplicate viewers");
                    }

                    return duplicates.Count;
                }
                catch (Exception ex)
                {
                    Log.Error($"[ToolkitCore] Error removing duplicate viewers: {ex.Message}");
                    return 0;
                }
            }
        }

        /// <summary>
        /// Gets or creates a viewer by username (thread-safe)
        /// </summary>
        /// <param name="username">The username to get or create</param>
        /// <returns>The existing or newly created viewer</returns>
        public static Viewer GetOrCreateViewer(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                Log.Error("[ToolkitCore] Attempted to get or create viewer with null or empty username");
                return null;
            }

            lock (_lock)
            {
                try
                {
                    var viewer = GetViewer(username);
                    if (viewer != null)
                        return viewer;

                    return CreateViewer(username);
                }
                catch (Exception ex)
                {
                    Log.Error($"[ToolkitCore] Error getting or creating viewer {username}: {ex.Message}");
                    return null;
                }
            }
        }

        /// <summary>
        /// Updates a viewer's information from a chat message
        /// </summary>
        /// <param name="username">The viewer's username</param>
        /// <param name="updateAction">Action to update the viewer's properties</param>
        /// <returns>True if the viewer was found and updated, false otherwise</returns>
        public static bool UpdateViewer(string username, Action<Viewer> updateAction)
        {
            if (string.IsNullOrEmpty(username) || updateAction == null)
            {
                Log.Warning("[ToolkitCore] Invalid parameters for UpdateViewer");
                return false;
            }

            lock (_lock)
            {
                try
                {
                    var viewer = GetViewer(username);
                    if (viewer == null)
                        return false;

                    updateAction(viewer);
                    return true;
                }
                catch (Exception ex)
                {
                    Log.Error($"[ToolkitCore] Error updating viewer {username}: {ex.Message}");
                    return false;
                }
            }
        }
    }
}

//using System;
//using ToolkitCore.Models;

//namespace ToolkitCore.Controllers
//{
//    public static class ViewerController
//    {
//        public static Viewer CreateViewer(string Username)
//        {
//            Viewer viewer = !ViewerExists(Username) ? new Viewer(Username) : throw new Exception("Viewer already exists");
//            Viewers.All.Add(viewer);
//            return viewer;
//        }

//        public static Viewer GetViewer(string Username) => Viewers.All.Find((Predicate<Viewer>)(vwr => vwr.Username == Username));

//        public static bool ViewerExists(string Username) => Viewers.All.Find((Predicate<Viewer>)(x => x.Username == Username)) != null;
//    }
//}
