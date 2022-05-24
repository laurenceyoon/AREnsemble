using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MidiPlayerTK;

public class Instrument : MonoBehaviour
{
    public FMOD.Studio.EventInstance instance;
    public float sizeScale;
    public Vector3 originalSize;
    public string instrumentName;
    public List<MPTKEvent> MIDIsequence;
    public ParticleSystem ps;


    public void Init(FMOD.Studio.EventInstance newInstance, string instrName)
    {
        instance = newInstance;
        instance.start();
        instance.setPaused(true);
        sizeScale = 1;
        originalSize = transform.localScale;
        instrumentName=instrName;
    }
    public void play()
    {
        instance.setPaused(false);
        foreach (MPTKEvent midiEvent in MIDIsequence)
        {
            Debug.Log($"Channel: {midiEvent.Channel}, Command: {midiEvent.Command}, Duration: {midiEvent.Duration}, Value: {midiEvent.Value}, Velocity: {midiEvent.Velocity}, RealTime: {midiEvent.RealTime}, Tick: {midiEvent.Tick}, TickTime: {midiEvent.TickTime}");
            if (midiEvent.Command == MPTKCommand.NoteOn) StartCoroutine(ShowNote(midiEvent));
        }
    }

    public void pause()
    {
        instance.setPaused(true);
    }

    /*public void move(FMOD.ATTRIBUTES_3D loc)
    {
        instance.set3DAttributes(loc);
    }*/
    private void Update()
    {
        instance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(this.gameObject));
    }


    private IEnumerator ShowNote(MPTKEvent midiEvent)
    {
        yield return new WaitForSeconds(midiEvent.RealTime / 1000);
        Debug.Log(midiEvent.Value.ToString());
        // TODO: Show note visualization 
        // transform, midiEvent
    }
}
