using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InstrumentToggle : MonoBehaviour
{
    public int instrumentID;
    public Image image;
    public Text InstrumentName;
    public void Init(int id)
    {
        instrumentID = id;
        //image.sprite = Resources.Load<Sprite>((instrumentID+1).ToString()+".png"); 
        switch (instrumentID) {
            case 0:
                InstrumentName.text = "Violin";
                break;
            case 1:
                InstrumentName.text = "Viola";
                break;
            case 2:
                InstrumentName.text = "Cello";
                break;
            case 3:
                InstrumentName.text = "Piano";
                break;

        }
    }
}
