using UnityEngine;

public class BossBullKingHealth : MonoBehaviour, HealthBarInterface
{
    [Header("Health Settings")]
    [SerializeField] public int maxHealth = 100;
    [SerializeField] public int currentHealth;

    [Header("Death Settings")]
    [SerializeField] private bool destroyOnDeath = true;
    [SerializeField] private float deathDelay = 2f;

    public event System.Action<int, int> OnHealthChanged;
    public System.Action OnDeath;

    private bool isDead = false;
    public Animator anim;
    public GameObject bossHealthBar;
    public GameObject bossLockdown;

    void Start()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

    }

    // Implementing HealthBar Interface
    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
    public bool IsDead() => isDead;

    public void TakeDamage(int damage)
    {
        BossBullKingAI bossAI = GetComponent<BossBullKingAI>();
        if (bossAI != null && bossAI.isDoingAttack)
        {
            // Ignores damage and continues to animate
            return;
        }
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        anim.Play("BossHitReact");

        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }

    public void Heal(int healAmount)
    {
        if (isDead) return;

        currentHealth += healAmount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void SetMaxHealth(int newMaxHealth)
    {
        maxHealth = newMaxHealth;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void Die()
    {
        isDead = true;
        OnDeath?.Invoke();
        anim.Play("BossDeath");
        Level1KalshiBanner.Show("LEVEL I COMPLETED");

        var mobAI = GetComponent<MobAI>();
        if (mobAI != null) mobAI.enabled = false;

        var navAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (navAgent != null) navAgent.enabled = false;

        if (destroyOnDeath)
        {
            Destroy(gameObject, deathDelay);
        }

        // disable boss health UI
        if (bossHealthBar != null)
            bossHealthBar.SetActive(false);
        if (bossLockdown != null)
            bossLockdown.SetActive(false);
    }

    [Header("Blood Effect")]

    public ParticleSystem bloodEffect;

    [Header("Attack Audio")]
    public AudioSource source;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Weapon"))
        {
            Debug.Log("takeDamageTrigger");

            Vector3 weaponPos = other.transform.position;
            Vector3 dirToEnemy = (transform.position - weaponPos).normalized;

            Vector3 hitPoint = transform.position;
            Vector3 hitNormal = -dirToEnemy;


            RaycastHit hit;
            if (Physics.Raycast(weaponPos, dirToEnemy, out hit, 20f))
            {
                hitPoint = hit.point;
                hitNormal = hit.normal;
            }

            PlayerWeapon weapon = other.GetComponentInParent<PlayerWeapon>();
            if (weapon != null)
            {
                // APPLY DAMAGE FROM PLAYER WEAPON
                Debug.Log(weapon.GetDamage());
                TakeDamage(weapon.GetDamage());
            }
            else
            {
                Debug.LogWarning("Weapon hit but PlayerWeapon script not found!");
            }

            SpawnBlood(hitPoint, hitNormal);

        }
    }


    private void SpawnBlood(Vector3 pos, Vector3 normal)
    {
        if (bloodEffect == null) return;

        Quaternion rot = Quaternion.LookRotation(normal);
        ParticleSystem ps = Instantiate(bloodEffect, pos, rot);
        ps.Play();
        source.Play();

        // Destroy after lifetime
        Destroy(ps.gameObject, ps.main.duration + 0.5f);
    }
}
