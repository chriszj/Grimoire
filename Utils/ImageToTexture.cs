﻿namespace GLIB.Utils
{

    using UnityEngine;
    using System.Collections;

    using System.IO;

    public static class ImageToTexture
    {

        
        public static Sprite LoadNewSprite(string FilePath, float PixelsPerUnit = 100.0f, Texture2D tex2D = null)
        {

            // Load a PNG or JPG image from disk to a Texture2D, assign this texture to a new sprite and return its reference

            Sprite NewSprite = null;
            Texture2D SpriteTexture = LoadTexture(FilePath, tex2D);
            NewSprite = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height), new Vector2(0, 0));

            
            return NewSprite;
        }

        public static Texture2D LoadTexture(string FilePath, Texture2D tex2D = null)
        {

            // Load a PNG or JPG file from disk to a Texture2D
            // Returns null if load fails
            if (tex2D == null)
                tex2D = new Texture2D(2, 2);

            byte[] FileData;

            if (File.Exists(FilePath))
            {
                FileData = File.ReadAllBytes(FilePath);
                         // Create new "empty" texture
                if (tex2D.LoadImage(FileData))           // Load the imagedata into the texture (size is set automatically)
                    return tex2D;                 // If data = readable -> return texture
            }
            return null;                     // Return null if load failed
        }


    }

}
