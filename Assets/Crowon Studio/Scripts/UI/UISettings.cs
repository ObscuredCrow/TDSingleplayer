using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class UISettings : MonoBehaviour
{
    [SerializeField] private Toggle toggle;
    public Audio music;
    public Audio effects;
    public Audio ambient;
    [SerializeField] private UISwitch[] vSwitches;
    [SerializeField] private Toggle[] vToggles;
    [SerializeField] private Slider[] vSliders;
    [SerializeField] private TMP_Text limitText;
    [SerializeField] private UniversalAdditionalCameraData mainCamera;

    private Resolution[] resolutions;

    private void Awake() {
        LoadAudio();
        LoadVideo();
    }

    public void Back() {
        SaveAudio();
        SaveVideo();
    }

    public void SetMusicVolume(float value) => SetVolume(value, music.sources);

    public void SetEffectsVolume(float value) => SetVolume(value, effects.sources);

    public void SetAmbientVolume(float value) => SetVolume(value, ambient.sources);

    private void SetVolume(float value, List<AudioSource> sources) {
        for (int i = 0; i < sources.Count; i++)
            sources[i].volume = value;
    }

    public void SetMute(bool value) {
        if (value) {
            music.savedVolume = music.slider.value;
            effects.savedVolume = effects.slider.value;
            ambient.savedVolume = ambient.slider.value;
        }

        music.slider.value = value ? 0 : music.savedVolume;
        effects.slider.value = value ? 0 : effects.savedVolume;
        ambient.slider.value = value ? 0 : ambient.savedVolume;
    }

    private void SaveAudio() {
        PlayerPrefs.SetFloat("Music", music.slider.value);
        PlayerPrefs.SetFloat("Effects", effects.slider.value);
        PlayerPrefs.SetFloat("Ambient", ambient.slider.value);
        PlayerPrefs.SetInt("Mute", toggle.isOn ? 1 : 0);
    }

    private void LoadAudio() {
        music.slider.value = PlayerPrefs.GetFloat("Music", 0);
        effects.slider.value = PlayerPrefs.GetFloat("Effects", 0);
        ambient.slider.value = PlayerPrefs.GetFloat("Ambient", 0);
        toggle.isOn = PlayerPrefs.GetInt("Mute", 0) == 1;
    }

    public void SetResolution(int index) => Screen.SetResolution(resolutions[index].width, resolutions[index].height, Screen.fullScreen);

    public void SetWindow(int index) => Screen.fullScreenMode =
        index == 0 ? FullScreenMode.ExclusiveFullScreen :
        index == 1 ? FullScreenMode.FullScreenWindow :
        index == 2 ? FullScreenMode.MaximizedWindow :
        FullScreenMode.Windowed;

    public void SetVertical(bool vertical) {
        if (vertical)
            SetLimit(false);
        QualitySettings.vSyncCount = vertical ? 1 : 0;
    }

    public void SetQuality(float value) => QualitySettings.SetQualityLevel((int)value);

    public void SetTexture(int index) => QualitySettings.globalTextureMipmapLimit = index;

    public void SetAnisotropic(int index) => QualitySettings.anisotropicFiltering =
        index == 0 ? AnisotropicFiltering.Disable :
        index == 1 ? AnisotropicFiltering.Enable :
        AnisotropicFiltering.ForceEnable;

    public void SetAntiAlias(int index) => mainCamera.antialiasing =
        index == 0 ? AntialiasingMode.None :
        index == 1 ? AntialiasingMode.FastApproximateAntialiasing :
        AntialiasingMode.SubpixelMorphologicalAntiAliasing;

    public void SetProbes(bool probes) => QualitySettings.realtimeReflectionProbes = probes;

    public void SetBillboards(bool billboards) => QualitySettings.billboardsFaceCameraPosition = billboards;

    public void SetStreaming(bool streaming) => QualitySettings.streamingMipmapsActive = streaming;

    public void SetShadowmask(int index) => QualitySettings.shadowmaskMode =
        index == 0 ? ShadowmaskMode.Shadowmask :
        ShadowmaskMode.DistanceShadowmask;

    public void SetSkin(int index) => QualitySettings.skinWeights =
        index == 0 ? SkinWeights.OneBone :
        index == 1 ? SkinWeights.TwoBones :
        index == 2 ? SkinWeights.FourBones :
        SkinWeights.Unlimited;

    public void SetProcessing(bool processing) => mainCamera.renderPostProcessing = processing;

    public void SetShadows(bool shadows) => mainCamera.renderShadows = shadows;

    public void SetFOV(float value) => mainCamera.GetComponent<Camera>().fieldOfView = value;

    public void SetDynamic(bool dynamic) => mainCamera.GetComponent<Camera>().allowDynamicResolution = dynamic;

    public void SetLimit(bool limit) {
        if (limit)
            vToggles[0].isOn = false;
        Application.targetFrameRate = limit ? (int)vSliders[2].value : 300;
        vSliders[2].interactable = limit;
    }

    public void SetLimitText(float value) {
        limitText.text = $"Limit FPS [{value}]";
        Application.targetFrameRate = vToggles[7].isOn ? (int)value : 300;
    }

    private void SaveVideo() {
        PlayerPrefs.SetInt("Resolution", vSwitches[0].currentIndex);
        PlayerPrefs.SetInt("Window", vSwitches[1].currentIndex);
        PlayerPrefs.SetInt("Vertical", vToggles[0].isOn ? 1 : 0);
        PlayerPrefs.SetFloat("Quality", vSliders[0].value);
        PlayerPrefs.SetInt("Texture", vSwitches[2].currentIndex);
        PlayerPrefs.SetInt("Anisotropic", vSwitches[3].currentIndex);
        PlayerPrefs.SetInt("AntiAlias", vSwitches[4].currentIndex);
        PlayerPrefs.SetInt("Probes", vToggles[1].isOn ? 1 : 0);
        PlayerPrefs.SetInt("Billboards", vToggles[2].isOn ? 1 : 0);
        PlayerPrefs.SetInt("Streaming", vToggles[3].isOn ? 1 : 0);
        PlayerPrefs.SetInt("Shadowmask", vSwitches[5].currentIndex);
        PlayerPrefs.SetInt("Skin", vSwitches[6].currentIndex);
        PlayerPrefs.SetInt("Processing", vToggles[4].isOn ? 1 : 0);
        PlayerPrefs.SetInt("Shadows", vToggles[5].isOn ? 1 : 0);
        PlayerPrefs.SetFloat("FOV", vSliders[1].value);
        PlayerPrefs.SetInt("Dynamic", vToggles[6].isOn ? 1 : 0);
        PlayerPrefs.SetInt("Limit", vToggles[7].isOn ? 1 : 0);
        PlayerPrefs.SetFloat("LimitAmount", vSliders[2].value);
    }

    private void LoadVideo() {
        GetResolutions();
        vSwitches[0].currentIndex = PlayerPrefs.GetInt("Resolution", vSwitches[0].currentIndex);
        vSwitches[1].currentIndex = PlayerPrefs.GetInt("Window", 0);
        vToggles[0].isOn = PlayerPrefs.GetInt("Vertical", 0) == 1;
        vSliders[0].value = PlayerPrefs.GetFloat("Quality", 3);
        vSwitches[2].currentIndex = PlayerPrefs.GetInt("Texture", 0);
        vSwitches[3].currentIndex = PlayerPrefs.GetInt("Anisotropic", 0);
        vSwitches[4].currentIndex = PlayerPrefs.GetInt("AntiAlias", 0);
        vToggles[1].isOn = PlayerPrefs.GetInt("Probes", 0) == 1;
        vToggles[2].isOn = PlayerPrefs.GetInt("Billboards", 0) == 1;
        vToggles[3].isOn = PlayerPrefs.GetInt("Streaming", 0) == 1;
        vSwitches[5].currentIndex = PlayerPrefs.GetInt("Shadowmask", 0);
        vSwitches[6].currentIndex = PlayerPrefs.GetInt("Skin", 0);
        vToggles[4].isOn = PlayerPrefs.GetInt("Processing", 0) == 1;
        vToggles[5].isOn = PlayerPrefs.GetInt("Shadows", 0) == 1;
        vSliders[1].value = PlayerPrefs.GetFloat("FOV", 60);
        vToggles[6].isOn = PlayerPrefs.GetInt("Dynamic", 0) == 1;
        vToggles[7].isOn = PlayerPrefs.GetInt("Limit", 0) == 1;
        vSliders[2].value = PlayerPrefs.GetFloat("LimitAmount", 60);
    }

    private void GetResolutions() {
        int currentRes = 0;
        resolutions = Screen.resolutions;
        vSwitches[0].ClearOptions();
        List<string> options = new List<string>();
        for (int i = 0; i < resolutions.Length; i++) {
            int iCopy = i;
            options.Add($"{resolutions[i].width} x {resolutions[i].height}");
            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
                currentRes = iCopy;
        }
        vSwitches[0].AddOptions(options);
        vSwitches[0].currentIndex = currentRes;
        vSwitches[0].UpdateSwitch(0);
    }
}