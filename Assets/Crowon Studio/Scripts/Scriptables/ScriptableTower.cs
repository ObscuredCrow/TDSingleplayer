using System.Text;
using UnityEngine;

[CreateAssetMenu(menuName = "Crowon Studio/Scriptables/Tower", order = -99)]
public class ScriptableTower : ScriptableAI
{
    public bool UsesTrapEffect = false;
    public float BuildTime = 2f;

    public override string ToolTip() {
        StringBuilder tip = new StringBuilder(base.ToolTip());
        tip.Replace("{UsesTrapEffect}", (UsesTrapEffect ? "Yes" : "No"));
        tip.Replace("{BuildTime}", BuildTime.ToString());
        return tip.ToString();
    }
}