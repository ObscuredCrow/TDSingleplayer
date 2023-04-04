//using Steamworks;
using UnityEngine;
using UnityEngine.UI;

public class UIMainMenu : MonoBehaviour
{
    public GameObject panel;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform startLocation;
    [SerializeField] private Toggle[] radioButtons;

    private void Start() {
        GameManager.Instance.SteamId = "12345678901234657";// SteamUser.GetSteamID().ToString();
    }

    public void Play() {
        if (UIManager.Instance.policy.PolicyAccepted()) {
            UIManager.Instance.hud.SetActive(true);
            GameObject go = Instantiate(playerPrefab, startLocation.position, startLocation.rotation, GameManager.Instance.playerParent);
            Player goPlayer = go.GetComponent<Player>();
            goPlayer.name = GameManager.Instance.SteamId;
            GameManager.Instance.StartGame();
            panel.SetActive(false);
        }
        else
            UIManager.Instance.policy.UpdatePolicy();
    }

    public void ChangeDifficulty(int value) {
        //for (int i = 0; i < radioButtons.Length; i++)
        //    radioButtons[i].isOn = false;

        //radioButtons[value].isOn = true;
        GameManager.Instance.enemyDifficulty = value;
    }

    public void Quit() => Application.Quit();
}