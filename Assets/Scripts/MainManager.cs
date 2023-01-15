using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class MainManager : MonoBehaviour
{
    [SerializeField] int Pos_x;
    [SerializeField] int Pos_y;
    [SerializeField] int size_x;
    [SerializeField] int size_y;
    [SerializeField] int caveSizeLow;
    [SerializeField] int caveSizeHigh;
    [SerializeField] int caveNumber;
    [SerializeField] float gapSize;
    [SerializeField] GameObject WallTileCave;
    [SerializeField] GameObject WallTileError;
    [SerializeField] GameObject WallTileWall;
    GameObject[,] Grid = new GameObject[207, 207];
    public static GameObject tileWall, tileCave, tileError;
    public static CaveGenCore genCore;
    public static string path = Directory.GetCurrentDirectory();
    [SerializeField] private InputManager inputFields;
    public static bool Noload = true;
    void Start()
    {
        if(Noload) {genCore = new CaveGenCore(size_x, size_y);}
        else{
            Noload = true;
            size_x = genCore.limit_x;
            size_y = genCore.limit_y;
        }

        tileWall= (GameObject)Instantiate(WallTileWall, transform);
        tileCave= (GameObject)Instantiate(WallTileCave, transform);
        tileError= (GameObject)Instantiate(WallTileError, transform);

        GenerateGrid();
    }

    public void GenCaves(){
        inputFields.takeInput();
        if(inputFields.isInputCorrect == false){
            return;
        }
        DestroyGrid();
        inputFields.setInput(ref size_x, ref size_y, ref caveNumber, ref caveSizeLow, ref caveSizeHigh);
        genCore = new CaveGenCore(size_x, size_y);
        genCore.GenerateCaves(caveNumber, caveSizeLow, caveSizeHigh);
        GenerateGrid();
        
    }

    public void GenPaths(){
        inputFields.takeInput();
        if(inputFields.isInputCorrect == false){
            return;
        }
        genCore.CreatePaths(caveNumber);
        GenerateGrid();

    }

    private void DestroyGrid(){
        for(int i=1; i<=size_x; i++){
            for (int j = 1; j <= size_y; j++)
            {
                Destroy(Grid[i, j]);
            }
        }
    }

    private void GenerateGrid()
    {
        
        for(int i=1; i<=size_x; i++){
            for (int j = 1; j <= size_y; j++)
            {
                Destroy(Grid[i, j]);
                GameObject tile;
                char cell = genCore.getCell(i, j);
                if(cell == '.')
                    tile= (GameObject)Instantiate(WallTileWall, transform);
                else if(cell == 's' || cell == 'S')
                    tile= (GameObject)Instantiate(WallTileCave, transform);
                else 
                    tile= (GameObject)Instantiate(WallTileError, transform);

                float tileSizeX = tile.GetComponent<SpriteRenderer>().bounds.size.x;
                float tileSizeY = tile.GetComponent<SpriteRenderer>().bounds.size.y;
                float Scale = 460 / tileSizeX / (float)Mathf.Max(size_x, size_y);
                
                tile.transform.localScale = new Vector2(Scale, Scale);
                tileSizeX *= Scale;
                tileSizeY *= Scale;
                

                float posX = i * (tileSizeX + gapSize) + Pos_x - (tileSizeX + gapSize) + tileSizeX/2;
                float posY = j * -(tileSizeY + gapSize) + Pos_y + (tileSizeY + gapSize) - tileSizeY/2;
                
                tile.transform.position = new Vector2(posX, posY);
                Grid[i, j] = tile;
            }
        }
        
    }
}
