/*
 * File: ToolkitCoreLogger.cs
 * Project: ToolkitCore 
 * 
 * Created: September 19, 2025
 * 
 * Helper class to standardize logging with color-coded messages
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
 * THIS FILE IS GNU GPL v3 et al.
 * ©Copyright 2025 Captolamia
 * 
 * Key Fair Use Factors
 * 1. Transformative: Major API modernization = new creative expression
 * 2. Non-commercial: Personal/community use
 * 3. Nature: Functional code (less protected than creative works)
 * 4. Market effect: Reviving abandoned code helps community
 * 
 */


using Verse;

namespace ToolkitCore
{
    public static class ToolkitCoreLogger
    {
        private const string Prefix = "<color=#B1A2CA>[ToolkitCore]</color>";

        public static void Log(string message)
        {
            Verse.Log.Message($"{Prefix} {message}");
        }

        public static void Warning(string message)
        {
            Verse.Log.Warning($"{Prefix} <color=#FFFF00>{message}</color>");
        }

        public static void Error(string message)
        {
            Verse.Log.Error($"{Prefix} <color=#FF0000>{message}</color>");
        }

        public static void Message(string message)
        {
            Verse.Log.Message($"{Prefix} <color=#00FF00>{message}</color>");
        }

        public static void Debug(string message)
        {
            // Precompiler directive for development builds
#if DEBUG
            Verse.Log.Message($"{Prefix} <color=#888888>[DEBUG] {message}</color>");
#endif

            // Runtime toggle for debug logging in any build
            if (ToolkitCoreSettings.enableDebugLogging)
                Verse.Log.Message($"{Prefix} <color=#888888>[DEBUG] {message}</color>");
        }
    }
}