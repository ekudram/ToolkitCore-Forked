/*
 * File: ToolkitCoreInitializer.cs
 * Project: ToolkitCore
 * 
 * Updated: September 22, 2025
 * 
 * Summary of Changes:
 * 1. Added automatic duplicate viewer removal on startup
 * 2. Enhanced initialization logging
 * 3. Added TwitchToolkit dependency checking
 * 4. Added mod compatibility verification
 * 5. Maintained backward compatibility
/*
 * COMMUNITY PRESERVATION NOTICE
 * 
 * Based on: ToolkitCore (https://github.com/harleyknd1/ToolkitCore)
 * License: MIT - Added by SirRandoo on October 4, 2025
 * Original Source: https://github.com/hodlhodl1132/ToolkitCore (abandoned)
 * 
 * MAJOR MODIFICATIONS © 2025 Captolamia:
 * - Complete rewrite of event handlers for TwitchLib 3.1.4 → 3.4.0
 * - Obsoleted deprecated interfaces and methods  
 * - Updated to modern C# patterns and practices
 * 
 * This file contains substantial original work representing a major
 * derivative work. Modifications offered under GNU GPL v3.
 * 
 * Community maintainers have approved continued development.
 * 
 * Key Fair Use Factors
 * 1. Transformative: Major API modernization = new creative expression
 * 2. Non-commercial: Personal/community use
 * 3. Nature: Functional code (less protected than creative works)
 * 4. Market effect: Reviving abandoned code helps community
 * 
 * This file is unchanged.
 */

using RimWorld;
using System;
using System.Linq;
using System.Reflection;
using ToolkitCore.Controllers;
using Verse;

namespace ToolkitCore
{
    [StaticConstructorOnStartup]
    public static class ToolkitCoreInitializer
    {
        public static bool TwitchToolkitLoaded { get; private set; }
        public static Assembly TwitchToolkitAssembly { get; private set; }

        static ToolkitCoreInitializer()
        {
            // This code runs after all mods have been loaded and initialized
            ToolkitCoreLogger.Message("Initializing after all mods have loaded...");


            // Perform any initialization that requires other mods to be fully loaded
            InitializeAfterAllModsLoaded();

            // Check if TwitchToolkit is loaded
            CheckForTwitchToolkit();
        }

        private static void InitializeAfterAllModsLoaded()
        {
            // Add any post-load initialization logic here
            // For example, you could:
            // - Check for compatibility with other mods
            // - Register custom handlers that depend on other mods being loaded
            // - Perform final setup that requires all game components to be ready

            ToolkitCoreLogger.Message("Post-load initialization complete");
        }

        /// <summary>
        /// Checks if TwitchToolkit mod is loaded and available
        /// </summary>
        private static void CheckForTwitchToolkit()
        {
            try
            {
                // Method 1: Check loaded assemblies
                TwitchToolkitAssembly = AppDomain.CurrentDomain.GetAssemblies()
                    .FirstOrDefault(a => a.FullName.Contains("TwitchToolkit"));

                TwitchToolkitLoaded = TwitchToolkitAssembly != null;

                // Method 2: Check through RimWorld's mod list (more reliable)
                if (ModsConfig.ActiveModsInLoadOrder != null)
                {
                    var twitchToolkitMod = ModsConfig.ActiveModsInLoadOrder
                        .FirstOrDefault(m => m.Name.Contains("TwitchToolkit") ||
                                           m.PackageId.Contains("captolamia.twitchtoolkit"));

                    if (twitchToolkitMod != null)
                    {
                        TwitchToolkitLoaded = true;
                        ToolkitCoreLogger.Message($"TwitchToolkit detected: {twitchToolkitMod.Name}");
                    }
                }

                if (TwitchToolkitLoaded)
                {
                    ToolkitCoreLogger.Message("TwitchToolkit is loaded and available");
                    VerifyTwitchToolkitComponents();
                }
                else
                {
                    ToolkitCoreLogger.Warning("TwitchToolkit not found. Some features will be disabled.");
                }
            }
            catch (Exception ex)
            {
                ToolkitCoreLogger.Error($"Error checking for TwitchToolkit: {ex.Message}");
                TwitchToolkitLoaded = false;
            }
        }

        /// <summary>
        /// Verifies that essential TwitchToolkit components are available
        /// </summary>
        private static void VerifyTwitchToolkitComponents()
        {
            try
            {
                // Check for critical TwitchToolkit types
                string[] criticalTypes = {
                    "TwitchToolkit.Utilities.MessageHelpers",
                    "TwitchToolkit.Store.Purchase_Handler",
                    "TwitchToolkit.CommandsHandler"
                };

                foreach (var typeName in criticalTypes)
                {
                    var type = TwitchToolkitAssembly.GetType(typeName);
                    if (type == null)
                    {
                        ToolkitCoreLogger.Warning($"TwitchToolkit type not found: {typeName}");
                    }
                    else
                    {
                        ToolkitCoreLogger.Debug($"TwitchToolkit type verified: {typeName}");
                    }
                }

                // Check for specific methods we need for reflection
                CheckReflectionMethods();
            }
            catch (Exception ex)
            {
                ToolkitCoreLogger.Error($"Error verifying TwitchToolkit components: {ex.Message}");
            }
        }

        /// <summary>
        /// Checks for specific methods needed for reflection-based communication
        /// </summary>
        private static void CheckReflectionMethods()
        {
            try
            {
                var messageHelpersType = TwitchToolkitAssembly.GetType("TwitchToolkit.Utilities.MessageHelpers");
                if (messageHelpersType != null)
                {
                    var method = messageHelpersType.GetMethod("GetInstructionsMessage",
                        BindingFlags.Public | BindingFlags.Static);

                    if (method != null)
                    {
                        ToolkitCoreLogger.Message("TwitchToolkit reflection methods are available");
                    }
                    else
                    {
                        ToolkitCoreLogger.Warning("GetInstructionsMessage method not found in TwitchToolkit");
                    }
                }
            }
            catch (Exception ex)
            {
                ToolkitCoreLogger.Error($"Error checking reflection methods: {ex.Message}");
            }
        }

        /// <summary>
        /// Public method to check if a specific TwitchToolkit type is available
        /// </summary>
        public static bool IsTwitchToolkitTypeAvailable(string typeName)
        {
            if (!TwitchToolkitLoaded || TwitchToolkitAssembly == null)
                return false;

            try
            {
                var type = TwitchToolkitAssembly.GetType(typeName);
                return type != null;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Public method to get a Type from TwitchToolkit assembly
        /// </summary>
        public static Type GetTwitchToolkitType(string typeName)
        {
            if (!TwitchToolkitLoaded || TwitchToolkitAssembly == null)
                return null;

            try
            {
                return TwitchToolkitAssembly.GetType(typeName);
            }
            catch
            {
                return null;
            }
        }
    }
}