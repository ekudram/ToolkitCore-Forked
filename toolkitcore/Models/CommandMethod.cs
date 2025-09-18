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

/* Old Code

using TwitchLib.Client.Interfaces;

namespace ToolkitCore.Models
{
    public class CommandMethod
    {
        public ToolkitChatCommand command;

        public CommandMethod(ToolkitChatCommand command) => this.command = command;

        public virtual bool CanExecute(ITwitchCommand twitchCommand) => this.command.enabled && (!this.command.requiresBroadcaster || twitchCommand.ChatMessage == null || twitchCommand.ChatMessage.IsBroadcaster) && (!this.command.requiresMod || twitchCommand.ChatMessage == null || twitchCommand.ChatMessage.IsBroadcaster || twitchCommand.ChatMessage.IsModerator);

        public virtual void Execute(ITwitchCommand twitchCommand)
        {
        }
    }
}
*/