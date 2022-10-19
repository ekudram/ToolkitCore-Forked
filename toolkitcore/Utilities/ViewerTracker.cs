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
            if (ViewerTracker.viewersLastActiveTime.ContainsKey(viewer.UserId))
                ViewerTracker.viewersLastActiveTime[viewer.Username] = DateTime.Now;
            else
                ViewerTracker.viewersLastActiveTime.Add(viewer.UserId, DateTime.Now);
        }

        public static int MinutesSinceLastActive(Viewer viewer)
        {
            if (viewer == null || !ViewerTracker.viewersLastActiveTime.ContainsKey(viewer.UserId))
                throw new Exception("Cannot provide Minutes since viewer was last active since viewer has not been tracker.");
            return (DateTime.Now - ViewerTracker.viewersLastActiveTime[viewer.UserId]).Minutes;
        }

        public static bool ViewerIsBeingTracker(Viewer viewer) => ViewerTracker.viewersLastActiveTime.ContainsKey(viewer.UserId);
    }
}
