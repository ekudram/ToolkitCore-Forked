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
 */

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
