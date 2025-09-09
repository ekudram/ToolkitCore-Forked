/*
 * File: ToolkitCoreSettings.cs
 * Project: ToolkitCore
 * 
 * Updated: October 26, 2023
 * Modified Using: DeepSeek AI
 * 
 * Summary of Changes:
 * 1.  Removed all static modifiers from fields to eliminate global state and align with instance-based access pattern.
 * 2.  Updated UI rendering methods to use instance fields instead of static fields.
 * 3.  Modified ExposeData() method to save/load instance fields directly.
 * 4.  Updated canConnectOnStartup() method to use instance fields.
 * 5.  Added XML documentation comments for better code understanding.
 * 6.  Improved UI layout calculations for better maintainability.
 * 7.  Added validation and error handling for Twitch connection status checks.
 * 8.  Added reference to ToolkitCore mod to access TwitchWrapper instance.
 * 
 * Why These Changes Were Made:
 * Static fields create global state that can cause issues with mod reloading and testing.
 * The changes align with the updated ToolkitCore.cs implementation that uses instance-based settings access.
 * These improvements make the code more maintainable and follow RimWorld modding best practices for version 1.4+.
 */

using System;
using ToolkitCore.Utilities;
using UnityEngine;
using Verse;

namespace ToolkitCore
{
    public class ToolkitCoreSettings : ModSettings
    {
        // Reference to the mod instance to access TwitchWrapper
        private ToolkitCore _mod;

        // Instance fields instead of static fields
        public string channel_username = "";
        public string bot_username = "";
        public string oauth_token = "";
        public bool connectOnGameStartup = false;
        public bool allowWhispers = true;
        public bool forceWhispers = false;
        public bool sendMessageToChatOnStartup = true;

        private bool showOauth;
        private const float verticalHeight = 32f;
        private const float verticalSpacing = 40f;

        // Method to set the mod reference
        public void SetMod(ToolkitCore mod)
        {
            _mod = mod;
        }

        /// <summary>
        /// Renders the settings window UI
        /// </summary>
        /// <param name="inRect">The rectangle area available for drawing the settings UI</param>
        public void DoWindowContents(Rect inRect)
        {
            // Help button
            if (Widgets.ButtonText(new Rect(inRect.width - 120f, verticalSpacing, 90f, verticalHeight), "Help", true, true, true))
            {
                Application.OpenURL("https://github.com/hodldeeznuts/ToolkitCore/wiki/Twitch-Chat-Connection");
            }

            // Channel Details section
            Rect sectionLabelRect = new Rect(0f, verticalSpacing, inRect.width / 2f, 64f);
            Widgets.Label(sectionLabelRect, TCText.BigText("Channel Details"));

            float currentY = sectionLabelRect.y + verticalSpacing * 2f;

            // Channel username field
            RenderLabelAndField(ref currentY, "Channel:", ref channel_username, 200f);

            // Bot username field
            RenderLabelAndField(ref currentY, "Bot Username:", ref bot_username, 200f);
            if (channel_username != "" && Widgets.ButtonText(new Rect(410f, currentY - verticalSpacing, 210f, verticalHeight),
                "Same as Channel", true, true, true))
            {
                bot_username = channel_username;
            }

            // OAuth token field
            RenderLabelAndField(ref currentY, "OAuth Token:", ref oauth_token, 200f, true);

            // Connection section
            currentY += verticalSpacing * 2f;
            sectionLabelRect = new Rect(0f, currentY, inRect.width / 2f, 64f);
            Widgets.Label(sectionLabelRect, TCText.BigText("Connection"));

            currentY += verticalSpacing * 2f;

            // Connection status
            RenderConnectionStatus(ref currentY, inRect);

            // Auto-connect checkbox
            RenderCheckbox(ref currentY, "Auto Connect on Startup:", ref connectOnGameStartup);

            // Allow whispers checkbox
            RenderCheckbox(ref currentY, "Allow Viewers to Whisper:", ref allowWhispers);

            // Force whispers checkbox
            RenderCheckbox(ref currentY, "Force Viewers to Whisper:", ref forceWhispers);

            // Send connection message checkbox
            RenderCheckbox(ref currentY, "Send Connection Message:", ref sendMessageToChatOnStartup);
        }

