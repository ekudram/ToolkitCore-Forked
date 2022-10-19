using UnityEngine;

namespace ToolkitCore.Utilities
{
    public static class TCText
    {
        public static string BigText(string str) => "<size=32>" + str + "</size>";

        public static string ColoredText(string str, Color color) => "<color=#" + ColorUtility.ToHtmlStringRGB(color) + ">" + str + "</color>";
    }
}
