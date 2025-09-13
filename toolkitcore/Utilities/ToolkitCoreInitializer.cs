/*
 * File: ToolkitCoreInitializer.cs
 * Project: ToolkitCore
 * 
 * Updated: October 26, 2023
 * Modified Using: DeepSeek AI
 * 
 * Summary of Changes:
 * 1. Added automatic duplicate viewer removal on startup
 * 2. Enhanced initialization logging
 * 3. Maintained backward compatibility
 */

using RimWorld;
using ToolkitCore.Controllers;
using Verse;

namespace ToolkitCore
{
    [StaticConstructorOnStartup]
    public static class ToolkitCoreInitializer
    {
        static ToolkitCoreInitializer()
        {
            // This code runs after all mods have been loaded and initialized
            Log.Message("[ToolkitCore] Initializing after all mods have loaded...");

            // Perform any initialization that requires other mods to be fully loaded
            InitializeAfterAllModsLoaded();
        }

        private static void InitializeAfterAllModsLoaded()
        {
            // Add any post-load initialization logic here
            // For example, you could:
            // - Check for compatibility with other mods
            // - Register custom handlers that depend on other mods being loaded
            // - Perform final setup that requires all game components to be ready

            // Remove duplicate viewers on startup
            int removedCount = ViewerController.RemoveDuplicateViewers();
            if (removedCount > 0)
            {
                Log.Message($"[ToolkitCore] Removed {removedCount} duplicate viewers on startup");
            }

            Log.Message("[ToolkitCore] Post-load initialization complete");
        }
    }
}