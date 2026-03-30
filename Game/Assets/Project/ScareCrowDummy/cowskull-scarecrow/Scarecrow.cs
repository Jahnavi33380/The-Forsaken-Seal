using UnityEngine;

public class Scarecrow : MonoBehaviour
{
    [Header("Hit VFX")]
    public ParticleSystem hitEffect;

    [Header("Hit SFX")]
    public AudioSource audioSource;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Weapon"))
        {
            Vector3 weaponPos = other.transform.position;
            Vector3 direction = (transform.position - weaponPos).normalized;

            Vector3 hitPoint = transform.position;
            Vector3 hitNormal = -direction;

            //Hit Point
            if (Physics.Raycast(weaponPos, direction, out RaycastHit hit, 20f))
            {
                hitPoint = hit.point;
                hitNormal = hit.normal;
            }

            PlayHitReaction(hitPoint, hitNormal);
        }
    }

    private void PlayHitReaction(Vector3 position, Vector3 normal)
    {
        // Hit Reaction particles
        if (hitEffect != null)
        {
            Quaternion rot = Quaternion.LookRotation(normal);
            ParticleSystem ps = Instantiate(hitEffect, position, rot);
            ps.Play();
            Destroy(ps.gameObject, ps.main.duration + 0.5f);
        }

        // Sound
        if (audioSource != null)
        {
            audioSource.Play();
        }
    }
}