        /// <summary>
        /// Renders a label and text field pair in the settings UI
        /// </summary>
        private void RenderLabelAndField(ref float currentY, string label, ref string value, float fieldWidth, bool isPassword = false)
        {
            Rect labelRect = new Rect(0f, currentY, 200f, verticalHeight);
            Widgets.Label(labelRect, label);

            Rect fieldRect = new Rect(200f, currentY, fieldWidth, verticalHeight);

            if (isPassword)
            {
                RenderPasswordField(fieldRect, ref value);
            }
            else
            {
                value = Widgets.TextField(fieldRect, value);
            }

            currentY += verticalSpacing;
        }

        /// <summary>
        /// Renders a password field with show/hide functionality
        /// </summary>
        private void RenderPasswordField(Rect fieldRect, ref string value)
        {
            if (showOauth)
            {
                value = Widgets.TextField(fieldRect, value);
                if (Widgets.ButtonText(new Rect(fieldRect.x + fieldRect.width + 10f, fieldRect.y, 60f, verticalHeight), "Hide", true, true, true))
                {
                    showOauth = false;
                }
            }
            else
            {
                Widgets.Label(fieldRect, new string('*', Math.Min(value.Length, 16)));
                if (Widgets.ButtonText(new Rect(fieldRect.x + fieldRect.width + 10f, fieldRect.y, 60f, verticalHeight), "Show", true, true, true))
                {
                    showOauth = true;
                }
            }

            // OAuth token helper buttons
            if (Widgets.ButtonText(new Rect(fieldRect.x + fieldRect.width + 80f, fieldRect.y, 140f, verticalHeight), "New OAuth Token", true, true, true))
            {
                Application.OpenURL("https://www.twitchapps.com/tmi/");
            }

            if (Widgets.ButtonText(new Rect(fieldRect.x, fieldRect.y + verticalSpacing, 200f, verticalHeight), "Paste from Clipboard", true, true, true))
            {
                value = GUIUtility.systemCopyBuffer;
            }
        }

        /// <summary>
        /// Renders the connection status and connect/disconnect button
        /// </summary>
        private void RenderConnectionStatus(ref float currentY, Rect inRect)
        {
            Rect labelRect = new Rect(0f, currentY, 200f, verticalHeight);
            Widgets.Label(labelRect, "Status:");

            Rect statusRect = new Rect(200f, currentY, 200f, verticalHeight);
            Rect buttonRect = new Rect(statusRect.x + statusRect.width + 2f, currentY, statusRect.width, verticalHeight);

            bool isConnected = false;
            try
            {
                // Access TwitchWrapper through the mod reference
                isConnected = _mod?.TwitchWrapper?.Client != null && _mod.TwitchWrapper.Client.IsConnected;
            }
            catch (Exception ex)
            {
                Log.Error($"[ToolkitCore] Error checking connection status: {ex.Message}");
            }

            if (isConnected)
            {
                Widgets.Label(statusRect, TCText.ColoredText("Connected", Color.green));
                if (Widgets.ButtonText(buttonRect, "Disconnect", true, true, true))
                {
                    try
                    {
                        _mod.TwitchWrapper.Client.Disconnect();
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"[ToolkitCore] Error disconnecting: {ex.Message}");
                    }
                }
            }
            else
            {
                Widgets.Label(statusRect, TCText.ColoredText("Not Connected", Color.red));
                if (Widgets.ButtonText(buttonRect, "Connect", true, true, true))
                {
                    try
                    {
                        _mod.TwitchWrapper.StartAsync();
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"[ToolkitCore] Error connecting: {ex.Message}");
                    }
                }
            }

