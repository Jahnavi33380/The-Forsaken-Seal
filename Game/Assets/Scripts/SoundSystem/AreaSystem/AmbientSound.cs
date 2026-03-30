using UnityEngine;
using System.Collections;

public class AmbientSound : MonoBehaviour
{
    //public Collider Area;        // The trigger area where sound should play
    public AudioSource source;    // The AudioSource that holds the ambient clip
    public AudioClip audioClip;


    void upadte()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            source.PlayOneShot(audioClip);
        }
    }

}
