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
