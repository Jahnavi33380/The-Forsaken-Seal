using UnityEngine;
using TMPro;

public class PlayerStatusUI : MonoBehaviour
{
    [Header("Text References")]
    [SerializeField] private TextMeshProUGUI maxHealthText;
    [SerializeField] private TextMeshProUGUI weaponDamageText;

    [Header("References")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private PlayerWeapon playerWeapon;

    void Start()
    {
        if (playerHealth == null)
            playerHealth = FindObjectOfType<PlayerHealth>();

        if (playerWeapon == null)
            playerWeapon = FindObjectOfType<PlayerWeapon>();
    }

    void Update()
    {
        if (playerHealth != null)
        {
            maxHealthText.text = "Max HP: " + playerHealth.GetMaxHealth().ToString();
        }

        if (playerWeapon != null)
        {
            weaponDamageText.text = "Weapon Damage: " + playerWeapon.GetDamage().ToString();
        }
    }
}
