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
