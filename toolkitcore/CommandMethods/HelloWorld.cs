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
using TwitchLib.Client.Models; // For ChatMessage
using System.Collections.Generic;
// Remove the using for TwitchLib.Client.Interfaces

namespace ToolkitCore.CommandMethods
{
    public class HelloWorld : CommandMethod
    {
        public HelloWorld(ToolkitChatCommand command) : base(command)
        {
        }

        // Refactored CanExecute to take a ChatMessage
        public override bool CanExecute(ChatMessage chatMessage)
        {
            // Implement your permission/validation logic here using chatMessage
            // e.g., check chatMessage.IsModerator, chatMessage.IsSubscriber, etc.
            return base.CanExecute(chatMessage); // You'll need to adjust the base method too
        }

        // Refactored Execute to take a ChatMessage
        public override void Execute(ChatMessage chatMessage)
        {
            if (ToolkitCore.Instance?.TwitchWrapper != null)
            {
                ToolkitCore.Instance.TwitchWrapper.SendChatMessageInternal("Hello World!");
            }
        }
    }
}
