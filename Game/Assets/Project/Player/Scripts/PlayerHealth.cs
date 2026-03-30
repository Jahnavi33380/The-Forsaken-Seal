using UnityEngine;
using TMPro;

public class PlayerHealth : MonoBehaviour, HealthBarInterface
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Death Settings")]
    public float respawnDelay = 0.7f;
    public Vector3 respawnPosition;

    private bool isDead = false;

    // Event sends: (currentHealth, maxHealth)
    public event System.Action<int, int> OnHealthChanged;
    public System.Action OnPlayerDeath;

    public Animator anim;
    [Header("Attack Audio")]
    public AudioSource source;


    public HUDController hud;

    void Start()
    {
        currentHealth = maxHealth;

        if (respawnPosition == Vector3.zero)
            respawnPosition = transform.position;

        // Notify listeners at start
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    //HealthBarInterface
    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
    public bool IsDead() => isDead;


    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth = Mathf.Max(0, currentHealth - damage);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (anim != null)
        {
            anim.Play("Hit React");
            source.Play();
        }

        Debug.Log($"Player took {damage} damage. Health: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }

    }

    public void Heal(int healAmount)
    {
        // if (isDead) return;

        // currentHealth = Mathf.Min(maxHealth, currentHealth + healAmount);
        // OnHealthChanged?.Invoke(currentHealth, maxHealth);

        // Debug.Log($"Player healed {healAmount}. Health: {currentHealth}/{maxHealth}");
    }
    public void ApplyFullHeal(int amount)
    {
        maxHealth = amount;
        currentHealth = amount;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    void Die()
    {
        isDead = true;

        var controller = GetComponent<ControllerPerson>();
        if (controller != null)
            controller.enabled = false;

        OnPlayerDeath?.Invoke();


        Debug.Log("Player died.");

        //Destroy(gameObject, respawnDelay);
        anim.Play("Player Death");

        hud.ShowDeathScreen();  //death screen jahnavi
    }

}
