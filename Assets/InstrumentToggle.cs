using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InstrumentToggle : MonoBehaviour
{
    public int instrumentID;
    public Image image;
    public void Init(int id)
    {
        instrumentID = id;
        image.sprite = Resources.Load<Sprite>("UI/InstrumentSnapshots/"+instrumentID.ToString());   
    }
}
