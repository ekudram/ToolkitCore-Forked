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
                Log.Error($"Error in GetChatCommand: {ex.Message}");
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

//using System;
//using System.Linq;
//using ToolkitCore.Models;
//using ToolkitCore.Utilities;
//using Verse;

//namespace ToolkitCore.Controllers
//{
//    public static class ChatCommandController
//    {
//        public static ToolkitChatCommand GetChatCommand(string commandText)
//        {
//            string baseCommand = CommandFilter.Parse(commandText).FirstOrDefault<string>();
//            return baseCommand == null ? (ToolkitChatCommand)null : DefDatabase<ToolkitChatCommand>.AllDefsListForReading.FirstOrDefault<ToolkitChatCommand>((Func<ToolkitChatCommand, bool>)(c => GenText.EqualsIgnoreCase(c.commandText, baseCommand)));
//        }
//    }
//}
