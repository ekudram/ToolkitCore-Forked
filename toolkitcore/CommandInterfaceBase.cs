using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;      
using System.Threading.Tasks; // For general utilities
using System.Windows.Input; // For ICommand
using ToolkitCore.Interfaces; // For ICommand
using Verse;    // For GameComponent

namespace ToolkitCore
{
    public abstract class CommandInterfaceBase : GameComponent
    {
        public abstract void ParseCommand(ICommand command);
    }
}