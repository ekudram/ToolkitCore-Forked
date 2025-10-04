/*
 * File: ChatCommandController.cs
 * Project: ToolkitCore
 * 
 * Updated: September 13, 2025
 * Modified Using: DeepSeek AI
 * 
 * Summary of Changes:
 * 1. Instance-Based Pattern: The core logic is now in an instance method that only loads into memory when needed.
 * 2. Backward Compatibility: The static GetChatCommand method is preserved exactly as it was, ensuring that ToolkitUtilities and other mods that might override this method won't break.
 * 3. Lazy Initialization: The static Instance property uses lazy initialization - the instance is only created when first accessed.
 * 4. Error Handling: Added proper exception handling to catch and log any errors that might occur during command parsing. 
 * 5. Virtual Method: The instance method is marked as virtual, allowing other mods to inherit from this class and override the behavior if needed.
 * 6. Cleaner Code: Improved readability with better formatting and null checking.
 * 
 *  * Why These Changes Were Made:
 * The HelloWorld command was accessing TwitchWrapper.SendChatMessage as a static method,
 * which conflicted with our changes to make TwitchWrapper instance-based. This update
 * ensures the command works with the new architecture.
 * 
 *
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
using System.Linq;
using ToolkitCore.Models;
using ToolkitCore.Utilities;
using Verse;

namespace ToolkitCore.Controllers
{
    public class ChatCommandController
    {
        private static ChatCommandController _instance;

        // Static property for backward compatibility
        public static ChatCommandController Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ChatCommandController();
                }
                return _instance;
            }
        }

        // Instance method - the actual implementation
        public virtual ToolkitChatCommand GetChatCommandInternal(string commandText)
        {
            try
            {
                string baseCommand = CommandFilter.Parse(commandText).FirstOrDefault();

                if (string.IsNullOrEmpty(baseCommand))
                    return null;

                return DefDatabase<ToolkitChatCommand>.AllDefsListForReading
                    .FirstOrDefault(c => GenText.EqualsIgnoreCase(c.commandText, baseCommand));
            }
            catch (Exception ex)
            {
                ToolkitCoreLogger.Error($"Error in GetChatCommand: {ex.Message}");
                return null;
            }
        }

        // Static method for backward compatibility with ToolkitUtilities and other mods
        public static ToolkitChatCommand GetChatCommand(string commandText)
        {
            return Instance.GetChatCommandInternal(commandText);
        }
    }
}

