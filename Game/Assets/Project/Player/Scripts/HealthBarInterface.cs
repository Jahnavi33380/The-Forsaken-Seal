using UnityEngine;

public interface HealthBarInterface
{
    float GetCurrentHealth();
    float GetMaxHealth();
    bool IsDead();
    void TakeDamage(int damage);
    void Heal(int healAmount);
    event System.Action<int, int> OnHealthChanged;

}
