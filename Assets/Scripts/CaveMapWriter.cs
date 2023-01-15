using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
public class CaveMapWriter
{
    private StreamWriter file;
    public DirectoryInfo DirectoryOfSaves;
    public void CreateNewSave(string NewSave){
        file = new StreamWriter("Saves/" + NewSave + ".Cave");
    }
    public bool CheckName(string nameToCheck){
        if(!Directory.Exists(MainManager.path + "\\Saves")){    
            Directory.CreateDirectory(MainManager.path + "\\Saves");
        }
        DirectoryOfSaves = new DirectoryInfo(MainManager.path + "\\Saves");

        FileInfo[] Files = DirectoryOfSaves.GetFiles("*.Cave"); 
        foreach(FileInfo fileTemp in Files){
            if(fileTemp.Name == nameToCheck + ".Cave") return false;
        }
        return true;
    }
    
    public void WriteMap(CaveGenCore GC){
        using(file){
            file.WriteLine("SizeX: " + GC.limit_x.ToString());
            file.WriteLine("SizeY: " + GC.limit_y.ToString());
            file.WriteLine("Seeds: " + ConvertList(ref GC.seeds_pos));
            file.WriteLine("Edges: " + ConvertList(ref GC.edges));

            file.WriteLine("Caves:");
            printPole(ref GC.poleCaves, GC.limit_x, GC.limit_y);
            file.WriteLine("Paths:");
            printPole(ref GC.polePaths, GC.limit_x, GC.limit_y);
            file.WriteLine("Rivers:");
            printPole(ref GC.poleRivers, GC.limit_x, GC.limit_y);
            file.WriteLine("Creatures:");
            printPole(ref GC.poleCreatures, GC.limit_x, GC.limit_y);
        }
    }
    private void printPole(ref char[,] pole, int size_x, int size_y){
        for(int i = 0; i < size_x; i++){
            for(int j = 0; j < size_y; j++){
                file.Write(pole[i, j]);
            }
            file.WriteLine();
        }
    }
    private string ConvertList(ref List<int[]> LTC){
        string res = "[";
        foreach(var elem in LTC){
            res += "{ " + elem[0].ToString() + " " + elem[1].ToString() + " };";
        }
        res += "]";
        return res;
    }
    
}
