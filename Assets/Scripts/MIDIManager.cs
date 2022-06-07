using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MidiPlayerTK;
using UnityEngine.XR.ARFoundation.Samples;

public class MIDIManager : MonoBehaviour
{
    [SerializeField]
    public MidiFileLoader midiFileLoader;
    public GameObject spawnedObject { get; private set; }
    public List<ParticleSystem> psList;
    public ARManager arManager;

    private void Awake()
    {
        Debug.Log("Awake: dynamically add MidiFileLoader component");

        // MidiPlayerGlobal is a singleton: only one instance can be created. 
        if (MidiPlayerGlobal.Instance == null)
            gameObject.AddComponent<MidiPlayerGlobal>();

        // When running, this component will be added to this gameObject
        midiFileLoader = gameObject.AddComponent<MidiFileLoader>();

        arManager.initInstruments();
    }

    public void midiInit(Instrument instrument)
    {
        print(instrument == null);
        midiFileLoader.MPTK_MidiName = instrument.instrumentName;
        // Load the MIDI file
        if (midiFileLoader.MPTK_Load())
        {
            // Read all MIDI events
            List<MPTKEvent> sequence = midiFileLoader.MPTK_ReadMidiEvents();
            Debug.Log($"Loading '{midiFileLoader.MPTK_MidiName}', MIDI events count:{sequence.Count}");
            instrument.MIDIsequence = sequence;
        }
        else
        {
            Debug.Log($"Loading '{midiFileLoader.MPTK_MidiName}' - Error");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
