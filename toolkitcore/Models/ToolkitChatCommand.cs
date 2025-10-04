/*
 * File: ToolkitChatCommand.cs
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


using System;
using TwitchLib.Client.Models;
using Verse;

namespace ToolkitCore.Models
{
    public class ToolkitChatCommand : Def, IExposable
    {
        public string commandText;
        public bool enabled;
        public Type commandClass;
        public bool requiresMod;
        public bool requiresBroadcaster;

        public bool TryExecute(ChatMessage twitchCommand)
        {
            try
            {
                CommandMethod instance = (CommandMethod)Activator.CreateInstance(this.commandClass, (object)this);
                if (!instance.CanExecute(twitchCommand))
                    return false;
                instance.Execute(twitchCommand);
            }
            catch (Exception ex)
            {
                ToolkitCoreLogger.Error(ex.Message);
            }
            return true;
        }

        public void ExposeData()
        {
            Scribe_Values.Look<string>(ref this.commandText, "commandText", "helloworld", false);
            Scribe_Values.Look<bool>(ref this.enabled, "enabled", true, false);
            Scribe_Values.Look<Type>(ref this.commandClass, "commandClass", typeof(CommandMethod), false);
            Scribe_Values.Look<bool>(ref this.requiresMod, "requiresMod", false, false);
            Scribe_Values.Look<bool>(ref this.requiresBroadcaster, "requiresBroadcaster", false, false);
        }
    }
}
