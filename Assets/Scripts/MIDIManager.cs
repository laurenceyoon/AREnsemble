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
    public GameObject spawnedObject { get; private set; }
    public List<ParticleSystem> psList;

    private void Awake()
    {
        Debug.Log("Awake: dynamically add MidiFileLoader component");

        // MidiPlayerGlobal is a singleton: only one instance can be created. 
        if (MidiPlayerGlobal.Instance == null)
            gameObject.AddComponent<MidiPlayerGlobal>();

        // When running, this component will be added to this gameObject
        midiFileLoader = gameObject.AddComponent<MidiFileLoader>();

        // Play test MIDI for debug
    //    Debug.Log($"psList: {psList}");
    //    Debug.Log($"psList: {psList[0]}");
    //    var particle = psList[0];
    //    particle.transform.position = new Vector3(100.0f, 100.0f, 0.0f);
    //    MIDIText.text = "HELLO WORLD!";
    //    midiFileLoader.MPTK_MidiName = "example";
    //    if (midiFileLoader.MPTK_Load())
    //    {
    //        MIDIText.text = "Load Succeed!";
    //        // Read all MIDI events
    //        List<MPTKEvent> sequence = midiFileLoader.MPTK_ReadMidiEvents();
    //        Debug.Log($"Loading '{midiFileLoader.MPTK_MidiName}', MIDI events count:{sequence.Count}");
    //        MIDIText.text = $"Loading '{midiFileLoader.MPTK_MidiName}', MIDI events count:{sequence.Count}";
            
    //        foreach (MPTKEvent midiEvent in sequence)
    //        {
    //            // Debug.Log($"Channel: {midiEvent.Channel}, Command: {midiEvent.Command}, Duration: {midiEvent.Duration}, Value: {midiEvent.Value}, Velocity: {midiEvent.Velocity}, RealTime: {midiEvent.RealTime}, Tick: {midiEvent.Tick}, TickTime: {midiEvent.TickTime}");
    //            if (midiEvent.Command == MPTKCommand.NoteOn) StartCoroutine(manipulateParticle(midiEvent, particle));
    //        }
    //    }
    //    else
    //    {
    //        MIDIText.text = "Load Fail!";
    //        Debug.Log($"Loading '{midiFileLoader.MPTK_MidiName}' - Error");
    //    }

    //    ParticleSystem ps = GetComponent<ParticleSystem>();
    //    var main = ps.main;
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
    private IEnumerator manipulateParticle(MPTKEvent midiEvent, ParticleSystem particle) // 추후에 Instrument을 추가 파라미터로 받아야 함
    {
        yield return new WaitForSeconds(midiEvent.RealTime / 1000);
        Debug.Log($"Channel: {midiEvent.Channel}, Command: {midiEvent.Command}, Duration: {midiEvent.Duration}, Value: {midiEvent.Value}, Velocity: {midiEvent.Velocity}, RealTime: {midiEvent.RealTime}, Tick: {midiEvent.Tick}, TickTime: {midiEvent.TickTime}");
        // TODO: Show note visualization
        var main = particle.main;
        main.startSize = 100.0f; // 0 ~ 3 사이의 값으로, velocity mapping
        // transform, midiEvent
    }
}
