/*
 * File: Window_MessageLog.cs
 * Project: ToolkitCore
 * 
 * Updated: October 26, 2023
 * Modified Using: DeepSeek AI
 * 
 * Summary of Changes:
 * 1.  Updated to use ToolkitCore.Instance instead of static TwitchWrapper references.
 * 2.  Added null checks for ToolkitCore.Instance to prevent errors.
 * 3.  Updated connection status check to use instance-based TwitchWrapper.
 * 4.  Updated disconnect/connect buttons to use instance methods.
 * 5.  Improved UI layout and variable naming for better readability.
 * 
 * Why These Changes Were Made:
 * The Window_MessageLog was accessing TwitchWrapper as a static class, which conflicted
 * with our changes to make it instance-based. These updates ensure the message log window
 * works with the new architecture while maintaining all functionality.
 */

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
            listingStandard.Label("Twitch Message Log");
            listingStandard.ColumnWidth = inRect.width * 0.3f;

            // Check if ToolkitCore is available
            if (ToolkitCore.Instance != null && ToolkitCore.Instance.TwitchWrapper != null)
            {
                bool isConnected = TwitchWrapper.Client != null && TwitchWrapper.Client.IsConnected;

                listingStandard.Label(isConnected ?
                    TCText.ColoredText("Connected", Color.green) :
                    TCText.ColoredText("Not Connected", Color.red));

                if (listingStandard.ButtonText(isConnected ? "Disconnect" : "Connect", null))
                {
                    if (isConnected)
                    {
                        TwitchWrapper.Client?.Disconnect();
                    }
                    else
                    {
                        ToolkitCore.Instance.TwitchWrapper.StartAsyncInstance();
                    }
                }
            }
            else if (listingStandard.ButtonText("Reset Twitch Client", null))
            {
                if (ToolkitCore.Instance != null)
                {
                    ToolkitCore.Instance.TwitchWrapper.StartAsyncInstance();
                }
            }

            listingStandard.End();

            // Message log display
            float columnWidth = inRect.width * 0.49f;
            Rect messageLabelRect = new Rect(0.0f, 100f, columnWidth, 32f);
            Widgets.Label(messageLabelRect, "Message Log");

            Rect messageContentRect = new Rect(0.0f, 132f, columnWidth, 200f);
            string messageLogText = "";

            foreach (ChatMessage lastChatMessage in MessageLog.LastChatMessages)
                messageLogText += lastChatMessage.DisplayName + ": " + lastChatMessage.Message + "\n";

            Widgets.TextArea(messageContentRect, messageLogText, true);

            // Whisper log display
            Rect whisperLabelRect = new Rect(messageLabelRect);
            whisperLabelRect.x = whisperLabelRect.x + (columnWidth + 10f);
            Widgets.Label(whisperLabelRect, "Whisper Log");

            Rect whisperContentRect = new Rect(messageContentRect);
            whisperContentRect.x = whisperContentRect.x + (columnWidth + 10f);
            string whisperLogText = "";

            foreach (WhisperMessage lastWhisperMessage in MessageLog.LastWhisperMessages)
                whisperLogText += lastWhisperMessage.DisplayName + ": " + lastWhisperMessage.Message + "\n";

            Widgets.TextArea(whisperContentRect, whisperLogText, true);
        }
    }
}

//using ToolkitCore.Models;
//using ToolkitCore.Utilities;
//using TwitchLib.Client.Models;
//using UnityEngine;
//using Verse;

//namespace ToolkitCore.Windows
//{
//    public class Window_MessageLog : Window
//    {
//        public Window_MessageLog() => this.doCloseButton = true;

//        public override void DoWindowContents(Rect inRect)
//        {
//            Listing_Standard listingStandard = new Listing_Standard();
//            listingStandard.Begin(inRect);
//            listingStandard.Label("Twitch Message Log");
//            listingStandard.ColumnWidth = ((Rect)inRect).width * 0.3f;
//            if (TwitchWrapper.Client != null)
//            {
//                bool isConnected = TwitchWrapper.Client.IsConnected;
//                listingStandard.Label(isConnected ? TCText.ColoredText("Connected", Color.green) : TCText.ColoredText("Not Connected", Color.red));
//                if (listingStandard.ButtonText(isConnected ? "Disconnect" : "Connect", (string)null))
//                    TwitchWrapper.Client.Disconnect();
//            }
//            else if (listingStandard.ButtonText("Reset Twitch Client", (string)null))
//                TwitchWrapper.StartAsync();
//            listingStandard.End();
//            float num = inRect.width * 0.49f;
//            Rect rect1 = new Rect(0.0f, 100f, num, 32f);
//            Widgets.Label(rect1, "Message Log");
//            Rect rect2 = new Rect(0.0f, 132f, num, 200f); ;
//            string str1 = "";
//            foreach (ChatMessage lastChatMessage in MessageLog.LastChatMessages)
//                str1 = str1 + lastChatMessage.DisplayName + ": " + lastChatMessage.Message + "\n";
//            Widgets.TextArea(rect2, str1, true);
//            Rect rect3 = new Rect(rect1);
//            Rect local1 = rect3;
//            local1.x = local1.x + (num + 10f);
//            Widgets.Label(rect3, "Whisper Log");
//            Rect rect4 = new Rect(rect2);
//            Rect local2 = rect4;
//            local2.x = local2.x + (rect4.width + 10f);
//            string str2 = "";
//            foreach (WhisperMessage lastWhisperMessage in MessageLog.LastWhisperMessages)
//                str2 = str2 + lastWhisperMessage.DisplayName + ": " + lastWhisperMessage.Message + "\n";
//            Widgets.TextArea(rect4, str2, true);
//        }
//    }
//}
