using UnityEngine;

public class BossBullKingWeapon : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] private int smallSwingDamage = 1;
    [SerializeField] private int bigSwingDamage = 3;

    private bool canDealDamage = false;
    private int currentDamage = 0;
    private bool hasDealtDamageThisSwing = false;


    public void EnableSmallSwing1()
    {
        EnableDamage(smallSwingDamage);
    }


    public void EnableSmallSwing2()
    {
        EnableDamage(smallSwingDamage);
    }

   
    public void EnableBigSwing()
    {
        EnableDamage(bigSwingDamage);
    }

 
    private void EnableDamage(int damage)
    {
        canDealDamage = true;
        currentDamage = damage;
        hasDealtDamageThisSwing = false;
        Debug.Log($"Weapon enabled with {damage} damage");
    }

  
    public void DisableDamage()
    {
        canDealDamage = false;
        currentDamage = 0;
        hasDealtDamageThisSwing = false;
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Weapon collided with: {other.gameObject.name}, Tag: {other.tag}");

        if (!canDealDamage)
        {
            Debug.Log("Weapon damage is disabled");
            return;
        }

        if (hasDealtDamageThisSwing)
        {
            Debug.Log("Already dealt damage this swing");
            return;
        }

       
        if (other.CompareTag("Player"))
        {
            Debug.Log("Hit object with Player tag!");
            var playerHealth = other.GetComponent<PlayerHealth>();

            if (playerHealth == null)
            {
              
                playerHealth = other.GetComponentInParent<PlayerHealth>();
                Debug.Log($"PlayerHealth in parent: {playerHealth != null}");
            }

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(currentDamage);
                hasDealtDamageThisSwing = true;
                Debug.Log($"Weapon hit player for {currentDamage} damage!");
            }
            else
            {
                Debug.LogError("PlayerHealth component not found on Player!");
            }
        }
        else
        {
            Debug.Log($"Object tag '{other.tag}' doesn't match 'Player'");
        }
    }
}