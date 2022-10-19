using ToolkitCore.Controllers;
using ToolkitCore.Models;
using TwitchLib.Client.Models.Interfaces;
using Verse;

namespace ToolkitCore.Utilities
{
    public class ViewerInterface : TwitchInterfaceBase
    {
        public ViewerInterface(Game game)
        {
        }

        public override void ParseMessage(ITwitchMessage twitchCommand)
        {
            if (twitchCommand == null)
                return;
            Viewer viewer = !ViewerController.ViewerExists(twitchCommand.Username) ? ViewerController.CreateViewer(twitchCommand.Username) : ViewerController.GetViewer(twitchCommand.Username);
            if (viewer == null || twitchCommand.ChatMessage == null)
                return;
            viewer.UpdateViewerFromMessage(twitchCommand.ChatMessage);
        }
    }
}
