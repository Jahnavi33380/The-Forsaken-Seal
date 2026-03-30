using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Needed for Text Mesh Pro

public class DialogueManagerSideQuest1 : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public GameObject dialogueBox;
    public GameObject interactPrompt; // The "Press E" text

    private Queue<string> sentences; 
    private Animator currentNPCAnimator; 

   
    public static DialogueManagerSideQuest1 Instance;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        sentences = new Queue<string>();
        dialogueBox.SetActive(false);
        if (interactPrompt != null) interactPrompt.SetActive(false);
    }

    public void StartDialogue(string name, string[] dialogueLines, Animator npcAnimator)
    {
        currentNPCAnimator = npcAnimator;

     
        dialogueBox.SetActive(true);
        nameText.text = name;

       
        if (currentNPCAnimator != null)
        {
            currentNPCAnimator.SetBool("IsTalking", true);
        }

     
        sentences.Clear();
        foreach (string sentence in dialogueLines)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        dialogueText.text = sentence;
       
    }

    void EndDialogue()
    {
        dialogueBox.SetActive(false);

     
        if (currentNPCAnimator != null)
        {
            currentNPCAnimator.SetBool("IsTalking", false);
            currentNPCAnimator = null;
        }
    }
}