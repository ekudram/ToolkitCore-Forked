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
                Log.Error($"[ToolkitCore] Error saving/loading viewer data for {Username}: {ex.Message}");
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
                Log.Error($"[ToolkitCore] Error converting viewer {Username} to JSON: {ex.Message}");
                return "{}";
            }
        }

        public void UpdateViewerFromMessage(ChatMessage chatMessage)
        {
            if (chatMessage == null)
            {
                Log.Warning("[ToolkitCore] Attempted to update viewer from null chat message");
                return;
            }

            try
            {
                DisplayName = chatMessage.DisplayName ?? string.Empty;
                UserId = chatMessage.UserId ?? string.Empty;
                IsBroadcaster = chatMessage.IsBroadcaster;
                IsBot = chatMessage.IsMe;
                IsModerator = chatMessage.IsModerator;
                IsSubscriber = chatMessage.IsSubscriber;
                Badges = chatMessage.Badges ?? new List<KeyValuePair<string, string>>();
                UserType = chatMessage.UserType;

                CheckIfNewViewer();
            }
            catch (Exception ex)
            {
                Log.Error($"[ToolkitCore] Error updating viewer from message: {ex.Message}");
            }
        }

        private void CheckIfNewViewer()
        {
            try
            {
                if (string.IsNullOrEmpty(Username))
                {
                    Log.Warning("[ToolkitCore] Attempted to add viewer with empty username");
                    return;
                }

                // Use the safe AddViewer method instead of directly accessing the list
                Viewers.AddViewer(this);
            }
            catch (Exception ex)
            {
                Log.Error($"[ToolkitCore] Error checking if viewer {Username} is new: {ex.Message}");
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
//using System.Collections.Generic;
//using TwitchLib.Client.Enums;
//using TwitchLib.Client.Models;
//using UnityEngine;
//using Verse;

//namespace ToolkitCore.Models
//{
//    public class Viewer : IExposable
//    {
//        public string Username;
//        public string DisplayName;
//        public string UserId;
//        public bool IsBroadcaster;
//        public bool IsBot;
//        public bool IsModerator;
//        public bool IsSubscriber;
//        public List<KeyValuePair<string, string>> Badges;
//        public UserType UserType;

//        public Viewer()
//        {
//        }

//        public Viewer(string username) => this.Username = username;

//        public Viewer(
//          string Username = null,
//          string DisplayName = null,
//          string UserId = null,
//          bool IsBroadcaster = false,
//          bool IsBot = false,
//          bool IsModerator = false,
//          bool IsSubscriber = false)
//        {
//            this.Username = Username;
//            this.DisplayName = DisplayName;
//            this.UserId = UserId;
//            this.IsBroadcaster = IsBroadcaster;
//            this.IsBot = IsBot;
//            this.IsModerator = IsModerator;
//            this.IsSubscriber = IsSubscriber;
//        }

//        public void ExposeData()
//        {
//            Scribe_Values.Look<string>(ref this.Username, "Username", (string)null, false);
//            Scribe_Values.Look<string>(ref this.DisplayName, "DisplayName", (string)null, false);
//            Scribe_Values.Look<string>(ref this.UserId, "UserId", (string)null, false);
//            Scribe_Values.Look<bool>(ref this.IsBroadcaster, "IsBroadcaster", false, false);
//            Scribe_Values.Look<bool>(ref this.IsBot, "IsBot", false, false);
//            Scribe_Values.Look<bool>(ref this.IsModerator, "IsModerator", false, false);
//            Scribe_Values.Look<bool>(ref this.IsSubscriber, "IsSubscriber", false, false);
//        }

//        public string ToJson() => JsonUtility.ToJson((object)this, true);

//        public void UpdateViewerFromMessage(ChatMessage chatMessage)
//        {
//            this.DisplayName = chatMessage.DisplayName;
//            this.UserId = chatMessage.UserId;
//            this.IsBroadcaster = chatMessage.IsBroadcaster;
//            this.IsBot = chatMessage.IsMe;
//            this.IsModerator = chatMessage.IsModerator;
//            this.IsSubscriber = chatMessage.IsSubscriber;
//            this.Badges = chatMessage.Badges;
//            this.UserType = chatMessage.UserType;
//            this.CheckIfNewViewer();
//        }

//        private void CheckIfNewViewer()
//        {
//            lock (Viewers.All)
//            {
//                lock (Viewers.All)
//                {
//                    if (Viewers.All.Contains(this))
//                        return;
//                    Viewers.All.Add(this);
//                }
//            }
//        }
//    }
//}
