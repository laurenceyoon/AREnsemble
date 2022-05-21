using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    // Start is called before the first frame update
    public ParticleSystem ps;
    public float size;
    public int maxParticleNumber;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        var main = ps.main;
        main.startSize = size;
        main.maxParticles = maxParticleNumber;

    }
}