            currentY += verticalSpacing;
        }

        /// <summary>
        /// Renders a labeled checkbox in the settings UI
        /// </summary>
        private void RenderCheckbox(ref float currentY, string label, ref bool value)
        {
            Rect labelRect = new Rect(0f, currentY, 200f, verticalHeight);
            Widgets.Label(labelRect, label);

            Rect checkboxRect = new Rect(200f, currentY, 24f, verticalHeight);
            Widgets.Checkbox(checkboxRect.position, ref value, 24f);

            currentY += verticalSpacing;
        }

        /// <summary>
        /// Saves and loads mod settings
        /// </summary>
        public override void ExposeData()
        {
            Scribe_Values.Look(ref channel_username, "channel_username", "");
            Scribe_Values.Look(ref bot_username, "bot_username", "");
            Scribe_Values.Look(ref oauth_token, "oauth_token", "");
            Scribe_Values.Look(ref connectOnGameStartup, "connectOnGameStartup", false);
            Scribe_Values.Look(ref allowWhispers, "allowWhispers", true);
            Scribe_Values.Look(ref sendMessageToChatOnStartup, "sendMessageToChatOnStartup", true);
            Scribe_Values.Look(ref forceWhispers, "forceWhispers", false);
        }

        /// <summary>
        /// Determines if the mod can attempt to connect to Twitch on startup
        /// </summary>
        /// <returns>True if all required credentials are present and auto-connect is enabled</returns>
        public bool canConnectOnStartup()
        {
            return connectOnGameStartup &&
                   !string.IsNullOrEmpty(bot_username) &&
                   !string.IsNullOrEmpty(oauth_token);
        }
    }
}

//using System;
//using ToolkitCore.Utilities;
//using UnityEngine;
//using Verse;

//namespace ToolkitCore
//{
//    public class ToolkitCoreSettings : ModSettings
//    {
//        public static string channel_username = "";
//        public static string bot_username = "";
//        public static string oauth_token = "";
//        public static bool connectOnGameStartup = false;
//        public static bool allowWhispers = true;
//        public static bool forceWhispers = false;
//        public static bool sendMessageToChatOnStartup = true;
//        private bool showOauth;
//        private static readonly float verticalHeight = 32f;
//        private static readonly float verticalSpacing = 40f;

