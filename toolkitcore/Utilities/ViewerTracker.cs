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
 * 
 * Key Fair Use Factors
 * 1. Transformative: Major API modernization = new creative expression
 * 2. Non-commercial: Personal/community use
 * 3. Nature: Functional code (less protected than creative works)
 * 4. Market effect: Reviving abandoned code helps community
 * 
 * This file is unchanged.
 */

using System;
using System.Collections.Generic;
using ToolkitCore.Models;

namespace ToolkitCore.Utilities
{
    public static class ViewerTracker
    {
        private static Dictionary<string, DateTime> viewersLastActiveTime = new Dictionary<string, DateTime>();

        public static void UpdateViewer(Viewer viewer)
        {
            if (viewer == null)
                throw new ArgumentNullException("viewer is null");
            if (viewersLastActiveTime.ContainsKey(viewer.UserId))
                viewersLastActiveTime[viewer.Username] = DateTime.Now;
            else
                viewersLastActiveTime.Add(viewer.UserId, DateTime.Now);
        }

        public static int MinutesSinceLastActive(Viewer viewer)
        {
            if (viewer == null || !viewersLastActiveTime.ContainsKey(viewer.UserId))
                throw new Exception("Cannot provide Minutes since viewer was last active since viewer has not been tracker.");
            return (DateTime.Now - viewersLastActiveTime[viewer.UserId]).Minutes;
        }

        public static bool ViewerIsBeingTracker(Viewer viewer) => viewersLastActiveTime.ContainsKey(viewer.UserId);
    }
}
