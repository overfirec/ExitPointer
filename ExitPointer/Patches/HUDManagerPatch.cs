using System;
using BepInEx.Logging;
using ExitPointer.Utils;
using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;


namespace ExitPointer.Patches;

[HarmonyPatch(typeof(HUDManager))]
public class HUDManagerPatch
{
    private static ExitPointerClass? _mainExitPointer;
    private static ExitPointerClass? _fireExitPointer;
    internal static Camera? PlayerCamera;
    internal static Tuple<Transform, Transform>? Entrances;
    internal static bool IsAlready;
        
    [HarmonyPatch("Start"), HarmonyPostfix]
    public static void StartPostfix(ref HUDManager __instance)
    {
        Tuple<Texture2D, Texture2D> pointersTextures = ExitPointerPluginRegister.PointerTextures;
        
        Tuple<GameObject, GameObject> pointersGameObjects = Tuple.Create(
            ResourcesLoader.SetGameObject("MainExitPointer", pointersTextures.Item1),
            ResourcesLoader.SetGameObject("FireExitPointer", pointersTextures.Item2));
        
        try
        {
            _mainExitPointer = new ExitPointerClass(pointersGameObjects.Item1);
            _fireExitPointer = new ExitPointerClass(pointersGameObjects.Item2);
        }
        catch (Exception e)
        {
            ExitPointerPluginRegister.Log($"Error while creating ExitPointers: {e.Message}", LogLevel.Error);
        }

        
        _mainExitPointer.SetActive(false);
        _fireExitPointer.SetActive(false);

        _mainExitPointer.SetDefaultStatus(((Component)__instance.loadingDarkenScreen).transform, new Vector3(0.28f, 0.3f, 1f));
        _fireExitPointer.SetDefaultStatus(((Component)__instance.loadingDarkenScreen).transform, new Vector3(0.28f, 0.3f, 1f));
    }

    [HarmonyPatch("Update"), HarmonyPostfix]
    public static void UpdatePostfix(ref HUDManager __instance)
    {
        if (GameNetworkManager.Instance.localPlayerController.isInsideFactory)
        {
            if (PlayerCamera == null || Entrances == null || Entrances.Item1 == null || Entrances.Item2 == null)
            {
                PlayerCamera = GameNetworkManager.Instance.localPlayerController.gameplayCamera;
                Entrances = Extensions.GetExitPosition();
                ExitPointerPluginRegister.Log("Try to get Entrances...");
            }
            
            _mainExitPointer?.SetActive(true);
            _fireExitPointer?.SetActive(true);
            
            _mainExitPointer.MovePointer(PlayerCamera, Entrances.Item1);
            _fireExitPointer.MovePointer(PlayerCamera, Entrances.Item2);
        }
        
        else
        {
            _mainExitPointer?.SetActive(false);
            _fireExitPointer?.SetActive(false);
        }
        
    }
}