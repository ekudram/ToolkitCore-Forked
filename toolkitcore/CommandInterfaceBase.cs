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


using System.Windows.Input; // For ICommand
using Verse;    // For GameComponent

namespace ToolkitCore
{
    public abstract class CommandInterfaceBase : GameComponent
    {
        public abstract void ParseCommand(ICommand command);
    }
}