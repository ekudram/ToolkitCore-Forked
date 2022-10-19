using System.Collections.Generic;
using ToolkitCore.Controllers;
using ToolkitCore.Utilities;
using TwitchLib.Client.Models;

namespace ToolkitCore.Models
{
    public static class MessageLog
    {
        private static readonly int chatMessageQueueLength = 10;
        private static readonly int whisperMessageQueueLength = 5;

        public static List<ChatMessage> LastChatMessages { get; } = new List<ChatMessage>(MessageLog.chatMessageQueueLength);

        public static List<WhisperMessage> LastWhisperMessages { get; } = new List<WhisperMessage>(MessageLog.whisperMessageQueueLength);

        public static void LogMessage(ChatMessage chatMessage)
        {
            if (MessageLog.LastChatMessages.Count >= MessageLog.chatMessageQueueLength - 1)
                MessageLog.LastChatMessages.RemoveAt(0);
            MessageLog.LastChatMessages.Add(chatMessage);
            if (!ViewerController.ViewerExists(chatMessage.Username))
                return;
            ViewerTracker.UpdateViewer(ViewerController.GetViewer(chatMessage.Username));
        }

        public static void LogMessage(WhisperMessage whisperMessage)
        {
            if (MessageLog.LastWhisperMessages.Count >= MessageLog.whisperMessageQueueLength - 1)
                MessageLog.LastWhisperMessages.RemoveAt(0);
            MessageLog.LastWhisperMessages.Add(whisperMessage);
        }
    }
}
