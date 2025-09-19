/*
 * File: ToolkitCoreLogger.cs
 * Project: ToolkitCore 
 * 
 * Created: September 19, 2025
 * 
 * Helper class to standardize logging with color-coded messages
 */

using Verse;

namespace ToolkitCore
{
    public static class ToolkitCoreLogger
    {
        private const string Prefix = "<color=#B1A2CA>[ToolkitCore]</color>";

        public static void Log(string message)
        {
            Verse.Log.Message($"{Prefix} {message}");
        }

        public static void Warning(string message)
        {
            Verse.Log.Warning($"{Prefix} <color=#FFFF00>{message}</color>");
        }

        public static void Error(string message)
        {
            Verse.Log.Error($"{Prefix} <color=#FF0000>{message}</color>");
        }

        public static void Message(string message)
        {
            Verse.Log.Message($"{Prefix} <color=#00FF00>{message}</color>");
        }

        public static void Debug(string message)
        {
            // Precompiler directive for development builds
#if DEBUG
            Verse.Log.Message($"{Prefix} <color=#888888>[DEBUG] {message}</color>");
#endif

            // Runtime toggle for debug logging in any build
            if (ToolkitCoreSettings.enableDebugLogging)
                Verse.Log.Message($"{Prefix} <color=#888888>[DEBUG] {message}</color>");
        }
    }
}