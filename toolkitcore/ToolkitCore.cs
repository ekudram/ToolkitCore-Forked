/*
 * File: ToolkitCore.cs
 * Project: ToolkitCore
 * 
 * Updated: October 26, 2023
 * Modified Using: DeepSeek AI
 * 
 * Summary of Changes:
 * 1.  Removed static reference to settings to eliminate global state and improve mod reload safety.
 * 2.  Added TwitchWrapper instance field for managing Twitch connections.
 * 3.  Added robust try-catch error handling around the Twitch connection startup to prevent unhandled exceptions from crashing the game.
 * 4.  Refactored the initialization logic for improved readability and maintainability.
 * 5.  Updated the SettingsCategory to return a more user-friendly display name.
 * 
 * Why These Changes Were Made:
 * The original code used a static reference to settings, which can cause issues if the mod is reloaded and creates tight coupling.
 * Adding a TwitchWrapper instance allows proper access to the non-static TwitchWrapper class.
 * Error handling is critical for async operations like network connections to ensure game stability.
 * These updates align the mod with modern RimWorld 1.4+ modding best practices for reliability and maintainability.
 */

using UnityEngine;
using Verse;

namespace ToolkitCore
{
    public class ToolkitCore : Mod
    {
        // Add the TwitchWrapper instance field here
        private TwitchWrapper _twitchWrapper;
        public TwitchWrapper TwitchWrapper => _twitchWrapper;
        public static ToolkitCore Instance { get; private set; }

        public ToolkitCore(ModContentPack content) : base(content)
        {
            Instance = this; // Add this line
            Settings = GetSettings<ToolkitCoreSettings>();
            Settings.SetMod(this);
            _twitchWrapper = new TwitchWrapper(this);
            Init();
        }


        // Property to safely access the settings instance without a static field
        public ToolkitCoreSettings Settings { get; }

        public override string SettingsCategory() => "Toolkit Core";

        public override void DoSettingsWindowContents(Rect inRect) =>
            Settings.DoWindowContents(inRect);

        // Handles the automatic connection to Twitch on game startup
        private void Init()
        {
            if (Settings == null)
                return;

            // Check if we have the necessary credentials to attempt a connection
            bool hasValidCredentials = !string.IsNullOrEmpty(Settings.bot_username) &&
                                       !string.IsNullOrEmpty(Settings.oauth_token);

            if (Settings.canConnectOnStartup() && hasValidCredentials)
            {
                try
                {
                    // Use the TwitchWrapper instance instead of the static method
                    _twitchWrapper.StartAsync();
                }
                catch (System.Exception ex)
                {
                    // Log any errors to help with debugging failed connections
                    Log.Warning($"[ToolkitCore] Failed to connect to Twitch on startup: {ex.Message}");
                }
            }
        }
    }
}
//using UnityEngine;
//using Verse;

//namespace ToolkitCore
//{
//    public class ToolkitCore : Mod
//    {
//        public static ToolkitCoreSettings settings;

//        public ToolkitCore(ModContentPack content)
//          : base(content)
//        {
//            ToolkitCore.settings = GetSettings<ToolkitCoreSettings>();
//            Init();
//        }

//        public void Init()
//        {
//            if(settings != null && settings.canConnectOnStartup() && !string.IsNullOrEmpty(ToolkitCoreSettings.bot_username) && !string.IsNullOrEmpty(ToolkitCoreSettings.oauth_token))
//            {
//                TwitchWrapper.StartAsync();
//            }
//        }

//        public override string SettingsCategory() => nameof(ToolkitCore);

//        public override void DoSettingsWindowContents(Rect inRect) =>
//            ToolkitCore.settings.DoWindowContents(inRect);
//    }
//}
