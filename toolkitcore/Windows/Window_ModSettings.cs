using UnityEngine;
using Verse;

namespace ToolkitCore.Windows
{
    public class Window_ModSettings : Window
    {
        public Window_ModSettings(Mod mod)
        {
            this.Mod = mod;
            this.doCloseButton = true;
        }

        public override void DoWindowContents(Rect inRect) => this.Mod.DoSettingsWindowContents(inRect);

        public override Vector2 InitialSize => new Vector2(900f, 700f);

        public override void PostClose() => this.Mod.WriteSettings();

        public Mod Mod { get; }
    }
}
