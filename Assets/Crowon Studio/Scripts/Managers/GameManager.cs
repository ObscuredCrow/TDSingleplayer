using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    public bool testing;
    [SerializeField] private int player1HealthMax = 50;
    [SerializeField] private int player2HealthMax = 50;
    public int player1Income = 20;
    public int player2Income = 20;
    public int player1Points = 30;
    public int player2Points = 30;
    [SerializeField] private Transform[] start1;
    [SerializeField] private Transform[] end1;
    [SerializeField] private Transform[] startFlying1;
    [SerializeField] private Transform[] endFlying1;
    [SerializeField] private Transform[] start2;
    [SerializeField] private Transform[] end2;
    [SerializeField] private Transform[] startFlying2;
    [SerializeField] private Transform[] endFlying2;
    public GameObject[] enemyPrefabs;
    public Transform playerParent;
    [SerializeField] private Transform cameraLocation;
    [SerializeField] private GameObject player2Boundary;

    [HideInInspector] public int enemyDifficulty = 0;
    [HideInInspector] public Npc npc;
    [HideInInspector] public Transform[][] start;
    [HideInInspector] public Transform[][] end;
    [HideInInspector] public List<List<GameObject>> activeMobs = new List<List<GameObject>>();
    [HideInInspector] public List<List<GameObject>> queuedMobs = new List<List<GameObject>>();

    [HideInInspector] public string SteamId;

    private int player1Health;
    private int player2Health;
    private int activeLimit = 60;

    private void Awake() => _instance = _instance ?? this;

    public void StartGame() {
        start = new Transform[][] { start2, start1, startFlying2, startFlying1 };
        end = new Transform[][] { end2, end1, endFlying2, endFlying1 };
        activeMobs.Add(new List<GameObject>());
        activeMobs.Add(new List<GameObject>());
        queuedMobs.Add(new List<GameObject>());
        queuedMobs.Add(new List<GameObject>());

        player1Health = player1HealthMax;
        player2Health = player2HealthMax;
        SetUI();

        player2Boundary.SetActive(!testing);
        if (!testing) {
            GameObject enemy = Instantiate(enemyPrefabs[enemyDifficulty]);
            npc = enemy.GetComponent<Npc>();
            npc.StartGame();
        }

        UIManager.Instance.queue.StartGame();
        InvokeRepeating("MoveMob", 0, 0.25f);
    }

    public void TakeDamage(int value, bool player1) {
        if (!player1)
            player1Health -= value;
        else
            player2Health -= value;

        SetUI();
        if (player1Health <= 0)
            Lose();
        if (player2Health <= 0)
            Win();
    }

    private void GameFinished() {
        UIManager.Instance.mainMenu.panel.SetActive(true);

        UIManager.Instance.queue.EndGame();
        CancelInvoke();

        Destroy(npc.gameObject);
        queuedMobs.Clear();
        activeMobs.Clear();
        Entity[] entities = FindObjectsOfType<Entity>();
        for (int i = 0; i < entities.Length; i++)
            Destroy(entities[i].gameObject);

        Destroy(FindObjectOfType<Player>().gameObject);
        Camera.main.transform.position = cameraLocation.position;
        Camera.main.transform.rotation = cameraLocation.rotation;

        player1HealthMax = 50;
        player2HealthMax = 50;
        player1Income = 20;
        player2Income = 20;
        player1Points = 30;
        player2Points = 30;

        UIManager.Instance.hud.SetActive(false);
    }

    private void Win() {
        Debug.Log("You Win!");
        GameFinished();
    }

    private void Lose() {
        Debug.Log("You Lose!");
        GameFinished();
    }

    private void SetUI() {
        UIManager.Instance.health.yourHealth.text = $"{player1Health}";
        UIManager.Instance.health.theirHealth.text = $"{player2Health}";
        UIManager.Instance.points.yourPoints.text = $"{player1Points} ({player1Income})";
        UIManager.Instance.points.theirPoints.text = $"{player2Points} ({player2Income})";
    }

    public void StartIncome() => InvokeRepeating("ProvideIncome", 10, 10);

    private void ProvideIncome() {
        player1Points += player1Income;
        player2Points += player2Income;
        SetUI();
    }

    public void IncreaseIncome(int value, bool player) {
        if (player)
            player1Income += value;
        else
            player2Income += value;
        SetUI();
    }

    public void GainPoints(int value, bool player) {
        if (player)
            player1Points += value;
        else
            player2Points += value;
        SetUI();
    }

    public void UsePoints(int value, bool player) {
        if (player) { if (player1Points >= value) player1Points -= value; }
        else { if (player2Points >= value) player2Points -= value; }

        SetUI();
        if (player1Points < 0)
            player1Points = 0;
        if (player2Points < 0)
            player2Points = 0;
    }

    private void MoveMob() {
        if (activeMobs[0].Count < activeLimit && queuedMobs[0].Count > 0) {
            SpawnMob(queuedMobs[0][0], false, queuedMobs[0][0].GetComponent<Mob>().flying);
            queuedMobs[0].RemoveAt(0);
        }

        if (activeMobs[1].Count < activeLimit && queuedMobs[1].Count > 0) {
            SpawnMob(queuedMobs[1][0], true, queuedMobs[1][0].GetComponent<Mob>().flying);
            queuedMobs[1].RemoveAt(0);
        }
    }

    public void QueueMob(GameObject prefab, bool isPlayer) {
        if (!isPlayer)
            queuedMobs[0].Add(prefab);
        else
            queuedMobs[1].Add(prefab);
    }

    public int PlayerIndex(bool isPlayer, bool isFlying = false) => isPlayer && isFlying ? 2 : !isPlayer && isFlying ? 3 : isPlayer && !isFlying ? 0 : 1;

    public void SpawnMob(GameObject prefab, bool isPlayer, bool isFlying = false) {
        System.Tuple<GameObject, int, int> spawn = Spawn(prefab, isPlayer, isFlying);
        spawn.Item1.GetComponent<Mob>().target = end[spawn.Item2][spawn.Item3];
        spawn.Item1.GetComponent<Mob>().playerOwned = isPlayer;
        activeMobs[PlayerIndex(!isPlayer)].Add(spawn.Item1);
    }

    private System.Tuple<GameObject, int, int> Spawn(GameObject prefab, bool isPlayer, bool isFlying = false) {
        int pIndex = PlayerIndex(isPlayer, isFlying);
        int location = Random.Range(0, start[PlayerIndex(isPlayer)].Length);
        GameObject go = Instantiate(prefab, start[pIndex][location].position, start[pIndex][location].rotation);
        return new System.Tuple<GameObject, int, int>(go, pIndex, location);
    }
}