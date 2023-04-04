using TMPro;
using UnityEngine;

public class UITooltipDisplay : MonoBehaviour
{
    public TMP_Text text;

    private Camera cam;
    private Vector3 min, max;
    private RectTransform rect;
    private float offset = 40f;

    private void Start() {
        cam = Camera.main;
        rect = GetComponent<RectTransform>();
        min = new Vector3(0, 0, 0);
        max = new Vector3(cam.pixelWidth, cam.pixelHeight, 0);
    }

    private void Update() {
        if (gameObject.activeSelf) {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
                Destroy(gameObject);
            Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y - offset, 0f);
            transform.position = new Vector3(Mathf.Clamp(position.x, min.x + rect.rect.width / 2, max.x - rect.rect.width / 2), Mathf.Clamp(position.y, min.y + rect.rect.height / 2, max.y - rect.rect.height / 2), transform.position.z);
        }
    }

    public void Create(string tooltip) => text.text = tooltip;
}