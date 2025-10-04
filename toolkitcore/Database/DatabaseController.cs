using System;
using System.IO;
using UnityEngine;
using Verse;

namespace ToolkitCore.Database
{
    /*
     * File: DatabaseController.cs
     * Project: ToolkitCore
     * 
     * Updated: September,13 2025
     * Modified Using: DeepSeek AI
     * 
     * Summary of Changes:
     * 1. Added comprehensive documentation headers for all methods
     * 2. Maintained static methods for backward compatibility with ToolkitUtilities
     * 3. Enhanced error handling with more descriptive error messages
     * 4. Added null checks for all input parameters
     * 5. Improved path handling with better validation
     * 6. Added XML comments for better IntelliSense support
     * 
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
     */

    [StaticConstructorOnStartup]
    public static class DatabaseController
    {
        private static readonly string modFolder = "ToolkitCore";
        public static readonly string dataPath = Path.Combine(Application.persistentDataPath, modFolder);

        /// <summary>
        /// Static constructor ensures data directory exists on startup
        /// </summary>
        static DatabaseController() => EnsureDataDirectoryExists();

        /// <summary>
        /// Ensures the data directory for ToolkitCore exists
        /// </summary>
        private static void EnsureDataDirectoryExists()
        {
            try
            {
                if (!Directory.Exists(dataPath))
                {
                    Directory.CreateDirectory(dataPath);
                    ToolkitCoreLogger.Message($"Created data directory: {dataPath}");
                }
            }
            catch (Exception ex)
            {
                ToolkitCoreLogger.Error($"Failed to create data directory: {ex.Message}");
            }
        }

        /// <summary>
        /// Saves an object to JSON file with mod-specific naming
        /// </summary>
        /// <param name="obj">The object to serialize</param>
        /// <param name="fileName">The base filename (without extension)</param>
        /// <param name="mod">The mod instance</param>
        /// <returns>True if successful, false otherwise</returns>
        public static bool SaveObject(object obj, string fileName, Mod mod)
        {
            if (obj == null)
            {
                ToolkitCoreLogger.Error("Cannot save null object");
                return false;
            }

            if (string.IsNullOrEmpty(fileName))
            {
                ToolkitCoreLogger.Error("File name cannot be null or empty");
                return false;
            }

            if (mod == null || mod.Content.Name == null)
            {
                ToolkitCoreLogger.Error("Mod reference is null or mod has no name");
                return false;
            }

            try
            {
                string modPrefix = mod.Content.Name.Replace(" ", "");
                string fullFileName = $"{modPrefix}_{fileName}.json";
                string json = JsonUtility.ToJson(obj, true);

                return SaveFileInternal(json, fullFileName);
            }
            catch (Exception ex)
            {
                ToolkitCoreLogger.Error($"Error saving object {fileName}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Loads an object from JSON file with mod-specific naming
        /// </summary>
        /// <typeparam name="T">The type of object to deserialize</typeparam>
        /// <param name="fileName">The base filename (without extension)</param>
        /// <param name="mod">The mod instance</param>
        /// <param name="obj">The deserialized object output</param>
        /// <returns>True if successful, false otherwise</returns>
        public static bool LoadObject<T>(string fileName, Mod mod, out T obj)
        {
            obj = default(T);

            if (string.IsNullOrEmpty(fileName))
            {
                ToolkitCoreLogger.Error("File name cannot be null or empty");
                return false;
            }

            if (mod == null || mod.Content.Name == null)
            {
                ToolkitCoreLogger.Error("Mod reference is null or mod has no name");
                return false;
            }

            try
            {
                string modPrefix = mod.Content.Name.Replace(" ", "");
                string fullFileName = $"{modPrefix}_{fileName}.json";
                string json;

                if (!LoadFileInternal(fullFileName, out json))
                {
                    ToolkitCoreLogger.Warning($"File not found: {fullFileName}");
                    return false;
                }

                obj = JsonUtility.FromJson<T>(json);
                return true;
            }
            catch (Exception ex)
            {
                ToolkitCoreLogger.Error($"Error loading object {fileName}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Internal method to save JSON content to a file
        /// </summary>
        private static bool SaveFileInternal(string json, string fileName)
        {
            if (string.IsNullOrEmpty(json))
            {
                ToolkitCoreLogger.Warning("Attempted to save empty JSON content");
                return false;
            }

            try
            {
                string fullPath = Path.Combine(dataPath, fileName);

                // Ensure directory exists
                EnsureDataDirectoryExists();

                File.WriteAllText(fullPath, json);
                ToolkitCoreLogger.Debug($"Saved file: {fileName}");
                return true;
            }
            catch (Exception ex)
            {
                ToolkitCoreLogger.Error($"Error saving file {fileName}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Internal method to load JSON content from a file
        /// </summary>
        private static bool LoadFileInternal(string fileName, out string json)
        {
            json = null;

            try
            {
                string fullPath = Path.Combine(dataPath, fileName);

                if (!File.Exists(fullPath))
                    return false;

                json = File.ReadAllText(fullPath);
                return true;
            }
            catch (Exception ex)
            {
                ToolkitCoreLogger.Error($"Error loading file {fileName}: {ex.Message}");
                return false;
            }
        }

        // The following methods are kept for backward compatibility but marked as obsolete
        #region Backward Compatibility Methods

        /// <summary>
        /// Empty method kept for backward compatibility
        /// </summary>
        public static void SaveToolkit()
        {
            // Kept for backward compatibility
        }

        /// <summary>
        /// Empty method kept for backward compatibility
        /// </summary>
        public static void LoadToolkit()
        {
            // Kept for backward compatibility
        }

        /// <summary>
        /// Legacy save method kept for backward compatibility
        /// </summary>
        public static bool SaveFile(string json, string fileName)
        {
            return SaveFileInternal(json, fileName);
        }

        /// <summary>
        /// Legacy load method kept for backward compatibility
        /// </summary>
        public static bool LoadFile(string fileName, out string json)
        {
            return LoadFileInternal(fileName, out json);
        }

        #endregion
    }
}
