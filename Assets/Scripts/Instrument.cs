using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MidiPlayerTK;
using System;

public class Instrument : MonoBehaviour
{
    public FMOD.Studio.EventInstance instance;
    public float sizeScale;
    public Vector3 originalSize;
    public string instrumentName;
    public List<MPTKEvent> MIDIsequence;
    public List<ParticleSystem> psList;
    private GameObject activatedParticle;


    public void Init(FMOD.Studio.EventInstance newInstance, string instrName, int particleIndex)
    {
        instance = newInstance;
        instance.start();
        instance.setPaused(true);
        sizeScale = 1;
        originalSize = transform.localScale;
        instrumentName = instrName;
        activateParticle(particleIndex);
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
        instance.setParameterByName("Size", sizeScale - 0.5f);
        activatedParticle.transform.position = transform.position;
    }


    private IEnumerator manipulateParticle(MPTKEvent midiEvent)
    {
        yield return new WaitForSeconds(midiEvent.RealTime / 1000);
        Debug.Log(midiEvent.Value.ToString());
        // TODO: Show note visualization
        // transform, midiEvent
        ParticleSystem particle = activatedParticle.GetComponent<ParticleSystem>();
        var main = particle.main;
        var shape = particle.shape;
        main.startColor = new Color(0.1f * midiEvent.Velocity, 0.3f * midiEvent.Velocity, 0.4f, 0.5f);
        main.maxParticles = (int)(0.5f*midiEvent.Value);
        shape.radius = 0.01f * midiEvent.Value;
    }

    private void activateParticle(int particleIndex)
    {
        foreach (ParticleSystem ps in psList)
        {
            ps.gameObject.SetActive(false);
        }
        psList[particleIndex].gameObject.SetActive(true);
        activatedParticle = psList[particleIndex].gameObject;
    }
}
