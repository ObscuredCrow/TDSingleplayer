using UnityEngine;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    public static UIManager Instance { get { return _instance; } }

    public UIMainMenu mainMenu;
    public UIPrivacyPolicy policy;
    public UISettings settings;
    public UIHealth health;
    public UIPoints points;
    public UIQueue queue;
    public GameObject hud;

    private void Awake() => _instance = _instance ?? this;
}