//        public void DoWindowContents(Rect inRect)
//        {
//            if (Widgets.ButtonText(new Rect((inRect).width - 120f, verticalSpacing, 90f, verticalHeight), "Help", true, true, true))
//            {
//                Application.OpenURL("https://github.com/hodldeeznuts/ToolkitCore/wiki/Twitch-Chat-Connection");
//            }
//            Rect val = new Rect(0f, verticalSpacing, inRect.width / 2f, 64f);
//            Widgets.Label(val, TCText.BigText("Channel Details"));
//            float num = val.y + verticalSpacing * 2f;
//            Rect val2 = new Rect(0f, num, 200f, verticalHeight);
//            Widgets.Label(val2, "Channel:");
//            val2.y = val2.y + verticalSpacing;
//            Widgets.Label(val2, "Bot Username:");
//            val2.y = val2.y + verticalSpacing;
//            Widgets.Label(val2, "OAuth Token:");
//            Rect val3 = new Rect(200f, num, 200f, verticalHeight);
//            channel_username = Widgets.TextField(val3, channel_username);
//            val3.y = val3.y + verticalSpacing;
//            bot_username = Widgets.TextField(val3, bot_username);
//            if (channel_username != "" && Widgets.ButtonText(new Rect(((val3)).x + ((val3)).width + 10f, ((val3)).y, 210f, verticalHeight), "Same as Channel", true, true, true))
//            {
//                bot_username = channel_username;
//            }
//            val3.y = val3.y + verticalSpacing;
//            Rect val4 = new Rect(((val3)).x + ((val3)).width + 10f, ((val3)).y, 60f, verticalHeight);
//            if (showOauth)
//            {
//                oauth_token = Widgets.TextField(val3, oauth_token);
//                if (Widgets.ButtonText(val4, "Hide", true, true, true))
//                {
//                    showOauth = !showOauth;
//                }
//            }
//            else
//            {
//                Widgets.Label(val3, new string('*', Math.Min(oauth_token.Length, 16)));
//                if (Widgets.ButtonText(val4, "Show", true, true, true))
//                {
//                    showOauth = !showOauth;
//                }
//            }
//            if (Widgets.ButtonText(new Rect(val4.x + val4.width + 10f, val3.y, 140f, verticalHeight), "New OAuth Token", true, true, true))
//            {
//                Application.OpenURL("https://www.twitchapps.com/tmi/");
//            }
//            val3.y = val3.y + verticalSpacing;
//            if (Widgets.ButtonText(val3, "Paste from Clipboard", true, true, true))
//            {
//                oauth_token = GUIUtility.systemCopyBuffer;
//            }
//            Rect val5 = new Rect(0f, val3.y + verticalSpacing * 2f, inRect.width / 2f, 64f);
//            Widgets.Label(val5, TCText.BigText("Connection"));
//            num = val5.y + verticalSpacing * 2f;
//            val2.y = (num);
//            val3.y = num;
//            Widgets.Label(val2, "Status:");
//            Rect val6 = new Rect(((val3)).x + ((val3)).width + 2f, ((val3)).y, ((val3)).width, verticalHeight);
//            if (TwitchWrapper.Client != null && TwitchWrapper.Client.IsConnected)
//            {
//                Widgets.Label(val3, TCText.ColoredText("Connected", Color.green));
//                if (Widgets.ButtonText(val6, "Disconnect", true, true, true))
//                {
//                    TwitchWrapper.Client.Disconnect();
//                }
//            }
//            else
//            {
//                Widgets.Label(val3, TCText.ColoredText("Not Connected", Color.red));
//                if (Widgets.ButtonText(val6, "Connect", true, true, true))
//                {
//                    TwitchWrapper.StartAsync();
//                }
//            }
//            val2.y = val2.y + verticalSpacing;
//            Widgets.Label(val2, "Auto Connect on Startup:");
//            val3.y = val2.y;
//            Widgets.Checkbox(((val3)).position, ref connectOnGameStartup, 24f, false, false, null, null);
//            val2.y = val2.y + verticalSpacing;
//            Widgets.Label(val2, "Allow Viewers to Whisper:");
//            val3.y = val2.y;
//            Widgets.Checkbox(((val3)).position, ref allowWhispers, 24f, false, false, null, null);
//            val2.y = val2.y + verticalSpacing;
//            Widgets.Label(val2, "Force Viewers to Whisper:");
//            val3.y = val2.y;
//            Widgets.Checkbox(((val3)).position, ref forceWhispers, 24f, false, false, null,null);
//            val2.y = val2.y + verticalSpacing;
//            Widgets.Label(val2, "Send Connection Message:");
//            val3.y = val2.y;
//            Widgets.Checkbox(((val3)).position, ref sendMessageToChatOnStartup, 24f);

//        }

//        public override void ExposeData()
//        {
//            Scribe_Values.Look<string>(ref ToolkitCoreSettings.channel_username, "channel_username", "", false);
//            Scribe_Values.Look<string>(ref ToolkitCoreSettings.bot_username, "bot_username", "", false);
//            Scribe_Values.Look<string>(ref ToolkitCoreSettings.oauth_token, "oauth_token", "", false);
//            Scribe_Values.Look<bool>(ref ToolkitCoreSettings.connectOnGameStartup, "connectOnGameStartup", false, false);
//            Scribe_Values.Look<bool>(ref ToolkitCoreSettings.allowWhispers, "allowWhispers", true, false);
//            Scribe_Values.Look<bool>(ref ToolkitCoreSettings.sendMessageToChatOnStartup, "sendMessageToChatOnStartup", true, false);
//            Scribe_Values.Look<bool>(ref ToolkitCoreSettings.forceWhispers, "forceWhispers", false, false);
//        }

//        public bool canConnectOnStartup()
//        {
//            ExposeData();
//            return (connectOnGameStartup && !string.IsNullOrEmpty(bot_username) && !string.IsNullOrEmpty(oauth_token));
//        }
//    }
//}
