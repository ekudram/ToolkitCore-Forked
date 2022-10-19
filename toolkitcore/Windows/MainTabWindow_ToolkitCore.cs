using RimWorld;
using ToolkitCore.Models;
using ToolkitCore.Utilities;
using UnityEngine;
using Verse;

namespace ToolkitCore.Windows
{
    public class MainTabWindow_ToolkitCore : MainTabWindow
    {
        public override void DoWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);

            listingStandard.Label("Toolkit Quick Menu");
            foreach (ToolkitAddon toolkitAddon in AddonRegistry.ToolkitAddons)
            {
                if (listingStandard.ButtonText(toolkitAddon.LabelCap))
                    Find.WindowStack.Add(new FloatMenu(toolkitAddon.GetAddonMenu().MenuOptions()));
            }

            listingStandard.End();
        }

        public override Vector2 RequestedTabSize => new Vector2(300f, (float)(100.0 + (double)AddonRegistry.ToolkitAddons.Count * 32.0));

        public override MainTabWindowAnchor Anchor => (MainTabWindowAnchor)1;
    }
}
