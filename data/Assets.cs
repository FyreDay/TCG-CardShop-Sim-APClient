using System.IO;
using System.Reflection;
using UnityEngine;

public static class EmbeddedResources
{
    public static Texture2D LoadTexture(string resourceName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        using (Stream stream = assembly.GetManifestResourceStream(resourceName))
        {
            if (stream == null)
            {
                Debug.LogError($"Embedded resource not found: {resourceName}");
                return null;
            }

            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(buffer);
            return texture;
        }
    }
}