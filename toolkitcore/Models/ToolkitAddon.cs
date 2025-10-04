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
 */

using System;
using ToolkitCore.Interfaces;
using Verse;

namespace ToolkitCore.Models
{
    public class ToolkitAddon : Def
    {
        public Type menuClass = typeof(IAddonMenu);

        public IAddonMenu GetAddonMenu() => Activator.CreateInstance(menuClass) as IAddonMenu;
    }
}
