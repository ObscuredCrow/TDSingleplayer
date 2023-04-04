using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    [HideInInspector] public Weapon weaponType;
    [HideInInspector] public bool usesSplashEffect;
    [HideInInspector] public float splashAmount = 0.5f;
    [HideInInspector] public float splashRadius = 5;
    [HideInInspector] public bool usesSlowEffect;
    [HideInInspector] public float slowAmount = 0.5f;
    [HideInInspector] public bool usesDotEffect;
    [HideInInspector] public float dotAmount = 0.5f;
    [HideInInspector] public bool usesStunEffect;
    [HideInInspector] public float stunAmount = 0.5f;
    [HideInInspector] public int damage;
    [HideInInspector] public bool playerOwned;
    [HideInInspector] public Transform target;
    [HideInInspector] public bool targetInit;
    [HideInInspector] public bool npcAttack;

    private void Update() {
        if (target != null && targetInit)
            transform.position = Vector3.Lerp(transform.position, target.position, speed * Time.deltaTime);
        else if (targetInit)
            Destroy(gameObject);
    }

    private string GetTag() => npcAttack ? "Tower" : "Mob";

    protected virtual void SplashDamage(Transform target) {
        Collider[] hits = Physics.OverlapSphere(target.position, splashRadius);
        for (int x = 0; x < hits.Length; x++)
            if (hits[x].tag == GetTag()) {
                Entity entity = hits[x].GetComponent<Entity>();
                if (entity.playerOwned == playerOwned)
                    continue;
                else if (usesSlowEffect)
                    entity.TakeDamage((int)((float)damage * splashAmount), playerOwned, weaponType, slowAmount);
                else
                    entity.TakeDamage((int)((float)damage * splashAmount), playerOwned, weaponType);
            }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == GetTag()) {
            if (other.GetComponent<Entity>()) {
                Entity entity = other.GetComponent<Entity>();
                if (usesSlowEffect)
                    entity.TakeDamage(damage, playerOwned, weaponType, slowAmount);
                else if (usesDotEffect)
                    entity.TakeDamage(damage, playerOwned, weaponType, dotAmount);
                else if (usesStunEffect)
                    entity.TakeDamage(damage, playerOwned, weaponType, 0, 0, stunAmount);
                else
                    entity.TakeDamage(damage, playerOwned, weaponType);

                if (usesSplashEffect)
                    SplashDamage(entity.transform);
                Destroy(gameObject);
            }
        }
    }
}