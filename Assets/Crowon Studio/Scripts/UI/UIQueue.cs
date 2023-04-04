using UnityEngine;

public class UIQueue : MonoBehaviour
{
    public Transform content;
    public GameObject slotPrefab;

    private GameManager manager;

    public void StartGame() {
        manager = GameManager.Instance;
        InvokeRepeating("UpdateQueue", 0, 0.25f);
    }

    public void EndGame() => CancelInvoke();

    private void UpdateQueue() {
        foreach (Transform child in content)
            Destroy(child.gameObject);

        if (manager.queuedMobs[0].Count > 0)
            for (int i = 0; i < manager.queuedMobs[0].Count; i++) {
                GameObject go = Instantiate(slotPrefab, content);
                go.GetComponent<UIQueueSlot>().nameText.text = manager.queuedMobs[0][i].name;
            }
    }
}