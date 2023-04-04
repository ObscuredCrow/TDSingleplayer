using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AddAudio : MonoBehaviour
{
    [SerializeField] private bool playAfterAdding;
    [SerializeField] private bool music;
    [SerializeField] private bool effect;
    [SerializeField] private bool ambient;

    private AudioSource audio;
    private UISettings settings;

    private void Awake() {
        audio = GetComponent<AudioSource>();
        settings = FindObjectOfType<UISettings>();
        if (music) { settings.music.sources.Add(audio); audio.volume = settings.music.slider.value; }
        if (effect) { settings.effects.sources.Add(audio); audio.volume = settings.effects.slider.value; }
        if (ambient) { settings.ambient.sources.Add(audio); audio.volume = settings.ambient.slider.value; }
        if (playAfterAdding)
            audio.Play();
    }

    private void OnDestroy() {
        if (playAfterAdding)
            audio.Stop();
        if (music)
            settings.music.sources.Remove(audio);
        if (effect)
            settings.effects.sources.Remove(audio);
        if (ambient)
            settings.ambient.sources.Remove(audio);
    }
}