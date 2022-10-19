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
        }

        public Viewer(string username) => this.Username = username;

        public Viewer(
          string Username = null,
          string DisplayName = null,
          string UserId = null,
          bool IsBroadcaster = false,
          bool IsBot = false,
          bool IsModerator = false,
          bool IsSubscriber = false)
        {
            this.Username = Username;
            this.DisplayName = DisplayName;
            this.UserId = UserId;
            this.IsBroadcaster = IsBroadcaster;
            this.IsBot = IsBot;
            this.IsModerator = IsModerator;
            this.IsSubscriber = IsSubscriber;
        }

        public void ExposeData()
        {
            Scribe_Values.Look<string>(ref this.Username, "Username", (string)null, false);
            Scribe_Values.Look<string>(ref this.DisplayName, "DisplayName", (string)null, false);
            Scribe_Values.Look<string>(ref this.UserId, "UserId", (string)null, false);
            Scribe_Values.Look<bool>(ref this.IsBroadcaster, "IsBroadcaster", false, false);
            Scribe_Values.Look<bool>(ref this.IsBot, "IsBot", false, false);
            Scribe_Values.Look<bool>(ref this.IsModerator, "IsModerator", false, false);
            Scribe_Values.Look<bool>(ref this.IsSubscriber, "IsSubscriber", false, false);
        }

        public string ToJson() => JsonUtility.ToJson((object)this, true);

        public void UpdateViewerFromMessage(ChatMessage chatMessage)
        {
            this.DisplayName = chatMessage.DisplayName;
            this.UserId = chatMessage.UserId;
            this.IsBroadcaster = chatMessage.IsBroadcaster;
            this.IsBot = chatMessage.IsMe;
            this.IsModerator = chatMessage.IsModerator;
            this.IsSubscriber = chatMessage.IsSubscriber;
            this.Badges = chatMessage.Badges;
            this.UserType = chatMessage.UserType;
            this.CheckIfNewViewer();
        }

        private void CheckIfNewViewer()
        {
            lock (Viewers.All)
            {
                lock (Viewers.All)
                {
                    if (Viewers.All.Contains(this))
                        return;
                    Viewers.All.Add(this);
                }
            }
        }
    }
}
