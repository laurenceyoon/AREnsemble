using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MidiPlayerTK;

public class MIDIManager : MonoBehaviour
{
    [SerializeField]
    public Text MIDIText;
    public MidiFileLoader midiFileLoader;

    private void Awake()
    {
        Debug.Log("Awake: dynamically add MidiFileLoader component");

        // MidiPlayerGlobal is a singleton: only one instance can be created. 
        if (MidiPlayerGlobal.Instance == null)
            gameObject.AddComponent<MidiPlayerGlobal>();

        // When running, this component will be added to this gameObject
        midiFileLoader = gameObject.AddComponent<MidiFileLoader>();
    }

    // Start is called before the first frame update
    void Start()
    {
        MIDIText.text = "HELLO WORLD!";
        midiFileLoader.MPTK_MidiName = "violin";
        // Load the MIDI file
        if (midiFileLoader.MPTK_Load())
        {
            MIDIText.text = "Load Succeed!";
            // Read all MIDI events
            List<MPTKEvent> sequence = midiFileLoader.MPTK_ReadMidiEvents();
            Debug.Log($"Loading '{midiFileLoader.MPTK_MidiName}', MIDI events count:{sequence.Count}");
            MIDIText.text = $"Loading '{midiFileLoader.MPTK_MidiName}', MIDI events count:{sequence.Count}";
        }
        else
        {
            MIDIText.text = "Load Fail!";
            Debug.Log($"Loading '{midiFileLoader.MPTK_MidiName}' - Error");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
