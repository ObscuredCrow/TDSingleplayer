using UnityEngine;
using UnityEngine.UI;

public abstract class Entity : MonoBehaviour
{
    public ScriptableAI ai;
    public Canvas canvas;
    [SerializeField] protected Image healthBar;
    [SerializeField] protected Transform projectileParent;

    [HideInInspector] public AudioSource audio;
    [HideInInspector] public Animator animator;
    [HideInInspector] public SkinnedMeshRenderer[] meshes;
    [HideInInspector] public bool playerOwned;
    [HideInInspector] public int bonusDamage;
    [HideInInspector] public float bonusRadius;
    [HideInInspector] public float bonusSpeed;
    [HideInInspector] public int bonusDefense;

    protected int health = 100;
    protected float currentAttackSpeed;
    protected float currentMovementSpeed;
    protected int slowCount;
    protected int dotCount;
    protected int dotAmount;
    protected int stunCount;
    protected Weapon attackType;

    protected virtual void Start() {
        animator = GetComponentInChildren<Animator>();
        canvas.worldCamera = Camera.main;
        meshes = GetComponentsInChildren<SkinnedMeshRenderer>();
        health = ai.HealthMax;
        currentAttackSpeed = GetAttackSpeed();
        currentMovementSpeed = GetMovementSpeed();
        audio = GetComponent<AudioSource>();

        if (ai.spawnClip != null) {
            audio.clip = ai.spawnClip;
            audio.Play();
        }

        InvokeRepeating("Attack", currentAttackSpeed, currentAttackSpeed);
        InvokeRepeating("Aura", currentAttackSpeed, currentAttackSpeed);
        InvokeRepeating("CheckSlow", 0, 1f);
        InvokeRepeating("CheckDoT", 0, 1f);
        InvokeRepeating("CheckStun", 0, 1f);
        InvokeRepeating("Regeneration", 1f, 1f);
    }

    protected virtual void Update() {
        if (currentAttackSpeed != GetAttackSpeed()) {
            currentAttackSpeed = GetAttackSpeed();
            CancelInvoke();
            InvokeRepeating("Attack", currentAttackSpeed, currentAttackSpeed);
            InvokeRepeating("Aura", currentAttackSpeed, currentAttackSpeed);
        }
    }

    protected virtual void Regeneration() {
        if (health < ai.HealthMax)
            health += ai.HealthRegen;

        if (health > ai.HealthMax)
            health = ai.HealthMax;
    }

    private void LateUpdate() => animator.SetInteger("Movement", GetTag() == "Tower" ? 1 : 0);

    protected virtual int GetDamage() => ai.AttackDamage + bonusDamage;

    protected virtual float GetRadius() => ai.AttackRadius + bonusRadius;

    protected virtual float GetAttackSpeed() => ai.AttackSpeed * (float)(1 - bonusSpeed);

    protected virtual float GetMovementSpeed() => ai.MovementSpeed * (float)(1 - bonusSpeed);

    protected virtual int GetDefense() => ai.Defense + bonusDefense;

    protected virtual string GetTag() => "";

    protected virtual bool IsAI() => GetTag() == "Tower";

    protected virtual void Attack() {
        if (!ai.UsesAuraEffect || ai.AuraCanAttack) {
            int multiCounter = 0;
            Collider[] hits = Physics.OverlapSphere(transform.position, GetRadius());
            for (int i = 0; i < hits.Length; i++) {
                if (hits[i].tag == GetTag()) {
                    Entity entity = hits[i].GetComponent<Entity>();
                    if (entity != null) {
                        if (entity.playerOwned == playerOwned)
                            continue;
                        if (CanAttack(entity.ai.UnitType)) {
                            animator.CrossFade("Attack", 0.2f);
                            DealDamage(entity);

                            if (ai.UsesMultiEffect && multiCounter < ai.MultiAmount)
                                multiCounter++;
                            else
                                break;
                        }
                    }
                }
            }
        }
    }

