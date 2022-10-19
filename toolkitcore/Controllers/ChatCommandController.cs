using System;
using System.Linq;
using ToolkitCore.Models;
using ToolkitCore.Utilities;
using Verse;

namespace ToolkitCore.Controllers
{
    public static class ChatCommandController
    {
        public static ToolkitChatCommand GetChatCommand(string commandText)
        {
            string baseCommand = CommandFilter.Parse(commandText).FirstOrDefault<string>();
            return baseCommand == null ? (ToolkitChatCommand)null : DefDatabase<ToolkitChatCommand>.AllDefsListForReading.FirstOrDefault<ToolkitChatCommand>((Func<ToolkitChatCommand, bool>)(c => GenText.EqualsIgnoreCase(c.commandText, baseCommand)));
        }
    }
}
