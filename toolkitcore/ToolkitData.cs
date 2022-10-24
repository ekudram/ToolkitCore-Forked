using ToolkitCore.Database;
using Verse;

namespace ToolkitCore
{
    public class ToolkitData : Mod
    {
        public static GlobalDatabase globalDatabase;

        public ToolkitData(ModContentPack content)
          : base(content)
        {
            ToolkitData.globalDatabase = this.GetSettings<GlobalDatabase>();
        }
    }
}
