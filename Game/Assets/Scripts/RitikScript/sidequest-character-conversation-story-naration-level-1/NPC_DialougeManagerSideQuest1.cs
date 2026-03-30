using UnityEngine;

public class NPC_DialougeManagerSideQuest1 : MonoBehaviour
{
    [Header("Settings")]
    public string npcName = "Guardian of Shangri-La";

    [TextArea(3, 10)] 
    public string[] sentences = new string[] {
        "Traveler... fate has brought you to this ancient, mystic land of Shangri-La, hidden deep within the frozen Himalayas.",
        "A great darkness stirs. To save this planet from imminent destruction, you must find the celestial Orb of Creation.",
        "But such power is not for the faint of heart. You must walk the Path of the Ancients to prove your soul.",
        "First, seek the blessings of Ganapati, the Son of Shiva. Only He can remove the obstacles on your path.",
        "Next, find strength in Hanuman, the mighty Rudra avatar, for the journey requires unwavering devotion.",
        "Then, you must bow before Parvati, the Ardhangini and Mother of the Universe, to gain the power of Shakti.",
        "Only when you have these blessings will the Great Lord Shiva reveal the Orb to you. Go now! The fate of the world rests in your hands."
    };

    [Header("Visuals")]
    public GameObject visualPrompt; 
    private Animator anim;

    private bool isPlayerClose;
    private bool isTalking;

    void Start()
    {
        anim = GetComponent<Animator>();
        if (visualPrompt != null) visualPrompt.SetActive(false);
    }

    void Update()
    {
       
        if (isPlayerClose && Input.GetKeyDown(KeyCode.E))
        {
            if (!isTalking)
            {
                
                DialogueManagerSideQuest1.Instance.StartDialogue(npcName, sentences, anim);
                isTalking = true;

                if (visualPrompt != null) visualPrompt.SetActive(false); 
            }
            else
            {
               
                DialogueManagerSideQuest1.Instance.DisplayNextSentence();
            }
        }
    }

   
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerClose = true;
            if (visualPrompt != null) visualPrompt.SetActive(true); // Show "Press E"
        }
    }

   
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerClose = false;
            isTalking = false;
            if (visualPrompt != null) visualPrompt.SetActive(false);

          
        }
    }
}