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
    
    public void midiInit(Instrument instrument)
    {
        MIDIText.text = "HELLO WORLD!";
        midiFileLoader.MPTK_MidiName = instrument.instrumentName;
        // Load the MIDI file
        if (midiFileLoader.MPTK_Load())
        {
            MIDIText.text = "Load Succeed!";
            // Read all MIDI events
            List<MPTKEvent> sequence = midiFileLoader.MPTK_ReadMidiEvents();
            Debug.Log($"Loading '{midiFileLoader.MPTK_MidiName}', MIDI events count:{sequence.Count}");
            MIDIText.text = $"Loading '{midiFileLoader.MPTK_MidiName}', MIDI events count:{sequence.Count}";
            /*foreach (MPTKEvent midiEvent in sequence)
            {
                Debug.Log($"Channel: {midiEvent.Channel}, Command: {midiEvent.Command}, Duration: {midiEvent.Duration}, Value: {midiEvent.Value}, Velocity: {midiEvent.Velocity}, RealTime: {midiEvent.RealTime}, Tick: {midiEvent.Tick}, TickTime: {midiEvent.TickTime}");
                if(midiEvent.Command==MPTKCommand.NoteOn) StartCoroutine(ShowNote(midiEvent));
            }*/
            instrument.MIDIsequence = sequence;
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
