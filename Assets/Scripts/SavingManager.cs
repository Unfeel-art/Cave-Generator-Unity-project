using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SavingManager: MonoBehaviour
{
    private string CaveName = "";
    private string alphabetOfName = "aAbBcCdDeEfFgGhHiIjJkKlLmMnNoOpPqQrRsStTuUvVwWxXyYzZ1234567890";
    private Text WarnErrorText;
    public GameObject SavingPlane;
    private GameObject plane;
    private InputField CaveNameField;
    public void startSaving(){
        plane = Instantiate(SavingPlane, transform);

        Transform panel = plane.transform.Find("Panel");
        
        CaveNameField = panel.Find("CaveNameField").GetComponent<InputField>();
        WarnErrorText = panel.Find("WarnErrorText2").GetComponent<Text>();
        Button OkButton = panel.Find("OkButton").GetComponent<Button>();
        Button CancelButton = panel.Find("CancelButton").GetComponent<Button>();

        OkButton.onClick.AddListener(PressOk);
        CancelButton.onClick.AddListener(PressCancel);
    }
    public void PressOk(){
        CaveName = CaveNameField.text;
        CaveMapWriter newMap = new CaveMapWriter();
        if(CheckInput(ref CaveName) == false){
            setWarnError(0);
            return;
        }
        if(newMap.CheckName(CaveName) == false){
            setWarnError(-1);
            return;
        }
        setWarnError(1);
        newMap.CreateNewSave(CaveName);
        newMap.WriteMap(MainManager.genCore);
        ImageConverter Img = new ImageConverter();
        Img.Draw(MainManager.genCore.limit_x, MainManager.genCore.limit_y, CaveName);
        Destroy(plane);
    }
    public void PressCancel(){
        Destroy(plane);
    }
    private bool CheckInput(ref string input){
        if(input.Length == 0) return false; 
        for(int i = 0; i < input.Length; i++){
            bool check = false;
            for(int j = 0; j < alphabetOfName.Length; j++){
                if(input[i] == alphabetOfName[j]) check = true;
            }
            if(check == false) return false;
        }
        return true;
    }
    private void setWarnError(int isInputCorrect){
        if(isInputCorrect == 0)
            WarnErrorText.GetComponent<Text>().text = "Error: the cave name has to consist only of English letters and numbers(without spaces)!";
        else if(isInputCorrect == -1){
            WarnErrorText.GetComponent<Text>().text = "Error: this cave name already exists!";
        }
        else 
            WarnErrorText.GetComponent<Text>().text = "";
    }
}
