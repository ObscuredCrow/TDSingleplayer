using UnityEngine;

public class Tower : Entity
{
    public GameObject buttons;

    [HideInInspector] public ScriptableTower tower;

    protected override void Start() {
        tower = ai as ScriptableTower;
        base.Start();
        Invoke("BlockCheck", 0.5f);
    }

    protected override void Update() {
        if ((Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1)) && playerOwned)
            buttons.SetActive(false);

        if (Input.GetKeyDown(KeyCode.X) && buttons.activeSelf)
            DestroySelf();

        base.Update();
    }

    private void BlockCheck() {
        Mob[] checkers = FindObjectsOfType<Mob>();
        for (int i = 0; i < checkers.Length; i++)
            if (checkers[i].playerOwned != playerOwned) {
                if (checkers[i].agent.pathStatus != UnityEngine.AI.NavMeshPathStatus.PathComplete)
                    Destroy(gameObject);

                break;
            }
    }

    private void OnMouseDown() => buttons.SetActive(playerOwned);

    public void DestroySelf() {
        GameManager.Instance.GainPoints(ai.SellCost, playerOwned);
        Destroy(gameObject);
    }

    protected override string GetTag() => "Mob";

    protected override void Attack() {
        if (!tower.UsesTrapEffect)
            base.Attack();
    }

    protected override void Aura() {
        base.Aura();
        if (ai.UsesAuraEffect) {
            Collider[] hits = Physics.OverlapSphere(transform.position, GetRadius());
            for (int i = 0; i < hits.Length; i++) {
                if (hits[i].GetComponent<Mob>() && ai.AuraRemovesInvisibility) {
                    Mob mob = hits[i].GetComponent<Mob>();
                    if (mob.playerOwned == playerOwned)
                        continue;
                    if (!mob.meshes[0].enabled) {
                        mob.shownCount = 5;
                        for (int x = 0; x < mob.meshes.Length; x++)
                            mob.meshes[x].enabled = true;

                        mob.canvas.gameObject.SetActive(true);
                    }
                }
            }
        }
    }

    protected override void OnDeath(bool player) {
        if (IsAI())
            GameManager.Instance.npc.towers.Remove(this);

        base.OnDeath(player);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == GetTag() && tower.UsesTrapEffect)
            DealDamage(other.GetComponent<Mob>());
    }
}