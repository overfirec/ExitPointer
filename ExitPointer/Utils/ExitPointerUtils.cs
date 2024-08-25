using System;
using System.IO;
using System.Reflection;
using BepInEx.Logging;
using UnityEngine;
using UnityEngine.UI;


namespace ExitPointer.Utils;

public static class ResourcesLoader
{
    // This part of code from NicholaScott.BepInEx.Utils.Resources.Extensions
    // link:https://thunderstore.io/c/lethal-company/p/Ozone/BepInUtils/
    private static byte[] ReadAllBytes(this Stream inStream)
    {
        if (inStream is MemoryStream memoryStream)
            return memoryStream.ToArray();
        using (MemoryStream destination = new MemoryStream())
        {
            inStream.CopyTo((Stream)destination);
            return destination.ToArray();
        }
    }

    // This part of code from NicholaScott.BepInEx.Utils.Resources.Extensions
    // link:https://thunderstore.io/c/lethal-company/p/Ozone/BepInUtils/
    internal static Texture2D? LoadResource(string resourceName)
    {
        try
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            string name = executingAssembly.GetName().Name + ".Image." + resourceName + ".png";
            Texture2D val = new Texture2D(0, 0);
            ImageConversion.LoadImage(val, ReadAllBytes(executingAssembly.GetManifestResourceStream(name)));
            return val;
        }

        catch (Exception e)
        {
            ExitPointerPluginRegister.Log($"Error loading resource {resourceName}: {e.Message}", LogLevel.Error);
            return null;
        }
    }

    // Create GameObject with Image component and set the sprite of the Image component to the arrowTexture2D
    internal static GameObject? SetGameObject(string objectName, Texture2D arrowTexture2D)
    {
        try
        {
            GameObject obj = new GameObject(objectName);
            obj.AddComponent<Image>();
            obj.GetComponent<Image>().sprite = Sprite.Create(arrowTexture2D,
                new Rect(0, 0, arrowTexture2D.width, arrowTexture2D.height), new Vector2(0.5f, 0.5f));

            return obj;
        }
        catch
        {
            return null;
        }
    }
}

internal static class Extensions{
    // Get Exit position
    internal static Tuple<Transform, Transform>? GetExitPosition()
    {
        EntranceTeleport[] entranceArray = UnityEngine.Object.FindObjectsByType<EntranceTeleport>(FindObjectsSortMode.None);
        
        Transform? mainExitEntrance = null;
        Transform? fireExitEntrance = null;

        if (entranceArray != null && entranceArray.Length > 0)
        {
            for (int i = 0; i < entranceArray.Length; i++)
            {
                switch (entranceArray[i].name)
                {
                    case "EntranceTeleportA(Clone)":
                    {
                        mainExitEntrance = entranceArray[i].entrancePoint;
                        break;
                    }

                    case "EntranceTeleportB(Clone)":
                    {
                        fireExitEntrance = entranceArray[i].entrancePoint;
                        break;
                    }
                }
            }
        }

        if (fireExitEntrance != null && mainExitEntrance != null)
        {
            return new Tuple<Transform, Transform>(mainExitEntrance, fireExitEntrance);
        }
        
        return null;
    }
}