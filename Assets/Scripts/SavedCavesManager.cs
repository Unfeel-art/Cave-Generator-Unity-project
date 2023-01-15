using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class SavedCavesManager : MonoBehaviour
{
    public DirectoryInfo DirectoryOfSaves;
    public Text SaveNameText;
    public Button LoadSaveButton;
    public Button DeleteSaveButton;
    private RectTransform Content;
    private float contentSizeX = 1901, contentSizeY = 1;
    private List<Text> SaveNames = new List<Text>();
    private List<Button> LoadButtons = new List<Button>();
    private List<Button> DeleteButtons = new List<Button>();
    [SerializeField] public GameObject LoadPlane;
    private GameObject plane;
    private int counter;
    void Start(){
        if(!Directory.Exists(MainManager.path + "\\Saves")){    
            Directory.CreateDirectory(MainManager.path + "\\Saves");
        }
        DirectoryOfSaves = new DirectoryInfo(MainManager.path + "\\Saves");
        counter = 0;
        Content = GameObject.Find("Content").GetComponent<RectTransform>();
        PrintSaveMenu();
    }
    private void PrintSaveMenu()
    {
        FileInfo[] Files = DirectoryOfSaves.GetFiles("*.Cave");
        if(Files.Length == 0)GameObject.Find("NoSavesText").GetComponent<Text>().text = "You haven't got saved caves!";
        else GameObject.Find("NoSavesText").GetComponent<Text>().text = "";
        
        Vector2 spawnPos = new Vector2(-500, -300+452*counter);

        Content.sizeDelta = new Vector2(contentSizeX, contentSizeY + 180*Files.Length);
        for(int i = 0; i < Files.Length; i++){
            FileInfo file = Files[i];
            Text newText = Instantiate(SaveNameText, Content);
            newText.transform.position = spawnPos;
            SaveNames.Add(newText);

            spawnPos = new Vector2(spawnPos.x, spawnPos.y - 50);
            
            Button newLoadButton = Instantiate(LoadSaveButton, Content);
            newLoadButton.transform.position = new Vector2(spawnPos.x + 55, spawnPos.y);;
            LoadButtons.Add(newLoadButton);
            string nameOfFile = file.Name;
            nameOfFile = nameOfFile.Remove(nameOfFile.Length-5);
            newLoadButton.onClick.AddListener(() => LoadSave(MainManager.path + "\\Saves\\" + nameOfFile, nameOfFile));

            Button newDeleteButton = Instantiate(DeleteSaveButton, Content);
            newDeleteButton.transform.position = new Vector2(spawnPos.x + 200, spawnPos.y);
            DeleteButtons.Add(newDeleteButton);
            newDeleteButton.onClick.AddListener(() => DeleteSave(MainManager.path + "\\Saves\\" + nameOfFile));

            spawnPos = new Vector2(spawnPos.x, spawnPos.y - 50);

            newText.text = "Save name: " + file.Name;
        }
        
    }
    private void clearSaveMenu(){
        foreach(Text textToDelete in SaveNames){
            Destroy(textToDelete.gameObject);
        }
        foreach(Button buttonToDelete in LoadButtons){
            Destroy(buttonToDelete.gameObject);
        }
        foreach(Button buttonToDelete in DeleteButtons){
            Destroy(buttonToDelete.gameObject);
        }
        SaveNames.Clear();
        LoadButtons.Clear();
        DeleteButtons.Clear();
    }
    public void DeleteSave(string pathToDelete){
        counter = 1;
        File.Delete(pathToDelete+".Cave");
        File.Delete(pathToDelete+".png");
        clearSaveMenu();
        PrintSaveMenu();
    }
    public void LoadSave(string pathToLoad, string LoadFileName){
        plane = Instantiate(LoadPlane, GameObject.Find("SavesManager").GetComponent<Transform>());

        Transform panel = plane.transform.Find("Panel");
        
        Image img = panel.Find("Image").GetComponent<Image>();
        Button BackButton = panel.Find("BackButton").GetComponent<Button>();
        Button LoadButton = panel.Find("LoadButton").GetComponent<Button>();

        if(File.Exists(pathToLoad + ".png")) {
            Texture2D texture = LoadPNG(pathToLoad + ".png");
            img.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        }

        LoadButton.onClick.AddListener(() => {PressLoad(pathToLoad, LoadFileName);});
        BackButton.onClick.AddListener(BackLoad);
    }

    public void PressLoad(string pathToLoad, string LoadFileName){
        CaveMapReader cr = new CaveMapReader();
        if(File.Exists(pathToLoad + ".Cave")) MainManager.genCore = cr.ReadSave(LoadFileName);
        MainManager.Noload = false;
        SceneManager.LoadScene(1);

    }

    public void BackLoad(){
        Destroy(plane);
    }

    public static Texture2D LoadPNG(string filePath) {
 
        Texture2D texture = null;
        byte[] fileData;
    
        if (File.Exists(filePath))     {
            fileData = File.ReadAllBytes(filePath);
            texture = new Texture2D(1, 1);
            texture.LoadImage(fileData);
        }
        return texture;
    }
}
