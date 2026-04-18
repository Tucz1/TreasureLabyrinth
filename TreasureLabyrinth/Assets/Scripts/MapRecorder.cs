using UnityEngine;
using System.IO;
using UnityEditor;

public static class RenderTextureSaver
{
    public static void SaveRenderTextureToPNG(RenderTexture rt, string path)
    {
        // Backup currently active RT
        RenderTexture currentRT = RenderTexture.active;

        // Set the given RenderTexture as active
        RenderTexture.active = rt;

        // Create a Texture2D with same dimensions
        Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);

        // Read pixels from RenderTexture into Texture2D
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        tex.Apply();

        // Restore previously active RT
        RenderTexture.active = currentRT;

        // Encode to PNG
        byte[] pngData = tex.EncodeToPNG();

        // Write to disk
        File.WriteAllBytes(path, pngData);

        // Cleanup (important in editor scripts)
        Object.DestroyImmediate(tex);
    }
}

public class MapRecorder : MonoBehaviour
{
    [SerializeField] RenderTexture myRenderTexture;
    #if UNITY_EDITOR
    [MenuItem("Tools/Save RT")]
    static void Save()
    {
        string path = Path.Combine(Application.dataPath, "output.png");
        var myObj = FindAnyObjectByType<MapRecorder>();
        RenderTextureSaver.SaveRenderTextureToPNG(myObj.myRenderTexture, path);
        UnityEditor.AssetDatabase.Refresh();

        Debug.Log("Saved to: " + path);
    }
    #endif
}