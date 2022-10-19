// Decompiled with JetBrains decompiler
// Type: ToolkitCore.ToolkitData
// Assembly: ToolkitCore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: CCE5A9F3-9986-4CDC-8194-AAC71A727132
// Assembly location: C:\Users\Kirito\Downloads\steamcmd\steamapps\workshop\content\294100\2018368654\Assemblies\ToolkitCore.dll

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
