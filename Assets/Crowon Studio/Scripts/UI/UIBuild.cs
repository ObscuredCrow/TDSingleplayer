using UnityEngine;

public class UIBuild : MonoBehaviour
{
    public ScriptableAI ai;
    [SerializeField] private GameObject preview;
    [SerializeField] private bool upgrade;

    private bool cantBuild;

    private void Update() {
        if (Input.GetKeyDown(ai.HotKey)) {
            if (upgrade)
                SpawnUpgrade();
            else
                SpawnPreview();
        }
    }

    public void SpawnPreview() {
        cantBuild = false;
        Preview[] previews = FindObjectsByType<Preview>(FindObjectsSortMode.None);
        if (previews.Length > 0)
            for (int i = 0; i < previews.Length; i++)
                if (!previews[i].isBuilding) {
                    cantBuild = true;
                    break;
                }

        if (!cantBuild && GameManager.Instance.player1Points >= preview.GetComponent<Preview>().tower.PurchaseCost)
            Instantiate(preview);
    }

    public void SpawnUpgrade(bool isAi = false) {
        if (GameManager.Instance.player1Points >= preview.GetComponent<Preview>().tower.PurchaseCost || isAi) {
            if (GetComponentInParent<Tower>()) {
                GameObject go = Instantiate(preview);
                Preview upgradePreview = go.GetComponent<Preview>();
                GameObject original = GetComponentInParent<Tower>().gameObject;
                upgradePreview.aiBuilt = isAi;
                upgradePreview.originalTower = original;
                upgradePreview.upgradeLocation = original.transform.position;
                upgradePreview.isUpgrade = true;
            }
        }
    }
}