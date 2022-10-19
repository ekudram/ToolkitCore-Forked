using System;
using ToolkitCore.Models;

namespace ToolkitCore.Controllers
{
    public static class ViewerController
    {
        public static Viewer CreateViewer(string Username)
        {
            Viewer viewer = !ViewerController.ViewerExists(Username) ? new Viewer(Username) : throw new Exception("Viewer already exists");
            Viewers.All.Add(viewer);
            return viewer;
        }

        public static Viewer GetViewer(string Username) => Viewers.All.Find((Predicate<Viewer>)(vwr => vwr.Username == Username));

        public static bool ViewerExists(string Username) => Viewers.All.Find((Predicate<Viewer>)(x => x.Username == Username)) != null;
    }
}
