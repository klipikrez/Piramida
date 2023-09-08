using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class HeptagramAboveHead : MonoBehaviour
{
    public VisualEffect heptagramParticles;
    public MeshRenderer heptahram;
    public Animator laserHeptagramAnimation;
    Material heptagramMaterial;
    public float time = 52f;
    float timer = 0;
    private void Start()
    {

        laserHeptagramAnimation.speed = 1 / time;
        heptagramMaterial = heptahram.material;
        heptagramMaterial.SetFloat("_Fade", 1);

    }
    private void Update()
    {
        // heptagramMaterial.SetFloat("_Fade", 1 - (timer / time));
        timer += Time.deltaTime;

    }


    public void EndWarmup()
    {
        heptagramParticles.SendEvent("Stop");
    }
}
