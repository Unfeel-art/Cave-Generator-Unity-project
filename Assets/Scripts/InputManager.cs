using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    private string PoleHeight = "";
    private string PoleWidth = "";
    private string NumberOfCaves="";
    private string MinCaveSize = "";
    private string MaxCaveSize = "";
    
    public bool isInputCorrect = false;

    public GameObject PoleHeightField;
    public GameObject PoleWidthField;
    public GameObject NumberOfCavesField;
    public GameObject MinCaveSizeField;
    public GameObject MaxCaveSizeField;
    public GameObject WarnErrorText;

    public bool checkInput(){
        int temp, temp2;
        if((!(int.TryParse(PoleWidth, out temp))) || temp<=0 || temp>200){
            return false;
        }
        else if((!(int.TryParse(PoleHeight, out temp))) || temp<=0 || temp>200){
            return false;
        }
        else if((!(int.TryParse(NumberOfCaves, out temp))) || temp<=0 || temp>500){
            return false;
        }
        else if(!(int.TryParse(MinCaveSize, out temp2))|| temp2<=0){
            return false;
        }
        else if(!(int.TryParse(MaxCaveSize, out temp))|| temp<=0 || temp2>temp){
            return false;
        }
        return true;
    }
    
    public void setInput(ref int width, ref int height, ref int caveNum, ref int minCaveSize, ref int maxCaveSize){
        int.TryParse(PoleWidth, out width);
        int.TryParse(PoleHeight, out height);
        int.TryParse(NumberOfCaves, out caveNum);
        int.TryParse(MinCaveSize, out minCaveSize);
        int.TryParse(MaxCaveSize, out maxCaveSize);
    }
    public void setWarnError(){
        if(isInputCorrect == false)
            WarnErrorText.GetComponent<Text>().text = "Error: grid height, grid width, number of caves, minimum cave size, maximum cave size have to be positive integer numbers\ngrid width and height<=200;\nnumber of caves<=500;\nminimum cave size<=maximum cave size;";
        else 
            WarnErrorText.GetComponent<Text>().text = "";
    }

    public void takeInput(){
        PoleHeight = PoleHeightField.GetComponent<Text>().text;
        PoleWidth = PoleWidthField.GetComponent<Text>().text;
        NumberOfCaves = NumberOfCavesField.GetComponent<Text>().text;
        MinCaveSize = MinCaveSizeField.GetComponent<Text>().text;
        MaxCaveSize = MaxCaveSizeField.GetComponent<Text>().text;
        isInputCorrect = checkInput();
        setWarnError();
    }
    
}
