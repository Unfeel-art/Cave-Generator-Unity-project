using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CaveMapReader
{
    private StreamReader file;
    public DirectoryInfo DirectoryOfSaves;
    private CaveGenCore GC = new CaveGenCore(-1, -1);
    public CaveGenCore ReadSave(string Name){
        if(!Directory.Exists(MainManager.path + "\\Saves")){    
            Directory.CreateDirectory(MainManager.path + "\\Saves");
        }
        DirectoryOfSaves = new DirectoryInfo(MainManager.path + "\\Saves");
         FileInfo[] Files = DirectoryOfSaves.GetFiles("*.Cave"); 
         foreach(FileInfo fileTemp in Files){
            if(fileTemp.Name == Name + ".Cave"){
                int stage = 0, x = 1, y = 1;
                int poleCaveX = 0, polePathsX = 0, poleRiversX = 0, poleCreaturesX = 0;
                using(file = new StreamReader("Saves/" + fileTemp.Name)){
                    string line = "";
                    
                    while ((line = file.ReadLine()) != null){
                        string word = "", word2="";
                        int inpNum, pos = 0, inpNum2;
                        while(pos < line.Length){
                            pos = getNextWord(pos, ref line, ref word);
                            if(word == "SizeX:" || word =="SizeY:" || word == "Seeds:" || word == "Edges:" || word == "Caves:" || word == "Paths:" || word == "Rivers:" || word == "Creatures:") stage++;
                            else if(stage == 5){
                                for(int i = 0; i < word.Length; i++){
                                    GC.poleCaves[poleCaveX, i] = word[i];
                                }
                                poleCaveX++;
                            }
                            else if(stage == 6){
                                for(int i = 0; i < word.Length; i++){
                                    GC.polePaths[polePathsX, i] = word[i];
                                }
                                polePathsX++;
                            }
                            else if(stage == 7){
                                for(int i = 0; i < word.Length; i++){
                                    GC.poleRivers[poleRiversX, i] = word[i];
                                }
                                poleRiversX++;
                            }
                            else if(stage == 9){
                                for(int i = 0; i < word.Length; i++){
                                    GC.poleCreatures[poleCreaturesX, i] = word[i];
                                }
                                poleCreaturesX++;
                            }
                            else if(int.TryParse(word, out inpNum)){
                                if(stage == 1){
                                    x = inpNum;
                                }
                                else if(stage == 2){
                                    y = inpNum;
                                    GC = new CaveGenCore(x, y);
                                }
                                else if(stage == 3){
                                    pos = getNextWord(pos, ref line, ref word2);
                                    if(int.TryParse(word2, out inpNum2)){
                                        int[] seed_pos = new int[]{inpNum, inpNum2};
                                        GC.seeds_pos.Add(seed_pos);
                                    }
                                    else{
                                        GC.limit_x = -1;
                                    }
                                }
                                else if(stage == 4){
                                    pos = getNextWord(pos, ref line, ref word2);
                                    if(int.TryParse(word2, out inpNum2)){
                                        int[] edge = new int[]{inpNum, inpNum2};
                                        GC.edges.Add(edge);
                                    }
                                    else{
                                        GC.limit_x = -1;
                                    }
                                }
                                word="";
                                word2="";
                            }
                        }
                    }
                }
                return GC;
            }
         }
         return GC;
    }
    private static int getNextWord(int pos, ref string line, ref string word){
        word="";
        for(int i = pos; i < line.Length; i++){
            if(line[i] == ' '){
                return i+1;
            }
            else{
                word += line[i];
            }
        }
        return line.Length;
    }
}
