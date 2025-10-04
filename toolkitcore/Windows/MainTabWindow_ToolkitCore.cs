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

using RimWorld;
using ToolkitCore.Models;
using ToolkitCore.Utilities;
using UnityEngine;
using Verse;

namespace ToolkitCore.Windows
{
    public class MainTabWindow_ToolkitCore : MainTabWindow
    {
        public override void DoWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);

            listingStandard.Label("Toolkit Quick Menu");
            foreach (ToolkitAddon toolkitAddon in AddonRegistry.ToolkitAddons)
            {
                if (listingStandard.ButtonText(toolkitAddon.LabelCap))
                    Find.WindowStack.Add(new FloatMenu(toolkitAddon.GetAddonMenu().MenuOptions()));
            }

            listingStandard.End();
        }

        public override Vector2 RequestedTabSize => new Vector2(300f, (float)(100.0 + (double)AddonRegistry.ToolkitAddons.Count * 32.0));

        public override MainTabWindowAnchor Anchor => (MainTabWindowAnchor)1;
    }
}
