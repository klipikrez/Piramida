using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "newShieldStage", menuName = "Bosses/Piramida/ShieldStages")]

public class ShieldStages : ScriptableObject
{
    public AnimationCurve fresnel;
    [GradientUsage(true)]
    public Gradient color;
    public AnimationCurve timeSpeed;
    public AnimationCurve cellDensity;
    public AnimationCurve offset;
    public AnimationCurve kurvaZaFlicker;
    public float time = 1f;
}
