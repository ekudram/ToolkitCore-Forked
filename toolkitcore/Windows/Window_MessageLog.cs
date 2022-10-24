using ToolkitCore.Models;
using ToolkitCore.Utilities;
using TwitchLib.Client.Models;
using UnityEngine;
using Verse;

namespace ToolkitCore.Windows
{
    public class Window_MessageLog : Window
    {
        public Window_MessageLog() => this.doCloseButton = true;

        public override void DoWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);
            listingStandard.Label("Twitch Message Log", -1f, (string)null);
            listingStandard.ColumnWidth = ((Rect)inRect).width * 0.3f;
            if (TwitchWrapper.Client != null)
            {
                bool isConnected = TwitchWrapper.Client.IsConnected;
                listingStandard.Label(isConnected ? TCText.ColoredText("Connected", Color.green) : TCText.ColoredText("Not Connected", Color.red), -1f, (string)null);
                if (listingStandard.ButtonText(isConnected ? "Disconnect" : "Connect", (string)null))
                    TwitchWrapper.Client.Disconnect();
            }
            else if (listingStandard.ButtonText("Reset Twitch Client", (string)null))
                TwitchWrapper.StartAsync();
            listingStandard.End();
            float num = inRect.width * 0.49f;
            Rect rect1 = new Rect(0.0f, 100f, num, 32f);
            Widgets.Label(rect1, "Message Log");
            Rect rect2 = new Rect(0.0f, 132f, num, 200f); ;
            string str1 = "";
            foreach (ChatMessage lastChatMessage in MessageLog.LastChatMessages)
                str1 = str1 + lastChatMessage.DisplayName + ": " + lastChatMessage.Message + "\n";
            Widgets.TextArea(rect2, str1, true);
            Rect rect3 = new Rect(rect1);
            Rect local1 = rect3;
            local1.x = local1.x + (num + 10f);
            Widgets.Label(rect3, "Whisper Log");
            Rect rect4 = new Rect(rect2);
            Rect local2 = rect4;
            local2.x = local2.x + (rect4.width + 10f);
            string str2 = "";
            foreach (WhisperMessage lastWhisperMessage in MessageLog.LastWhisperMessages)
                str2 = str2 + lastWhisperMessage.DisplayName + ": " + lastWhisperMessage.Message + "\n";
            Widgets.TextArea(rect4, str2, true);
        }
    }
}
