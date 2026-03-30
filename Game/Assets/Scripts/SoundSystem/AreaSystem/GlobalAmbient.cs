using UnityEngine;



[RequireComponent(typeof(AudioSource))]
public class GlobalAmbient : MonoBehaviour
{
    [Range(0f, 1f)]
    public float volume = 0.5f;

    private AudioSource src;

    void Awake()
    {
        src = GetComponent<AudioSource>();
        src.loop = true;
        src.volume = volume;

     
        if (src.clip != null && !src.isPlaying)
        {
            src.playOnAwake = false; 
            src.Play();
        }
    }
}