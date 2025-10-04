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

namespace ToolkitCore.Windows
{
    public class Window_ModSettings : Window
    {
        public Window_ModSettings(Mod mod)
        {
            this.Mod = mod;
            this.doCloseButton = true;
        }

        public override void DoWindowContents(Rect inRect) => this.Mod.DoSettingsWindowContents(inRect);

        public override Vector2 InitialSize => new Vector2(900f, 700f);

        public Mod Mod { get; }

        public override void Close(bool doCloseSound = true)
        {
            ToolkitCoreLogger.Debug($"=== Called Window_ModSettings : Windows. ===");
            base.Close(doCloseSound);
            this.Mod.WriteSettings();
            ToolkitCoreLogger.Debug($"=== Mod Settings Saved. ===");
        }
    }
}
