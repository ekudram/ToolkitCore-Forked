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

using System.Collections.Generic;
using ToolkitCore.Controllers;
using ToolkitCore.Models;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Models;
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
        /// Parses a Twitch chat message to update or create viewer information
        /// </summary>
        public override void ParseMessage(ChatMessage chatMessage)
        {
            if (chatMessage == null)
                return;

            try
            {
                UpdateViewerFromMessage(chatMessage.Username, chatMessage);
            }
            catch (System.Exception ex)
            {
                ToolkitCoreLogger.Error($"[ViewerInterface] Error parsing chat message: {ex.Message}");
            }
        }

        /// <summary>
        /// Parses a Twitch whisper message to update or create viewer information
        /// </summary>
        public override void ParseWhisper(WhisperMessage whisperMessage)
        {
            if (whisperMessage == null)
                return;

            try
            {
                UpdateViewerFromMessage(whisperMessage.Username, whisperMessage);
            }
            catch (System.Exception ex)
            {
                ToolkitCoreLogger.Error($"[ViewerInterface] Error parsing whisper message: {ex.Message}");
            }
        }

        /// <summary>
        /// Common method to update viewer from either chat or whisper messages
        /// </summary>
        private void UpdateViewerFromMessage(string username, object message)
        {
            if (string.IsNullOrEmpty(username))
                return;

            Viewer viewer = !ViewerController.ViewerExists(username)
                ? ViewerController.CreateViewer(username)
                : ViewerController.GetViewer(username);

            if (viewer == null)
                return;

            // Update viewer based on message type
            if (message is ChatMessage chatMessage)
            {
                viewer.UpdateViewerFromMessage(chatMessage);
            }
            else if (message is WhisperMessage whisperMessage)
            {
                // You might need to create an UpdateViewerFromWhisper method
                // or modify UpdateViewerFromMessage to handle both types
                viewer.UpdateViewerFromMessage(ConvertWhisperToChatMessage(whisperMessage));
            }
        }

        /// <summary>
        /// Converts a WhisperMessage to a ChatMessage-like object for compatibility
        /// </summary>
        private ChatMessage ConvertWhisperToChatMessage(WhisperMessage whisperMessage)
        {
            // Create a minimal ChatMessage with the essential properties
            // This is a simplified approach - you might need to adjust based on your needs
            return new ChatMessage(
                botUsername: "",
                userId: whisperMessage.UserId,
                userName: whisperMessage.Username,
                displayName: whisperMessage.DisplayName,
                colorHex: "",
                color: System.Drawing.Color.Empty,
                emoteSet: new EmoteSet("", whisperMessage.Message), // Fixed: empty string instead of null
                message: whisperMessage.Message,
                userType: UserType.Viewer, // Default type
                channel: "",
                id: "",
                isSubscriber: false,
                subscribedMonthCount: 0,
                roomId: "",
                isTurbo: false,
                isModerator: false,
                isMe: false,
                isBroadcaster: false,
                isVip: false,
                isPartner: false,
                isStaff: false,
                noisy: Noisy.False,
                rawIrcMessage: "",
                emoteReplacedMessage: "",
                badges: new List<KeyValuePair<string, string>>(),
                cheerBadge: null,
                bits: 0,
                bitsInDollars: 0
            );
        }
    }
}
