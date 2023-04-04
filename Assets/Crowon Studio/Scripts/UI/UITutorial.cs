using UnityEngine;

public class UITutorial : MonoBehaviour
{
    [SerializeField] private GameObject[] popups;

    private int tutorialIndex = 0;

    public void StartTutorial() => popups[tutorialIndex].SetActive(true);

    public void NextTutorial() {
        popups[tutorialIndex].SetActive(false);

        tutorialIndex++;
        if (tutorialIndex < popups.Length)
            popups[tutorialIndex].SetActive(true);
    }

    public void FinishTutorial() {
        for (int i = 0; i < popups.Length; i++)
            popups[i].SetActive(false);

        tutorialIndex = 0;
        GameManager.Instance.TakeDamage(50, true);
    }
}