    protected virtual void DealDamage(Entity entity) {
        if (ai.attackClip != null) {
            audio.clip = ai.attackClip;
            audio.Play();
        }

        if (ai.ProjectilePrefab != null)
            ProjectileDamage(entity.transform);
        else {
            if (ai.UsesSlowEffect)
                entity.TakeDamage(GetDamage(), playerOwned, ai.WeaponType, ai.SlowAmount);
            else if (ai.UsesDotEffect)
                entity.TakeDamage(GetDamage(), playerOwned, ai.WeaponType, 0, ai.DotAmount);
            else if (ai.UsesStunEffect)
                entity.TakeDamage(GetDamage(), playerOwned, ai.WeaponType, 0, 0, ai.StunAmount);
            else
                entity.TakeDamage(GetDamage(), playerOwned, ai.WeaponType);

            if (ai.UsesSplashEffect)
                SplashDamage(entity.transform);
        }
    }

    protected virtual void ProjectileDamage(Transform target) {
        Vector3 position = transform.position;
        position.y += 0.5f;
        GameObject go = Instantiate(ai.ProjectilePrefab, position, transform.rotation);
        Projectile projectile = go.GetComponent<Projectile>();
        projectile.weaponType = ai.WeaponType;
        projectile.damage = GetDamage();
        projectile.usesSplashEffect = ai.UsesSplashEffect;
        projectile.splashAmount = ai.SplashAmount;
        projectile.splashRadius = ai.SplashRadius;
        projectile.usesSlowEffect = ai.UsesSlowEffect;
        projectile.slowAmount = ai.SlowAmount;
        projectile.playerOwned = playerOwned;
        projectile.npcAttack = IsAI();
        projectile.target = target;
        projectile.targetInit = true;
    }

    protected virtual void SplashDamage(Transform target) {
        Collider[] hits = Physics.OverlapSphere(target.position, ai.SplashRadius);
        for (int x = 0; x < hits.Length; x++)
            if (hits[x].tag == GetTag()) {
                Entity entity = hits[x].GetComponent<Entity>();
                if (entity.playerOwned == playerOwned)
                    continue;
                else if (ai.UsesSlowEffect)
                    entity.TakeDamage((int)((float)GetDamage() * ai.SplashAmount), playerOwned, ai.WeaponType, ai.SlowAmount);
                else
                    entity.TakeDamage((int)((float)GetDamage() * ai.SplashAmount), playerOwned, ai.WeaponType);
            }
    }

    public virtual void TakeDamage(int value, bool player, Weapon weaponType, float slowPercentage = 0, int dotAmount = 0, float stunChance = 0) {
        if (meshes[0].enabled && Random.Range(0.01f, 1f) > ai.Evasion) {
            if (weaponType == Weapon.Magic)
                health -= Mathf.RoundToInt(Mathf.Max(1, ((((float)value * CalculateDamagePercentage(ai.ArmorType, weaponType)) - GetDefense()) * (float)(1 - ai.DamageReduction)) * (float)(1 - ai.SpellReduction)));
            else
                health -= Mathf.RoundToInt(Mathf.Max(1, (((float)value * CalculateDamagePercentage(ai.ArmorType, weaponType)) - GetDefense()) * (float)(1 - ai.DamageReduction)));
            healthBar.fillAmount = (float)health / (float)ai.HealthMax;
            if (slowPercentage > 0 && !ai.SlowImmune)
                SetSlow(slowPercentage);
            if (dotAmount > 0 && !ai.DotImmune)
                SetDoT(dotAmount, weaponType);
            if (stunChance > 0 && !ai.StunImmune)
                SetStun(stunChance);
            if (health <= 0)
                OnDeath(player);
        }
    }

    protected virtual void OnDeath(bool player) => Destroy(gameObject);

    protected virtual void SetSlow(float slowPercentage) {
        slowCount = 10;
        currentAttackSpeed = GetAttackSpeed() * slowPercentage;
        currentMovementSpeed = GetMovementSpeed() * slowPercentage;
    }

    protected virtual void CheckSlow() {
        if (slowCount > 0)
            slowCount--;
        else {
            currentAttackSpeed = GetAttackSpeed();
            currentMovementSpeed = GetMovementSpeed();
        }
    }

