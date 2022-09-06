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
    public List<ParticleSystem> psList;
    private GameObject activatedParticle;
    private int psIndex;
    private Dictionary<string, float> violinPitch = new Dictionary<string, float>
    {
        { "mean", 73.82f },
        { "min", 62f },
        { "max", 81f }
    };
    private Dictionary<string, float> violinVelocity = new Dictionary<string, float>
    {
        { "mean", 59.65f },
        { "min", 24f },
        { "max", 84f }
    };
    private Dictionary<string, float> violinDuration = new Dictionary<string, float>
    {
        { "mean", 336469.31f },
        { "min", 34090.90f },
        { "max", 2186363.45f }
    };
    private Dictionary<string, float> violaPitch = new Dictionary<string, float>
    {
        { "mean", 60.94f },
        { "min", 52f },
        { "max", 69f }
    };
    private Dictionary<string, float> violaVelocity = new Dictionary<string, float>
    {
        { "mean", 61.26f },
        { "min", 33f },
        { "max", 84f }
    };
    private Dictionary<string, float> violaDuration = new Dictionary<string, float>
    {
        { "mean", 336469.31f },
        { "min", 61363.63f },
        { "max", 1822727.12f }
    };
    private Dictionary<string, float> celloPitch = new Dictionary<string, float>
    {
        { "mean", 62.02f },
        { "min", 56f },
        { "max", 67f }
    };
    private Dictionary<string, float> celloVelocity = new Dictionary<string, float>
    {
        { "mean", 54.16f },
        { "min", 30f },
        { "max", 80f }
    };
    private Dictionary<string, float> celloDuration = new Dictionary<string, float>
    {
        { "mean", 336469.31f },
        { "min", 43181.81f },
        { "max", 1872727.11f }
    };

    public void Init(FMOD.Studio.EventInstance newInstance, string instrName, int particleIndex)
    {

        instance = newInstance;
        instance.start();
        instance.setPaused(true);
        sizeScale = 1;
        originalSize = transform.localScale;
        instrumentName = instrName;
        psIndex = particleIndex;
        deactivateAllParticle();
    }
    public void play()
    {
        // instance.start();
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
        deactivateAllParticle();
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

    public void activate()
    {
        instance.setParameterByName("Active",1);
    }

    public void deactivate()
    {
        instance.setParameterByName("Active", 0);
    }

    private IEnumerator manipulateParticle(MPTKEvent midiEvent)
    {
        yield return new WaitForSeconds(midiEvent.RealTime / 1000);
        if (activatedParticle == null)
            activateParticle();

        ParticleSystem particle = activatedParticle.GetComponent<ParticleSystem>();
        var main = particle.main;
        var shape = particle.shape;
        if (particle.name == "fx_wave_shockwave_d")  // violin
        {
            float scaledValue = (midiEvent.Value - violinPitch["min"]) / (violinPitch["max"] - violinPitch["min"]);
            float scaledDuration = (midiEvent.Duration - violinDuration["min"]) / (violinDuration["max"] - violinDuration["min"]);
            float scaledVelocity = (midiEvent.Velocity - violinVelocity["min"]) / (violinVelocity["max"] - violinVelocity["min"]);
            main.startColor = new Color(255f * scaledDuration, 255f, 255f, 255f);
            main.maxParticles = (int)(10f * scaledValue);
            main.startSize = 3f * scaledVelocity;
            shape.radius = 0.02f * scaledValue;
        }
        else if (particle.name == "fx_wave_e")  // viola x
        {
            float scaledValue = (midiEvent.Value - violaPitch["min"]) / (violaPitch["max"] - violaPitch["min"]);
            float scaledDuration = (midiEvent.Duration - violaDuration["min"]) / (violaDuration["max"] - violaDuration["min"]);
            float scaledVelocity = (midiEvent.Velocity - violaVelocity["min"]) / (violaVelocity["max"] - violaVelocity["min"]);
            main.startColor = new Color(255f * scaledDuration, 149f, 58f, 255f);
            //main.maxParticles = (int)(3f * scaledDuration);
            main.startSize = scaledVelocity;
        }
        else if (particle.name == "fx_wave_c")  // cello x
        {
            float scaledValue = (midiEvent.Value - celloPitch["min"]) / (celloPitch["max"] - celloPitch["min"]);
            float scaledDuration = (midiEvent.Duration - celloDuration["min"]) / (celloDuration["max"] - celloDuration["min"]);
            float scaledVelocity = (midiEvent.Velocity - celloVelocity["min"]) / (celloVelocity["max"] - celloVelocity["min"]);
            main.startColor = new Color(255f * scaledDuration, 255f, 255f, 255f);
            //main.maxParticles = (int)(10f * scaledDuration);
            main.startSize = scaledVelocity;
            shape.radius = 0.02f * scaledValue;
        }
        //else  // piano
        //{
        //    main.startColor = new Color(0.1f * midiEvent.Velocity, 0.3f * midiEvent.Velocity, 0.4f, 0.5f);
        //    main.maxParticles = (int)(0.5f * midiEvent.Value);
        //    shape.radius = 0.003f * midiEvent.Value;
        //}
    }

    private void activateParticle()
    {
        deactivateAllParticle();
        psList[psIndex].gameObject.SetActive(true);
        activatedParticle = psList[psIndex].gameObject;
    }

    private void deactivateAllParticle()
    {
        foreach (ParticleSystem ps in psList)
        {
            ps.gameObject.SetActive(false);
        }
    }
}
