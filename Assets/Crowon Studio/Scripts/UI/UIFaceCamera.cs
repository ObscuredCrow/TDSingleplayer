using UnityEngine;

public class UIFaceCamera : MonoBehaviour
{
    private Camera cam;

    private void Start() => cam = Camera.main;

    private void Update() => transform.LookAt(transform.position + cam.transform.rotation * Vector3.forward, cam.transform.rotation * Vector3.up);
}