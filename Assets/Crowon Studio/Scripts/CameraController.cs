using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Vector3 offset;
    [SerializeField] private float turnSpeed;

    [HideInInspector] public Transform target;

    private void FixedUpdate() {
        if (target != null) {
            if (Input.GetKey(KeyCode.Mouse1))
                offset = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * turnSpeed, Vector3.up) * offset;

            transform.position = target.position + offset;
            transform.LookAt(target);
        }
    }
}