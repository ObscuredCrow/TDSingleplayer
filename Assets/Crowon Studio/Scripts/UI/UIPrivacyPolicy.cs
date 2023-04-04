using UnityEngine;

public class UIPrivacyPolicy : MonoBehaviour
{
    [SerializeField] private string url = "https://www.google.com/";
    [SerializeField] private GameObject popup;

    public void Accepted() {
        PlayerPrefs.SetInt("PrivacyPolicy", 1);
        UpdatePolicy();
    }

    public void UpdatePolicy() => popup.SetActive(PlayerPrefs.GetInt("PrivacyPolicy", 0) == 0);

    public bool PolicyAccepted() => PlayerPrefs.GetInt("PrivacyPolicy", 0) == 1;

    public void Website() => System.Diagnostics.Process.Start(url);
}