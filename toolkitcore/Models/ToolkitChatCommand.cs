using System;
using TwitchLib.Client.Interfaces;
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
                Log.Error(ex.Message);
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
