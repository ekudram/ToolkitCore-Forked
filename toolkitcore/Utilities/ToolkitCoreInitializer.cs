/*
 * File: ToolkitCoreInitializer.cs
 * Project: ToolkitCore
 * 
 * Updated: October 26, 2023
 * 
 * Summary:
 * Handles static initialization that runs after all mods have been loaded.
 * Uses RimWorld's StaticConstructorOnStartup attribute for proper initialization timing.
 */

using RimWorld;
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

            Log.Message("[ToolkitCore] Post-load initialization complete");
        }
    }
}