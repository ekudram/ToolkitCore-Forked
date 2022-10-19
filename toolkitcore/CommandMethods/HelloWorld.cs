// Decompiled with JetBrains decompiler
// Type: ToolkitCore.CommandMethods.HelloWorld
// Assembly: ToolkitCore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: CCE5A9F3-9986-4CDC-8194-AAC71A727132
// Assembly location: C:\Users\Kirito\Downloads\steamcmd\steamapps\workshop\content\294100\2018368654\Assemblies\ToolkitCore.dll

using ToolkitCore.Models;
using TwitchLib.Client.Interfaces;

namespace ToolkitCore.CommandMethods
{
    public class HelloWorld : CommandMethod
    {
        public HelloWorld(ToolkitChatCommand command)
          : base(command)
        {
        }

        public override bool CanExecute(ITwitchCommand twitchCommand) => base.CanExecute(twitchCommand);

        public override void Execute(ITwitchCommand twitchCommand) => TwitchWrapper.SendChatMessage("Hello World!");
    }
}
