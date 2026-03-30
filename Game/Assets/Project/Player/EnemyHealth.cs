using UnityEngine;

public class EnemyHealth : MonoBehaviour
{


    [Header("Blood Effect")]

    public ParticleSystem bloodEffect;

    [Header("Attack Audio")]
    public AudioSource source;   // Drag your blood prefab here

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
