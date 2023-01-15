using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ImageConverter 
{
    
    private int TextureSize = 16;
    public void Draw(int sizeX, int sizeY, string NameOfMap)
    {
        Texture2D texture = new Texture2D(sizeX*16, sizeY*16);
        for (var x = 1; x <= sizeX; x++)
        {
            for (var y = sizeY; y >= 1; y--)
            {
                GameObject tile;
                char cell = MainManager.genCore.getCell(x, y);
                
                if(cell == '.')
                    tile= MainManager.tileWall;
                else if(cell == 's' || cell == 'S')
                    tile= MainManager.tileCave;
                else
                    tile= MainManager.tileError;

                SpriteRenderer sr = tile.GetComponent<SpriteRenderer>();
                Texture2D tileTexture = sr.sprite.texture;

                for(int x1 = 0; x1 < TextureSize; x1++){
                    for(int y1 = 0; y1 < TextureSize; y1++){
                        int currX = (x - 1) * TextureSize + x1;
                        int currY = (sizeY - y) * TextureSize + y1;
                        texture.SetPixel(currX, currY, tileTexture.GetPixel(x1, y1));
                    }
                }
                
            }
        }
        byte[] bytes = texture.EncodeToPNG();
        var dirPath = Directory.GetCurrentDirectory() + "\\Saves\\";
        if(!Directory.Exists(dirPath)) {
            Directory.CreateDirectory(dirPath);
        }
        File.WriteAllBytes(dirPath + NameOfMap + ".png", bytes);
    }
}
