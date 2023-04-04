using UnityEngine;

public class UISend : MonoBehaviour
{
    [SerializeField] private ScriptableAI ai;
    [SerializeField] private GameObject mobPrefab;

    private void Update() {
        if (Input.GetKeyDown(ai.HotKey))
            SpawnMob();
    }

    public void SpawnMob() {
        if (GameManager.Instance.player1Points >= mobPrefab.GetComponent<Mob>().ai.PurchaseCost) {
            GameManager.Instance.player1Income += mobPrefab.GetComponent<Mob>().ai.IncomeIncrease;
            GameManager.Instance.UsePoints(mobPrefab.GetComponent<Mob>().ai.PurchaseCost, true);
            GameManager.Instance.QueueMob(mobPrefab, true);
        }
    }
}