using ToolkitCore.Models;
using TwitchLib.Client.Interfaces;

namespace ToolkitCore.CommandMethods
{
    public class HelloWorld : CommandMethod
    {
        public HelloWorld(ToolkitChatCommand command)
          : base(command)
        {
        }

        public override bool CanExecute(ITwitchCommand twitchCommand) => base.CanExecute(twitchCommand);

        public override void Execute(ITwitchCommand twitchCommand) => TwitchWrapper.SendChatMessage("Hello World!");
    }
}
