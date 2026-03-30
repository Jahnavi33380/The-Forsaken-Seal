using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    [Header("Weapon Damage Stats")]
    [SerializeField] private int baseDamage = 10;

    [Tooltip("Flat bonus damage added from upgrades, items, etc.")]
    [SerializeField] private int bonusDamage = 0;

    // Final damage = base + bonus
    public int GetDamage()
    {
        return baseDamage + bonusDamage;
    }

    // Upgrade methods
    public void AddFlatBonus(int amount) => bonusDamage += amount;
    public void SetBaseDamage(int amount) => baseDamage = amount;

    // Optional read-only properties
    public int BaseDamage => baseDamage;
    public int BonusDamage => bonusDamage;
}
