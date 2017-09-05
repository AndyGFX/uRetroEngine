using UnityEngine;
using System.Collections;
using System.IO;

public static class PNG
{
    /// <summary>
    /// Convert Texture to Base64 string
    /// </summary>
    /// <param name="texture"></param>
    /// <returns></returns>
	public static string ToBase64(Texture2D texture)
    {
        try
        {
            string result = "";
            byte[] bytes = texture.EncodeToPNG();
            result = System.Convert.ToBase64String(bytes);
            return result;
        }
        catch
        {
            Debug.LogError("CHANGE TEXTURE FORMAT on texture name:" + texture.name);
        }

        return "";
    }

    /// <summary>
    /// Convert Base64 string to texture
    /// </summary>
    /// <param name="base64Str"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    public static Texture2D FromBase64(string base64Str, int width, int height)
    {
        byte[] data_image = System.Convert.FromBase64String(base64Str);
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        texture.LoadImage(data_image);

        return texture;
    }
	
	public static void Save(string path,Texture2D texture)
	{
		File.WriteAllBytes(path, texture.EncodeToPNG());
		
	}
    /// <summary>
    /// Convert GrayScale texture to normal map
    /// </summary>
    /// <param name="source"></param>
    /// <param name="strength"></param>
    /// <returns></returns>
    public static Texture2D ConvertToNormalMap(Texture2D source, float strength)
    {
        strength = Mathf.Clamp(strength, 0.0F, 10.0F);
        Texture2D result;
        float xLeft;
        float xRight;
        float yUp;
        float yDown;
        float yDelta;
        float xDelta;
        result = new Texture2D(source.width, source.height, TextureFormat.ARGB32, true);

        for (int bby = 0; bby < result.height; bby++)
        {
            for (int bx = 0; bx < result.width; bx++)
            {
                xLeft = source.GetPixel(bx - 1, bby).grayscale * strength;
                xRight = source.GetPixel(bx + 1, bby).grayscale * strength;
                yUp = source.GetPixel(bx, bby - 1).grayscale * strength;
                yDown = source.GetPixel(bx, bby + 1).grayscale * strength;
                xDelta = ((xLeft - xRight) + 1) * 0.5f;
                yDelta = ((yUp - yDown) + 1) * 0.5f;
                result.SetPixel(bx, bby, new Color(xDelta, yDelta, 1.0f, yDelta));
            }
        }
        result.Apply();
        return result;
    }

    public static Texture2D LoadPNG(string filePath)
    {
        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists(filePath))
        {
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData);
        }
        return tex;
    }
}