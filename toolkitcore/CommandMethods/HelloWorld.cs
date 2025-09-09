/*
 * File: HelloWorld.cs
 * Project: ToolkitCore
 * 
 * Updated: October 26, 2023
 * Modified Using: DeepSeek AI
 * 
 * Summary of Changes:
 * 1.  Updated to use ToolkitCore.Instance instead of static TwitchWrapper reference.
 * 2.  Added null check for ToolkitCore.Instance to prevent errors.
 * 
 * Why These Changes Were Made:
 * The HelloWorld command was accessing TwitchWrapper.SendChatMessage as a static method,
 * which conflicted with our changes to make TwitchWrapper instance-based. This update
 * ensures the command works with the new architecture.
 */

using ToolkitCore.Models;
using TwitchLib.Client.Interfaces;

namespace ToolkitCore.CommandMethods
{
    public class HelloWorld : CommandMethod
    {
        public HelloWorld(ToolkitChatCommand command) : base(command)
        {
        }

        public override bool CanExecute(ITwitchCommand twitchCommand) => base.CanExecute(twitchCommand);

        public override void Execute(ITwitchCommand twitchCommand)
        {
            if (ToolkitCore.Instance != null && ToolkitCore.Instance.TwitchWrapper != null)
            {
                ToolkitCore.Instance.TwitchWrapper.SendChatMessage("Hello World!");
            }
        }
    }
}
//using ToolkitCore.Models;
//using TwitchLib.Client.Interfaces;

//namespace ToolkitCore.CommandMethods
//{
//    public class HelloWorld : CommandMethod
//    {
//        public HelloWorld(ToolkitChatCommand command)
//          : base(command)
//        {
//        }

//        public override bool CanExecute(ITwitchCommand twitchCommand) => base.CanExecute(twitchCommand);

//        public override void Execute(ITwitchCommand twitchCommand) => TwitchWrapper.SendChatMessage("Hello World!");
//    }
//}
