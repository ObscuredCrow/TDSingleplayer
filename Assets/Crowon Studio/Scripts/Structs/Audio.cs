using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct Audio
{
    public Slider slider;
    public List<AudioSource> sources;
    [HideInInspector] public float savedVolume;
}