using System;
using ToolkitCore.Interfaces;
using Verse;

namespace ToolkitCore.Models
{
    public class ToolkitAddon : Def
    {
        public Type menuClass = typeof(IAddonMenu);

        public IAddonMenu GetAddonMenu() => Activator.CreateInstance(this.menuClass) as IAddonMenu;
    }
}
