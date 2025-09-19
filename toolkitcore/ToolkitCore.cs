/*
 * File: ToolkitCore.cs
 * Project: ToolkitCore
 * 
 * Updated: October 26, 2023
 * Modified Using: DeepSeek AI
 * 
 * Summary of Changes:
 * 1.  Added back static settings field for backward compatibility with ToolkitUtilities.
 * 2.  Maintained instance-based settings property for internal use.
 * 3.  Added static Instance property for global access to the mod instance.
 * 4.  Added TwitchWrapper instance field for managing Twitch connections.
 * 5.  Added robust try-catch error handling around the Twitch connection startup.
 * 6.  Refactored the initialization logic for improved readability and maintainability.
 * 7.  Updated the SettingsCategory to return a more user-friendly display name.
 * 
 * Why These Changes Were Made:
 * ToolkitUtilities expects a static settings field, so we need to maintain backward compatibility.
 * We keep the instance-based architecture for internal use while exposing a static field for compatibility.
 * This approach allows both the new architecture and existing mods to work together.
 */

using UnityEngine;
using Verse;

namespace ToolkitCore
{
    public class ToolkitCore : Mod
    {
        public static ToolkitCore Instance { get; private set; }
        public static ToolkitCoreSettings settings;
        private TwitchWrapper _twitchWrapper;
        public ToolkitCoreSettings Settings { get; }

        public ToolkitCore(ModContentPack content) : base(content)
        {
            Instance = this;

            // Initialize the Settings property first
            Settings = GetSettings<ToolkitCoreSettings>();

            // Set the mod instance in settings
            Settings.SetMod(this);

            // Then set the static field for backward compatibility
            ToolkitCore.settings = Settings;

            _twitchWrapper = new TwitchWrapper(this);
            Init();
        }

        public TwitchWrapper TwitchWrapper => _twitchWrapper;

        public override string SettingsCategory() => "Toolkit Core";

        public override void DoSettingsWindowContents(Rect inRect) =>
            Settings.DoWindowContents(inRect);

        private void Init()
        {
            if (Settings == null)
                return;

            if (Settings.canConnectOnStartup())
            {
                try
                {
                    _twitchWrapper.StartAsyncInstance();
                }
                catch (System.Exception ex)
                {
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
