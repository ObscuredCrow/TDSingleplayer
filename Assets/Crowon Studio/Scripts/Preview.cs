using UnityEngine;
using UnityEngine.UI;

public class Preview : MonoBehaviour
{
    public ScriptableTower tower;
    [SerializeField] private GameObject buildPrefab;
    public bool aiBuilt;
    [SerializeField] private Canvas canvas;
    [SerializeField] private Image progressBar;
    [SerializeField] private Material good;
    [SerializeField] private Material bad;
    [SerializeField] private Collider col;

    [HideInInspector] public bool isBuilding;
    [HideInInspector] public bool isUpgrade;
    [HideInInspector] public Vector3 upgradeLocation;
    [HideInInspector] public GameObject originalTower;

    private AudioSource audio;
    private RaycastHit hit;
    private Grid grid;
    private MeshRenderer mesh;
    private bool cantBuild;
    private bool followMouse = true;
    private float buildProgress = 0;
    private bool upgradeStarted;

    private void Start() {
        audio = GetComponent<AudioSource>();
        canvas.worldCamera = Camera.main;
        grid = FindObjectOfType<Grid>();

        for (int i = 0; i < transform.childCount; i++)
            if (transform.GetChild(i).name.Contains("Cube")) {
                mesh = transform.GetChild(i).GetComponent<MeshRenderer>();
                break;
            }

        if (aiBuilt)
            StartBuilding();
    }

    private void Update() {
        if (!isUpgrade) {
            if (!aiBuilt)
                FollowMouse();
            else
                SetupAITower();

            if (Input.GetMouseButtonDown(0) && !cantBuild && followMouse && GameManager.Instance.player1Points >= tower.PurchaseCost) {
                if (tower.IncomeIncrease > 0)
                    GameManager.Instance.IncreaseIncome(tower.IncomeIncrease, !aiBuilt);
                GameManager.Instance.UsePoints(tower.PurchaseCost, !aiBuilt);
                StartBuilding();
            }

            if (Input.GetMouseButton(1) && !isBuilding)
                Destroy(gameObject);
        }
        else if (!upgradeStarted) {
            var finalPosition = grid.GetNearestPointOnGrid(upgradeLocation);
            transform.position = finalPosition;
            GameManager.Instance.UsePoints(tower.PurchaseCost, !aiBuilt);
            StartBuilding();
            upgradeStarted = true;
        }
    }

    private void FollowMouse() {
        if (followMouse) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 50000.0f, LayerMask.NameToLayer("TransparentFX"))) {
                var finalPosition = grid.GetNearestPointOnGrid(hit.point);
                transform.position = finalPosition;
            }
        }
    }

    private void SetupAITower() {
        if (!GetComponent<Rigidbody>()) {
            gameObject.AddComponent<Rigidbody>();
            Rigidbody rigid = GetComponent<Rigidbody>();
            rigid.isKinematic = true;
            rigid.useGravity = false;
            gameObject.tag = "Tower";
        }
        mesh.material = good;
    }

    private void StartBuilding() {
        if (isUpgrade)
            Destroy(originalTower);
        canvas.gameObject.SetActive(true);
        followMouse = false;
        isBuilding = true;
        audio.Play();
        InvokeRepeating("Build", 0.1f, 0.1f);
    }

    private void Build() {
        if (buildProgress < 1)
            buildProgress += 0.1f / tower.BuildTime;
        else {
            GameObject go = Instantiate(buildPrefab, transform.position, transform.rotation);
            Tower builtTower = go.GetComponent<Tower>();
            builtTower.playerOwned = !aiBuilt;
            if (aiBuilt)
                GameManager.Instance.npc.towers.Add(builtTower);
            Destroy(gameObject);
        }
        progressBar.fillAmount = buildProgress;
    }

    private void OnTriggerEnter(Collider other) => CheckBuildArea(other, true, bad);

    private void OnTriggerStay(Collider other) => CheckBuildArea(other, true, bad);

    private void OnTriggerExit(Collider other) => CheckBuildArea(other, false, good);

    private void CheckBuildArea(Collider other, bool value, Material mat) {
        if (other != null && mat != null && mesh != null) {
            if (other != col && other.attachedRigidbody != col.attachedRigidbody)
                if (other.tag == "Block" || other.tag == "Tower" || other.tag == "Preview" || (other.tag == "Mob" && !tower.UsesTrapEffect)) {
                    mesh.material = mat;
                    cantBuild = value;
                }
        }
    }
}