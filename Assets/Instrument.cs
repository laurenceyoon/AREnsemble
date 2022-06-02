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
    public GameObject spawnedParticle { get; private set; }


    public void Init(FMOD.Studio.EventInstance newInstance, string instrName, ParticleSystem particle)
    {
        instance = newInstance;
        instance.start();
        instance.setPaused(true);
        sizeScale = 1;
        originalSize = transform.localScale;
        instrumentName = instrName;
        spawnedParticle = Instantiate(particle.gameObject, transform.position, transform.rotation);
        spawnedParticle.SetActive(true);
    }
    public void play()
    {
        instance.setPaused(false);
        foreach (MPTKEvent midiEvent in MIDIsequence)
        {
            Debug.Log($"Channel: {midiEvent.Channel}, Command: {midiEvent.Command}, Duration: {midiEvent.Duration}, Value: {midiEvent.Value}, Velocity: {midiEvent.Velocity}, RealTime: {midiEvent.RealTime}, Tick: {midiEvent.Tick}, TickTime: {midiEvent.TickTime}");
            if (midiEvent.Command == MPTKCommand.NoteOn) StartCoroutine(manipulateParticle(midiEvent));
        }
    }

    public void pause()
    {
        instance.setPaused(true);
    }

    /* public void move(FMOD.ATTRIBUTES_3D loc)
    {
        instance.set3DAttributes(loc);
    } */
    private void Update()
    {
        instance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(this.gameObject));
        instance.setParameterByName("Size", sizeScale-0.5f);
        spawnedParticle.transform.position = transform.position;
    }


    private IEnumerator manipulateParticle(MPTKEvent midiEvent)
    {
        yield return new WaitForSeconds(midiEvent.RealTime / 1000);
        Debug.Log(midiEvent.Value.ToString());
        // TODO: Show note visualization
        // transform, midiEvent
        var particle = spawnedParticle.GetComponentInParent<ParticleSystem>();
        var main = particle.main;
        main.startSize = 500.0f;
    }
}
