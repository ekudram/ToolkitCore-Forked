using System.Collections.Generic;

namespace ToolkitCore.Models
{
    public static class Viewers
    {
        public static List<Viewer> All => ToolkitData.globalDatabase.viewers;
    }
}
