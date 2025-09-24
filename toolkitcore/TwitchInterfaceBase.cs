/*
 * File: TwitchInterfaceBase.cs
 * Project: ToolkitCore
 * 
 * Updated: October 26, 2023
 * Modified Using: DeepSeek AI
 * 
 * Summary of Changes:
 * 1. Removed incorrect constructor that tried to pass Game parameter to base.
 * 2. Added default constructor required for GameComponent.
 * 
 * Why These Changes Were Made:
 * GameComponent in RimWorld does not have a constructor that accepts parameters.
 * The base GameComponent class only has a default constructor, so we must use that.
 */

using TwitchLib.Client.Models;
using Verse;

namespace ToolkitCore
{
    public abstract class TwitchInterfaceBase : GameComponent
    {
        // GameComponent requires a default constructor
        public TwitchInterfaceBase()
        {
        }

        public TwitchInterfaceBase(Game game) : base()
        {
            // This empty constructor with game parameter might be needed
        }

        public abstract void ParseMessage(ChatMessage chatMessage);
        // public abstract void ParseMessage(TwitchMessageWrapper twitchMessageWrapper);
        // Method for handling whisper messages
        public abstract void ParseWhisper(WhisperMessage whisperMessage);

        // Optional: Method for handling whisper commands
        public virtual void ParseWhisperCommand(WhisperCommand whisperCommand)
        {
            // Default implementation can be empty
            // Individual interfaces can override if they need to handle whisper commands
        }

    }
}

//using TwitchLib.Client.Models.Interfaces;
//using Verse;

//namespace ToolkitCore
//{
//    public abstract class TwitchInterfaceBase : GameComponent
//    {
//        public abstract void ParseMessage(ITwitchMessage twitchMessage);
//    }
//}
