using UnityEngine;

public class UITier : MonoBehaviour
{
    [SerializeField] private int nextTier = 0;
    [SerializeField] private GameObject[] tierButtons;

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Tab))
            ChangeTier();
    }

    public void ChangeTier() {
        for (int i = 0; i < tierButtons.Length; i++)
            tierButtons[i].SetActive(false);

        tierButtons[nextTier].SetActive(true);
    }
}