using UnityEngine;

public class Powerpickup : MonoBehaviour
{
    [Header("Upgrade Values")]
    [SerializeField] private int newMaxHealth = 250;
    [SerializeField] private int newCurrentHealth = 250;
    [SerializeField] private int newWeaponDamage = 25;

    [Header("Pickup Settings")]
    [SerializeField] private bool destroyOnPickup = true;

    private void OnTriggerEnter(Collider other)
    {
        // Only the player should pick this up
        if (!other.CompareTag("Player")) return;

        // Get PlayerHealth
        PlayerHealth health = other.GetComponent<PlayerHealth>();
        if (health == null) health = other.GetComponentInParent<PlayerHealth>();
        if (health != null)
        {
            // update max and current health
            health.ApplyFullHeal(newMaxHealth);
        }

        // Get PlayerWeapon
        PlayerWeapon weapon = other.GetComponentInChildren<PlayerWeapon>();
        if (weapon != null)
        {
            weapon.SetBaseDamage(newWeaponDamage);
        }

        // Destroy pickup
        if (destroyOnPickup)
            Destroy(gameObject);
    }
}
