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
 * 
 * Key Fair Use Factors
 * 1. Transformative: Major API modernization = new creative expression
 * 2. Non-commercial: Personal/community use
 * 3. Nature: Functional code (less protected than creative works)
 * 4. Market effect: Reviving abandoned code helps community
 * 
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

            ToolkitCore.settings = Settings;

            _twitchWrapper = new TwitchWrapper(this);
            Init();
        }

        public TwitchWrapper TwitchWrapper => _twitchWrapper;

        public override string SettingsCategory() => "Toolkit Core 2.0e";

        public override void DoSettingsWindowContents(Rect inRect) =>
            Settings.DoWindowContents(inRect);

        private void Init()
        {
            if (Settings == null)
                return;
            Settings.SetMod(this);

            // Force save to create the settings file with current values
            Settings.Write();

            if (Settings.canConnectOnStartup())
            {
                try
                {
                    _twitchWrapper.StartAsyncInstance();
                }
                catch (System.Exception ex)
                {
                    ToolkitCoreLogger.Warning($"Failed to connect to Twitch on startup: {ex.Message}");
                }
            }
        }
        public void OnSettingsChanged()
        {
            Settings.Write();
            
        }
    }
}
