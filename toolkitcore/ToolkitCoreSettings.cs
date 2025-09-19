/*
 * File: ToolkitCoreSettings.cs
 * Project: ToolkitCore
 * 
 * Updated: September 22, 2023
 * Modified Using: DeepSeek AI
 * 
 * Summary of Changes:
 * 1. Restored original static field names for backward compatibility with ToolkitUtilities.
 * 2. Added instance fields for internal use with proper synchronization.
 * 3. Added all necessary helper methods for rendering the UI.
 * 4. Maintained instance-based architecture for internal mod use.
 * 5. Integrated translation support using keys from LanguageData.xml.
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

        // Instance fields for internal use
        private string _channel_username = "";
        private string _bot_username = "";
        private string _oauth_token = "";
        private bool _connectOnGameStartup = false;
        private bool _allowWhispers = true;
        private bool _forceWhispers = false;
        private bool _sendMessageToChatOnStartup = true;
        private bool _enableDebugLogging = false;

        // Static fields for backward compatibility with ToolkitUtilities
        public static string channel_username = "";
        public static string bot_username = "";
        public static string oauth_token = "";
        public static bool connectOnGameStartup = false;
        public static bool allowWhispers = true;
        public static bool forceWhispers = false;
        public static bool sendMessageToChatOnStartup = true;
        public static bool enableDebugLogging = false;

        private bool showOauth;
        private const float verticalHeight = 32f;
        private const float verticalSpacing = 40f;

        public void SetMod(ToolkitCore mod)
        {
            _mod = mod;
            SyncStaticFields();
        }

        private void SyncStaticFields()
        {
            channel_username = _channel_username;
            bot_username = _bot_username;
            oauth_token = _oauth_token;
            connectOnGameStartup = _connectOnGameStartup;
            allowWhispers = _allowWhispers;
            forceWhispers = _forceWhispers;
            sendMessageToChatOnStartup = _sendMessageToChatOnStartup;
            enableDebugLogging = _enableDebugLogging;
        }

        /// <summary>
        /// Renders the settings window UI
        /// </summary>
        public void DoWindowContents(Rect inRect)
        {
            // Help button
            if (Widgets.ButtonText(new Rect(inRect.width - 120f, verticalSpacing, 90f, verticalHeight), "Help".Translate()))
            {
                Application.OpenURL("https://github.com/ekudram/ToolkitCore-Forked/wiki/Twitch-Chat-Connection");
            }

            // Channel Details section
            Rect sectionLabelRect = new Rect(0f, verticalSpacing, inRect.width / 2f, 64f);
            Widgets.Label(sectionLabelRect, TCText.BigText("ChannelDetails".Translate()));

            float currentY = sectionLabelRect.y + verticalSpacing * 2f;

            // Channel username field
            RenderLabelAndField(ref currentY, "Channel".Translate(), ref _channel_username, 200f);
            channel_username = _channel_username;

            // Bot username field
            RenderLabelAndField(ref currentY, "BotUsername".Translate(), ref _bot_username, 200f);
            bot_username = _bot_username;

            if (_channel_username != "" && Widgets.ButtonText(new Rect(410f, currentY - verticalSpacing, 210f, verticalHeight),
                "SameAsChannel".Translate(), true, true, true))
            {
                _bot_username = _channel_username;
                bot_username = _bot_username;
            }

            // OAuth token field
            RenderLabelAndField(ref currentY, "AccessToken".Translate(), ref _oauth_token, 200f, true);
            oauth_token = _oauth_token;

            // Connection section
            currentY += verticalSpacing * 2f;
            sectionLabelRect = new Rect(0f, currentY, inRect.width / 2f, 64f);
            Widgets.Label(sectionLabelRect, TCText.BigText("Connection".Translate()));

            currentY += verticalSpacing * 2f;

            // Connection status
            RenderConnectionStatus(ref currentY, inRect);

            // Auto-connect checkbox
            RenderCheckbox(ref currentY, "AutoConnectOnStartup".Translate(), ref _connectOnGameStartup);
            connectOnGameStartup = _connectOnGameStartup;

            // Allow whispers checkbox
            RenderCheckbox(ref currentY, "AllowViewersToWhisper".Translate(), ref _allowWhispers);
            allowWhispers = _allowWhispers;

            // Force whispers checkbox
            RenderCheckbox(ref currentY, "ForceViewersToWhisper".Translate(), ref _forceWhispers);
            forceWhispers = _forceWhispers;

            // Send connection message checkbox
            RenderCheckbox(ref currentY, "SendConnectionMessage".Translate(), ref _sendMessageToChatOnStartup);
            sendMessageToChatOnStartup = _sendMessageToChatOnStartup;

            // Debug logging checkbox
            RenderCheckbox(ref currentY, "EnableDebugLogging".Translate(), ref _enableDebugLogging);
            enableDebugLogging = _enableDebugLogging;
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
        /// <summary>
        /// Renders a password field with show/hide functionality
        /// </summary>
        private void RenderPasswordField(Rect fieldRect, ref string value)
        {
            if (showOauth)
            {
                string newValue = Widgets.TextField(fieldRect, value);
                if (newValue != value)
                {
                    // Ensure the token has the "oauth:" prefix
                    if (!string.IsNullOrEmpty(newValue) && !newValue.StartsWith("oauth:"))
                    {
                        newValue = "oauth:" + newValue;
                    }
                    value = newValue;
                }
            }
            else
            {
                Widgets.Label(fieldRect, new string('*', Math.Min(value.Length, 16)));
                if (Widgets.ButtonText(new Rect(fieldRect.x + fieldRect.width + 10f, fieldRect.y, 60f, verticalHeight), "Show".Translate(), true, true, true))
                {
                    showOauth = true;
                }
            }

            // OAuth token helper buttons
            if (Widgets.ButtonText(new Rect(fieldRect.x + fieldRect.width + 80f, fieldRect.y, 140f, verticalHeight), "GetAccessToken".Translate(), true, true, true))
            {
                Application.OpenURL("https://twitchtokengenerator.com/");
            }

            if (Widgets.ButtonText(new Rect(fieldRect.x, fieldRect.y + verticalSpacing, 200f, verticalHeight), "PasteFromClipboard".Translate(), true, true, true))
            {
                string clipboardValue = GUIUtility.systemCopyBuffer;
                // Ensure pasted token has the "oauth:" prefix
                if (!string.IsNullOrEmpty(clipboardValue) && !clipboardValue.StartsWith("oauth:"))
                {
                    clipboardValue = "oauth:" + clipboardValue;
                }
                value = clipboardValue;
            }
        }

        /// <summary>
        /// Renders the connection status and connect/disconnect button
        /// </summary>
        private void RenderConnectionStatus(ref float currentY, Rect inRect)
        {
            Rect labelRect = new Rect(0f, currentY, 200f, verticalHeight);
            Widgets.Label(labelRect, "Status".Translate());

            Rect statusRect = new Rect(200f, currentY, 200f, verticalHeight);
            Rect buttonRect = new Rect(statusRect.x + statusRect.width + 2f, currentY, statusRect.width, verticalHeight);

            bool isConnected = false;
            try
            {
                isConnected = TwitchWrapper.Client != null && TwitchWrapper.Client.IsConnected;
            }
            catch (Exception ex)
            {
                Log.Error($"[ToolkitCore] Error checking connection status: {ex.Message}");
            }

            if (isConnected)
            {
                Widgets.Label(statusRect, TCText.ColoredText("Connected".Translate(), Color.green));
                if (Widgets.ButtonText(buttonRect, "Disconnect".Translate(), true, true, true))
                {
                    try
                    {
                        TwitchWrapper.Client?.Disconnect();
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"[ToolkitCore] Error disconnecting: {ex.Message}");
                    }
                }
            }
            else
            {
                Widgets.Label(statusRect, TCText.ColoredText("NotConnected".Translate(), Color.red));
                if (Widgets.ButtonText(buttonRect, "Connect".Translate(), true, true, true))
                {
                    try
                    {
                        _mod.TwitchWrapper.StartAsyncInstance();
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
            Scribe_Values.Look(ref _channel_username, "channel_username", "");
            Scribe_Values.Look(ref _bot_username, "bot_username", "");
            Scribe_Values.Look(ref _oauth_token, "oauth_token", "");
            Scribe_Values.Look(ref _connectOnGameStartup, "connectOnGameStartup", false);
            Scribe_Values.Look(ref _allowWhispers, "allowWhispers", true);
            Scribe_Values.Look(ref _sendMessageToChatOnStartup, "sendMessageToChatOnStartup", true);
            Scribe_Values.Look(ref _forceWhispers, "forceWhispers", false);
            Scribe_Values.Look(ref _enableDebugLogging, "enableDebugLogging", false);

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                // Ensure token has proper format after loading
                if (!string.IsNullOrEmpty(_oauth_token) && !_oauth_token.StartsWith("oauth:"))
                {
                    _oauth_token = "oauth:" + _oauth_token;
                }
                SyncStaticFields();
            }
        }

        /// <summary>
        /// Determines if the mod can attempt to connect to Twitch on startup
        /// </summary>
        public bool canConnectOnStartup()
        {
            return _connectOnGameStartup &&
                   !string.IsNullOrEmpty(_bot_username) &&
                   !string.IsNullOrEmpty(_oauth_token);
        }
    }
}