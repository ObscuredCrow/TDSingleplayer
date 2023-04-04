using UnityEngine;
using UnityEngine.UI;

public class UITooltip : MonoBehaviour
{
    [SerializeField] private bool hasIcon;
    [SerializeField] private Image iconImage;
    public GameObject tooltipPrefab;
    public Scriptables scriptable;

    [HideInInspector] public RectTransform newParent;

    private GameObject activeTooltip;

    public void DisplayTooltip() {
        GameObject go = Instantiate(tooltipPrefab, newParent);
        activeTooltip = go;
        UITooltipDisplay display = go.GetComponent<UITooltipDisplay>();
        display.Create(scriptable.ToolTip());
    }

    private void Start() {
        newParent = GameObject.Find("MainCanvas").GetComponent<RectTransform>();
        if (hasIcon && scriptable.icon != null)
            iconImage.sprite = scriptable.icon;
    }

    public void DestroyTooltip() => Destroy(activeTooltip);
}