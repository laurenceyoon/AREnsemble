using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    public GameObject echoSettings;

    public void onSettings() {
        echoSettings.SetActive(true);
    }
    public void offSettings() {
        echoSettings.SetActive(false);
    }
}
