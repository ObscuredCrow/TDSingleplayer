using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class UISwitch : MonoBehaviour
{
    [SerializeField] private TMP_Text visualText;
    [SerializeField] private int startIndex;
    [SerializeField] private string[] options;

    [HideInInspector] public int currentIndex;

    public UnityEvent<int> OnValueChange = new UnityEvent<int>();

    private void Start() {
        currentIndex = startIndex;
        UpdateSwitch(0);
    }

    public void GoBack() => UpdateSwitch(-1);

    public void GoForward() => UpdateSwitch(1);

    public void ClearOptions() => options = null;

    public void AddOptions(List<string> list) => options = list.ToArray();

    public void UpdateSwitch(int value) {
        currentIndex += value;
        if (currentIndex > options.Length)
            currentIndex = 0;
        else if (currentIndex < 0)
            currentIndex = options.Length;

        visualText.text = options[currentIndex];
        OnValueChange.Invoke(currentIndex);
    }

    public void UpdateSwitchNoInvoke(int value) {
        currentIndex += value;
        if (currentIndex > options.Length)
            currentIndex = 0;
        else if (currentIndex < 0)
            currentIndex = options.Length;

        visualText.text = options[currentIndex];
    }
}