    protected virtual void SetDoT(int amount, Weapon weaponType) {
        dotCount = 5;
        dotAmount = amount;
        attackType = weaponType;
    }

    protected virtual void CheckDoT() {
        if (dotCount > 0) {
            dotCount--;
            TakeDamage(dotAmount, !playerOwned, attackType);
        }
    }

    protected virtual void SetStun(float stunChance) {
        if (Random.Range(0f, 1f) < stunChance) {
            stunCount = 5;
            currentAttackSpeed = 0;
            currentMovementSpeed = 0;
        }
    }

    protected virtual void CheckStun() {
        if (stunCount > 0)
            stunCount--;
        else {
            currentAttackSpeed = GetAttackSpeed();
            currentMovementSpeed = GetMovementSpeed();
        }
    }

    protected virtual void Aura() {
        if (ai.UsesAuraEffect) {
            Collider[] hits = Physics.OverlapSphere(transform.position, GetRadius());
            for (int i = 0; i < hits.Length; i++)
                if (hits[i].tag == gameObject.tag && hits[i].GetComponent<Entity>() != this) {
                    Entity targetEntity = hits[i].GetComponent<Entity>();
                    if (targetEntity != null) {
                        targetEntity.bonusDefense = ai.AuraDefenseIncrease;
                        targetEntity.bonusDamage = ai.AuraDamageIncrease;
                        targetEntity.bonusRadius = ai.AuraRadiusIncrease;
                        targetEntity.bonusSpeed = ai.AuraSpeedIncrease;
                        targetEntity.health += ai.AuraHealingAmount;
                        if (targetEntity.health > targetEntity.ai.HealthMax)
                            targetEntity.health = targetEntity.ai.HealthMax;
                    }
                }
        }
    }

    protected virtual void OnDestroy() {
        if (ai.UsesAuraEffect) {
            Collider[] hits = Physics.OverlapSphere(transform.position, GetRadius());
            for (int i = 0; i < hits.Length; i++)
                if (hits[i].tag == gameObject.tag && hits[i].GetComponent<Entity>() != this) {
                    Entity targetEntity = hits[i].GetComponent<Entity>();
                    if (targetEntity != null) {
                        targetEntity.bonusDefense = 0;
                        targetEntity.bonusDamage = 0;
                        targetEntity.bonusRadius = 0;
                        targetEntity.bonusSpeed = 1;
                    }
                }
        }
    }

    protected virtual float CalculateDamagePercentage(Armor armor, Weapon weapon) {
        float[][] percentages = new float[6][];
        // Normal, Piercing, Magic, Siege, Hero, Chaos
        percentages[0] = new float[] { 1.00f, 1.50f, 1.00f, 1.50f, 1.00f, 1.00f }; // Unarmored
        percentages[1] = new float[] { 1.00f, 2.00f, 1.25f, 1.00f, 1.00f, 1.00f }; // Light
        percentages[2] = new float[] { 1.50f, 0.75f, 0.75f, 0.50f, 1.00f, 1.00f }; // Medium
        percentages[3] = new float[] { 1.00f, 1.00f, 2.00f, 1.00f, 1.00f, 1.00f }; // Heavy
        percentages[4] = new float[] { 0.70f, 0.35f, 0.35f, 1.50f, 0.50f, 1.00f }; // Fortified
        percentages[5] = new float[] { 1.00f, 0.50f, 0.50f, 0.50f, 1.00f, 1.00f }; // Hero

        return percentages[(int)armor][(int)weapon];
    }

    protected virtual bool CanAttack(Unit unitType) {
        switch (unitType) {
            case Unit.Ground:
                return ai.UnitType == Unit.Ground || ai.UnitType == Unit.Both;

            case Unit.Air:
                return ai.UnitType == Unit.Air || ai.UnitType == Unit.Both;

            case Unit.Both:
                return true;
        }

        return false;
    }
}

public enum Armor
{ Unarmored, Light, Medium, Heavy, Fortified, Hero }

public enum Weapon
{ Normal, Piercing, Magic, Siege, Hero, Chaos }

public enum Unit
{
    Ground, Air, Both
}