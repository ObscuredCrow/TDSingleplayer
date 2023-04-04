using System.Text;
using UnityEngine;

[CreateAssetMenu(menuName = "Crowon Studio/Scriptables/Scriptable", order = -99)]
public class Scriptables : ScriptableObject
{
    [SerializeField, TextArea(1, 30)] protected string toolTip;
    public Sprite icon;

    public virtual string ToolTip() {
        StringBuilder tip = new StringBuilder(toolTip);
        tip.Replace("{Name}", name);
        return tip.ToString();
    }
}