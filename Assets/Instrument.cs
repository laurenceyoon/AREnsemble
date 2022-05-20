using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instrument : MonoBehaviour
{
    public FMOD.Studio.EventInstance instance;

    public void Init(FMOD.Studio.EventInstance newInstance)
    {
        instance = newInstance;
        instance.start();
        instance.setPaused(true);
    }
    public void play()
    {
        instance.setPaused(false);
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
}
