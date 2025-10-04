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

using TwitchLib.Client.Models;

namespace ToolkitCore.Models
{
    public class CommandMethod
    {
        public ToolkitChatCommand command;

        public CommandMethod(ToolkitChatCommand command) => this.command = command;

        // Updated to use ChatMessage instead of ITwitchCommand
        public virtual bool CanExecute(ChatMessage chatMessage) =>
            this.command.enabled &&
            (!this.command.requiresBroadcaster || chatMessage == null || chatMessage.IsBroadcaster) &&
            (!this.command.requiresMod || chatMessage == null || chatMessage.IsBroadcaster || chatMessage.IsModerator);

        // Updated to use ChatMessage
        public virtual void Execute(ChatMessage chatMessage)
        {
            // Base implementation (can be empty)
        }
    }
}
