using System.Collections.Generic;
using UnityEngine;

public class Npc : MonoBehaviour
{
    [SerializeField] private bool tutorial = false;
    [SerializeField] private bool offensive = false;
    [SerializeField] private float actionTime = 2f;
    [SerializeField] private Preview[] buildOrder;
    [SerializeField] private GameObject[] mobPrefabs;

    [HideInInspector] public bool playerSpawned;
    [HideInInspector] public List<Tower> towers;

    private UITutorial uiTutorial;
    private GameManager manager;
    private ScriptableAI[] ais;
    private int buildIndex = 0;
    private int upgradeIndex = 0;

    public void StartGame() {
        ais = new ScriptableAI[mobPrefabs.Length];
        for (int i = 0; i < mobPrefabs.Length; i++)
            ais[i] = mobPrefabs[i].GetComponent<Mob>().ai;

        uiTutorial = FindObjectOfType<UITutorial>();
        manager = GameManager.Instance;
        if (!tutorial)
            InvokeRepeating("PerformActions", actionTime, actionTime);
        else
            uiTutorial.StartTutorial();
    }

    private void PerformActions() {
        if (playerSpawned) {
            Build();
            Upgrade();
            Mobs();
        }
    }

    private void Build() {
        if (buildOrder.Length > 0 && buildIndex < buildOrder.Length) {
            if (manager.player2Points >= buildOrder[buildIndex].tower.PurchaseCost) {
                manager.UsePoints(buildOrder[buildIndex].tower.PurchaseCost, false);
                buildOrder[buildIndex].gameObject.SetActive(true);
                buildIndex++;
            }
        }
    }

    private void Upgrade() {
        if (towers.Count > 0 && upgradeIndex < towers.Count) {
            if (offensive) {
                PurchaseTower(upgradeIndex);
                upgradeIndex++;
            }
            else {
                for (int i = 0; i < towers.Count; i++) {
                    int iCopy = i;
                    PurchaseTower(iCopy);
                }
            }
        }
        else if (upgradeIndex >= towers.Count)
            upgradeIndex = 0;
    }

    private void Mobs() {
        if (mobPrefabs.Length > 0) {
            for (int i = 0; i < mobPrefabs.Length; i++) {
                int iCopy = i;
                if (manager.player2Points >= ais[iCopy].PurchaseCost) {
                    manager.player2Income += ais[iCopy].IncomeIncrease;
                    manager.UsePoints(ais[iCopy].PurchaseCost, false);
                    manager.QueueMob(mobPrefabs[iCopy], false);
                }
            }
        }
    }

    private void PurchaseTower(int iCopy) {
        if (towers[iCopy].buttons.transform.childCount > 1) {
            int upgradeValue = Random.Range(0, towers[iCopy].buttons.transform.childCount - 1);
            if (manager.player2Points >= towers[iCopy].buttons.transform.GetChild(upgradeValue).GetComponent<UIBuild>().ai.PurchaseCost) {
                manager.UsePoints(towers[iCopy].buttons.transform.GetChild(upgradeValue).GetComponent<UIBuild>().ai.PurchaseCost, false);
                towers[iCopy].buttons.transform.GetChild(upgradeValue).GetComponent<UIBuild>().SpawnUpgrade(true);
                towers.RemoveAt(iCopy);
            }
        }
    }
}