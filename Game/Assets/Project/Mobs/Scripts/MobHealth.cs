using UnityEngine;

public class MobHealth : MonoBehaviour, HealthBarInterface
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
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        anim.Play("Enemy Hit React");

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
        anim.Play("Mob Death");


        var mobAI = GetComponent<MobAI>();
        if (mobAI != null)
        {
            mobAI.enabled = false;
        }

        var navAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (navAgent != null)
        {
            navAgent.enabled = false;
        }

        if (destroyOnDeath)
        {
            Destroy(gameObject, deathDelay);
        }
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

            // Try to raycast to get exact surface hit point
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
