using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ToolkitCore.Controllers;
using ToolkitCore.Interfaces;
using ToolkitCore.Models;
using Verse;

namespace ToolkitCore.Utilities
{
    public class CommandInterface : CommandInterfaceBase
    {
        public CommandInterface(Game game)
        {

        }

        public override void ParseCommand(ICommand command)
        {
            //throw new NotImplementedException();
        }
    }
}