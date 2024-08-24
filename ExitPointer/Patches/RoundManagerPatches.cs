using System;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;


namespace ExitPointer.Patches;

[HarmonyPatch(typeof(RoundManager))]
public class RoundManagerPatches
{
    internal static Camera? PlayerCamera;

    internal static Tuple<Transform, Transform>? Entrances;
    
    internal static bool IsAlready;
    
    [HarmonyPatch("SpawnMapObjects"), HarmonyPostfix]
    public static void SpawnMapObjectsPatches()
    {
        {
            if (GameNetworkManager.Instance.gameHasStarted)
            {
                PlayerCamera = StartOfRound.Instance.localPlayerController.gameplayCamera;
                Entrances = Utils.Extensions.GetExitPosition();
            }

            if (PlayerCamera != null && Entrances != null)
            {
                IsAlready = true;
                return;
            }
        
            if (PlayerCamera == null)
            {
                ExitPointerPluginRegister.Log("Player camera is null!", LogLevel.Error);
            }
        
            if (Entrances == null)
            {
                ExitPointerPluginRegister.Log("Entrances are null!", LogLevel.Error);
            }
        }
    }
}