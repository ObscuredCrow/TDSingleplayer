using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float jumpHeight = 1.0f;
    [SerializeField] private float gravity = -9.81f;

    [HideInInspector] public Vector3 targetPosition;

    private CameraController cam;
    private CharacterController cc;
    private Vector3 playerVelocity;
    private bool groundedPlayer;

    private void Start() {
        cc = GetComponent<CharacterController>();
        cam = Camera.main.GetComponent<CameraController>();
        RenderSettings.fog = false;
        cam.target = transform;

        if (!GameManager.Instance.testing)
            GameManager.Instance.npc.playerSpawned = true;
        GameManager.Instance.StartIncome();
    }

    private void Update() => Movement();

    private void Movement() {
        groundedPlayer = cc.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
            playerVelocity.y = 0f;

        Vector3 movement = new Vector3();
        bool forward = Input.GetKey(KeyCode.W);
        bool backward = Input.GetKey(KeyCode.S);
        bool left = Input.GetKey(KeyCode.A);
        bool right = Input.GetKey(KeyCode.D);

        if (forward)
            movement += cam.transform.forward;
        if (backward)
            movement += -cam.transform.forward;
        if (left)
            movement += -cam.transform.right;
        if (right)
            movement += cam.transform.right;

        movement.y = 0;
        float newSpeed = speed;
        if (((forward || backward) && (left || right)) || (left || right))
            newSpeed = speed * 0.6f;
        cc.Move(movement * Time.deltaTime * newSpeed);

        if (movement != Vector3.zero)
            gameObject.transform.forward = movement;

        if (Input.GetKeyDown(KeyCode.Space) && groundedPlayer)
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravity);

        playerVelocity.y += gravity * Time.deltaTime;
        cc.Move(playerVelocity * Time.deltaTime);
    }
}