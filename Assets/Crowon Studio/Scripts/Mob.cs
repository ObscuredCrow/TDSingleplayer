using UnityEngine;
using UnityEngine.AI;

public class Mob : Entity
{
    public bool flying;

    [HideInInspector] public ScriptableMob mob;
    [HideInInspector] public Transform target;
    [HideInInspector] public int counter = 0;
    [HideInInspector] public int shownCount;

    [HideInInspector] public NavMeshAgent agent;
    private Transform originalTarget;

    protected override void Start() {
        mob = ai as ScriptableMob;
        base.Start();
        agent = GetComponent<NavMeshAgent>();
        agent.SetDestination(target.position);
        agent.speed = currentMovementSpeed;
        originalTarget = target;

        InvokeRepeating("HideMob", 0, 1f);
    }

    private void HideMob() {
        if (mob.Invisible) {
            if (shownCount > 0)
                shownCount--;
            else {
                for (int i = 0; i < meshes.Length; i++)
                    meshes[i].enabled = false;

                canvas.gameObject.SetActive(false);
            }
        }
    }

    protected override string GetTag() => "Tower";

    protected override bool IsAI() => true;

    protected override void Attack() {
        if (ai.AttackDamage > 0) {
            bool noTowers = true;
            int multiCounter = 0;
            Collider[] hits = Physics.OverlapSphere(transform.position, GetRadius());
            for (int i = 0; i < hits.Length; i++) {
                if (hits[i].tag == GetTag()) {
                    if (noTowers) {
                        target = hits[i].transform;
                        agent.stoppingDistance = mob.AttackRadius - 2;
                        agent.SetDestination(target.position);
                    }
                    noTowers = false;

                    Entity entity = hits[i].GetComponent<Entity>();
                    if (entity != null) {
                        if (entity.playerOwned == playerOwned)
                            continue;
                        DealDamage(entity);

                        if (ai.UsesMultiEffect && multiCounter < ai.MultiAmount)
                            multiCounter++;
                        else
                            break;
                    }
                }
            }

            if (noTowers && target != originalTarget) {
                target = originalTarget;
                agent.stoppingDistance = 0;
                agent.SetDestination(target.position);
            }
        }
    }

    protected override void SetSlow(float slowPercentage) {
        base.SetSlow(slowPercentage);
        agent.speed = currentMovementSpeed;
    }

    protected override void CheckSlow() {
        base.CheckSlow();
        agent.speed = currentMovementSpeed;
    }

    protected override void OnDeath(bool player) {
        GameManager.Instance.activeMobs[GameManager.Instance.PlayerIndex(player)].Remove(gameObject);
        GameManager.Instance.GainPoints(Mathf.RoundToInt(Mathf.Max(1, (float)mob.IncomeIncrease / 2f)), player);

        if (mob.SpawnAmount > 0 && mob.SpawnPrefab != null) {
            for (int i = 0; i < mob.SpawnAmount; i++) {
                GameObject spawn = Instantiate(mob.SpawnPrefab, transform.position, transform.rotation);
                spawn.GetComponent<Mob>().playerOwned = playerOwned;
                spawn.GetComponent<Mob>().target = target;
                GameManager.Instance.activeMobs[GameManager.Instance.PlayerIndex(!player)].Add(spawn);
            }
        }

        base.OnDeath(player);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "End") {
            GameManager.Instance.TakeDamage(mob.LifeDamage, playerOwned);
            counter = 0;
            int pIndex = GameManager.Instance.PlayerIndex(playerOwned, mob.Flying);
            int location = Random.Range(0, GameManager.Instance.start[GameManager.Instance.PlayerIndex(playerOwned)].Length);
            agent.Warp(GameManager.Instance.start[pIndex][location].position);
            agent.SetDestination(GameManager.Instance.end[pIndex][location].position);
            originalTarget = target = GameManager.Instance.end[pIndex][location];
        }
    }
}