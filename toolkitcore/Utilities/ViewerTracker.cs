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
