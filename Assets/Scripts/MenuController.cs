using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.XR.ARFoundation.Samples;

public class MenuController : MonoBehaviour
{
    public GameObject echoSettings,instrumentSettings, artworkSettings,settingScreen;
    public ToggleGroup instrumentGroup;
    public GameObject InstrumentTogglePrefab;
    private List<GameObject> toggleRecords;
    public List<int> instrumentIDList;
    public bool isInit;
    public GameObject tempDoneButton;
    public ARManager arManager;
    private Toggle currentInstrumentToggle;

    private void Awake()
    {
        toggleRecords = new List<GameObject>();
        //isInit = true;
    }

    public void onEchoSettings() {
        echoSettings.SetActive(true);
    }
    public void offEchoSettings() {
        echoSettings.SetActive(false);
    }

    public void onInstrumentSettings()
    {
        instrumentSettings.SetActive(true);
        arManager.currentState = ARManager.UserState.InstrumentSettings;
        for (int i = 0; i < 4; i++) {
            var instr=Instantiate(InstrumentTogglePrefab, InstrumentTogglePrefab.transform.parent);
            var pos = instr.transform.position;
            instr.transform.position = new Vector3(pos.x+300*i,pos.y,pos.z);
            instr.SetActive(true);
            instr.GetComponent<InstrumentToggle>().Init(i);
            toggleRecords.Add(instr);
        }
    }

    public void offInstrumentSettings() {
        instrumentSettings.SetActive(false);
        foreach (var instr in toggleRecords) {
            Destroy(instr);
        }
        arManager.closeInstrumentSettings();
    }

    public void onArtworkSettings() {
        artworkSettings.SetActive(true);
    }

    public void offArtworkSettings()
    {
        artworkSettings.SetActive(false);
    }

    public void onSettingScreen() {
        settingScreen.SetActive(true);
    }

    public void offSettingScreen()
    {
        settingScreen.SetActive(false);
    }

    public void instrumentToggleChanged()
    {
        currentInstrumentToggle = instrumentGroup.ActiveToggles().FirstOrDefault();
        //Debug.Log(activeToggle.GetComponent<InstrumentToggle>().instrumentID);
        arManager.setSelectedInstrument(currentInstrumentToggle.GetComponent<InstrumentToggle>().instrumentID);

    }

    public void deselectToggle()
    {
        currentInstrumentToggle.isOn = false;
        instrumentGroup.ActiveToggles().FirstOrDefault().isOn = false;
    }
}
