/*
 * File: Viewer.cs
 * Project: ToolkitCore
 * 
 * Updated: September 13, 2023
 * Modified Using: DeepSeek AI
 * 
 * Summary of Changes:
 * 1. Added comprehensive null checking in all methods
 * 2. Improved thread safety in CheckIfNewViewer method
 * 3. Enhanced error handling with detailed logging
 * 4. Added parameter validation in constructors
 * 5. Maintained all existing public APIs for backward compatibility
 * 
 * /*
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

using System;
using System.Collections.Generic;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Models;
using UnityEngine;
using Verse;

namespace ToolkitCore.Models
{
    public class Viewer : IExposable
    {
        public string Username;
        public string DisplayName;
        public string UserId;
        public bool IsBroadcaster;
        public bool IsBot;
        public bool IsModerator;
        public bool IsSubscriber;
        public List<KeyValuePair<string, string>> Badges;
        public UserType UserType;

        public Viewer()
        {
            // Initialize with empty values to prevent null references
            Username = string.Empty;
            DisplayName = string.Empty;
            UserId = string.Empty;
            Badges = new List<KeyValuePair<string, string>>();
        }

        public Viewer(string username) : this()
        {
            Username = username ?? string.Empty;
            DisplayName = username ?? string.Empty;
        }

        public Viewer(
            string username = null,
            string displayName = null,
            string userId = null,
            bool isBroadcaster = false,
            bool isBot = false,
            bool isModerator = false,
            bool isSubscriber = false) : this()
        {
            Username = username ?? string.Empty;
            DisplayName = displayName ?? username ?? string.Empty;
            UserId = userId ?? string.Empty;
            IsBroadcaster = isBroadcaster;
            IsBot = isBot;
            IsModerator = isModerator;
            IsSubscriber = isSubscriber;
        }

        public void ExposeData()
        {
            try
            {
                Scribe_Values.Look(ref Username, "Username", string.Empty);
                Scribe_Values.Look(ref DisplayName, "DisplayName", string.Empty);
                Scribe_Values.Look(ref UserId, "UserId", string.Empty);
                Scribe_Values.Look(ref IsBroadcaster, "IsBroadcaster", false);
                Scribe_Values.Look(ref IsBot, "IsBot", false);
                Scribe_Values.Look(ref IsModerator, "IsModerator", false);
                Scribe_Values.Look(ref IsSubscriber, "IsSubscriber", false);

                // Note: Badges list is not saved/loaded as it's transient data from Twitch
            }
            catch (Exception ex)
            {
                ToolkitCoreLogger.Error($"Error saving/loading viewer data for {Username}: {ex.Message}");
            }
        }

        public string ToJson()
        {
            try
            {
                return JsonUtility.ToJson(this, true);
            }
            catch (Exception ex)
            {
                ToolkitCoreLogger.Error($"Error converting viewer {Username} to JSON: {ex.Message}");
                return "{}";
            }
        }

        public void UpdateViewerFromMessage(ChatMessage chatMessage)
        {
            if (chatMessage == null)
            {
                ToolkitCoreLogger.Warning("Attempted to update viewer from null chat message");
                return;
            }

            try
            {
                DisplayName = chatMessage.DisplayName ?? string.Empty;
                UserId = chatMessage.UserId ?? string.Empty;
                IsBroadcaster = chatMessage.IsBroadcaster;
                IsBot = chatMessage.IsMe;  // Note: IsMe indicates if the message is from the bot itself
                IsModerator = chatMessage.IsModerator;
                IsSubscriber = chatMessage.IsSubscriber;
                Badges = chatMessage.Badges ?? new List<KeyValuePair<string, string>>();
                UserType = chatMessage.UserType;

                CheckIfNewViewer();
            }
            catch (Exception ex)
            {
                ToolkitCoreLogger.Error($"[ToolkitCore] Error updating viewer from message: {ex.Message}");
            }
        }

        private void CheckIfNewViewer()
        {
            try
            {
                if (string.IsNullOrEmpty(Username))
                {
                    ToolkitCoreLogger.Warning("[ToolkitCore] Attempted to add viewer with empty username");
                    return;
                }

                // Use the safe AddViewer method instead of directly accessing the list
                Viewers.AddViewer(this);
            }
            catch (Exception ex)
            {
                ToolkitCoreLogger.Error($"[ToolkitCore] Error checking if viewer {Username} is new: {ex.Message}");
            }
        }

        // Override Equals and GetHashCode for proper comparison
        public override bool Equals(object obj)
        {
            if (obj is Viewer other)
            {
                return string.Equals(Username, other.Username, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Username?.GetHashCode() ?? 0;
        }
    }
}
