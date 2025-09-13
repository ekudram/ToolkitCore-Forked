/*
 * File: ViewerInterface.cs
 * Project: ToolkitCore
 * 
 * Updated: September 13, 2023
 * Modified Using: DeepSeek AI
 * 
 * Summary of Changes:
 * 1. Removed incorrect constructor that tried to pass Game parameter to base.
 * 2. Added default constructor required for GameComponent.
 * 3. Maintained error handling and documentation.
 * 
 * Why These Changes Were Made:
 * GameComponent in RimWorld does not have a constructor that accepts parameters.
 * The base GameComponent class only has a default constructor.
 */

using ToolkitCore.Controllers;
using ToolkitCore.Models;
using TwitchLib.Client.Models.Interfaces;
using Verse;

namespace ToolkitCore.Utilities
{
    public class ViewerInterface : TwitchInterfaceBase
    {
        /// <summary>
        /// Initializes a new instance of the ViewerInterface class.
        /// This default constructor is required for RimWorld GameComponents.
        /// </summary>
        public ViewerInterface()
        {
            // Default constructor is required for GameComponent
        }

        public ViewerInterface(Game game) : base()
        {
            // Empty constructor with game parameter
        }


        /// <summary>
        /// Parses a Twitch message to update or create viewer information
        /// </summary>
        /// <param name="twitchCommand">The Twitch message to parse</param>
        public override void ParseMessage(ITwitchMessage twitchCommand)
        {
            if (twitchCommand == null)
                return;

            try
            {
                Viewer viewer = !ViewerController.ViewerExists(twitchCommand.Username)
                    ? ViewerController.CreateViewer(twitchCommand.Username)
                    : ViewerController.GetViewer(twitchCommand.Username);

                if (viewer == null || twitchCommand.ChatMessage == null)
                    return;

                viewer.UpdateViewerFromMessage(twitchCommand.ChatMessage);
            }
            catch (System.Exception ex)
            {
                Log.Error($"[ViewerInterface] Error parsing message: {ex.Message}");
            }
        }
    }
}
//using ToolkitCore.Controllers;
//using ToolkitCore.Models;
//using TwitchLib.Client.Models.Interfaces;
//using Verse;

//namespace ToolkitCore.Utilities
//{
//    public class ViewerInterface : TwitchInterfaceBase
//    {
//        public ViewerInterface(Game game)
//        {
//        }

//        public override void ParseMessage(ITwitchMessage twitchCommand)
//        {
//            if (twitchCommand == null)
//                return;
//            Viewer viewer = !ViewerController.ViewerExists(twitchCommand.Username) ? ViewerController.CreateViewer(twitchCommand.Username) : ViewerController.GetViewer(twitchCommand.Username);
//            if (viewer == null || twitchCommand.ChatMessage == null)
//                return;
//            viewer.UpdateViewerFromMessage(twitchCommand.ChatMessage);
//        }
//    }
//}
