using System;
using HarmonyLib;
using BepInEx;
using BepInEx.Logging;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using ExitPointer.Utils;
using ExitPointer.PluginInfo;

namespace ExitPointer
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class ExitPointerPluginRegister : BaseUnityPlugin
    {
        private readonly Harmony _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
        
        private static ManualLogSource? LOGSource{ get; set; }
        
        private static ExitPointerPluginRegister? _instance;

        internal static Tuple<Texture2D, Texture2D>? PointerTextures{ get; private set; }
        
        internal const string MainExitPointer = @"MainExitPointer";

        internal const string FireExitPointer = @"FireExitPointer";
        
        
        // Initialize plugin
        private void Awake()
        {
            _instance = this;
            LOGSource = BepInEx.Logging.Logger.CreateLogSource(MyPluginInfo.PLUGIN_GUID);
            _harmony.PatchAll();
            Log($"{MyPluginInfo.PLUGIN_GUID} version: {MyPluginInfo.PLUGIN_VERSION} loading...");
            TryLoadResource();
        }
        
        // Unload plugin
        private void OnDisable()
        {
            _harmony.UnpatchSelf();
            LOGSource?.Dispose();
            _instance = null;
        }

        // Log message in BepInEx terminal
        internal static void Log(string message, LogLevel level = LogLevel.Info)
        {
            if (LOGSource is null)
            {
                BepInEx.Logging.Logger.CreateLogSource(MyPluginInfo.PLUGIN_NAME).LogError("Log source not found!");
                return;
            }
            
            LOGSource.Log(level, message);
        }

        private void TryLoadResource()
        {
            try
            {
                // Load resources(Texture2D)
                Tuple<Texture2D, Texture2D> pointerT2D = new Tuple<Texture2D, Texture2D>(
                    ResourcesLoader.LoadResource(MainExitPointer),
                    ResourcesLoader.LoadResource(FireExitPointer));
                
                if (pointerT2D.Item1 is null || pointerT2D.Item2 is null)
                {
                    throw new NullReferenceException("Pointer texture can't be loaded!");
                }

                PointerTextures = pointerT2D;
                Log("Resources loaded successfully!");
            }
            
            catch (Exception e)
            {
                Log(e.ToString(), LogLevel.Error);
                OnDisable();
            }
        }
    }
}

