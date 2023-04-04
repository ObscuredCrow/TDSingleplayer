using System.Text;
using UnityEngine;

[CreateAssetMenu(menuName = "Crowon Studio/Scriptables/Mob", order = -99)]
public class ScriptableMob : ScriptableAI
{
    public int LifeDamage = 1;
    public bool Invisible = false;
    public bool Flying = false;
    public int SpawnAmount = 0;
    public GameObject SpawnPrefab;

    public override string ToolTip() {
        StringBuilder tip = new StringBuilder(base.ToolTip());
        tip.Replace("{LifeDamage}", LifeDamage.ToString());
        tip.Replace("{Invisible}", (Invisible ? "Yes" : "No"));
        tip.Replace("{Flying}", (Flying ? "Yes" : "No"));
        tip.Replace("{SpawnAmount}", SpawnAmount.ToString());
        tip.Replace("{SpawnPrefab}", SpawnPrefab != null ? SpawnPrefab.name : "");
        return tip.ToString();
    }
}