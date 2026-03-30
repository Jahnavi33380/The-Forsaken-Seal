using System.Collections;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Collider))]
public class StateGate : MonoBehaviour
{

    [Header("Gate Settings")]
   
   
    [SerializeField] private PlayerHealth playerHealth;   


    
    [Tooltip("How much health the player needs to pass.")]
    public int requiredHealth = 220;

    [Tooltip("Tag used by the player object.")]
    public string playerTag = "Player";

    [Tooltip("The wall / mesh that blocks the player (will be disabled when gate opens).")]
    public GameObject wallObject;

    [Header("UI Hint")]
    public TextMeshProUGUI hintText;          // assign HintText
    [TextArea]
    public string lockedMessage =
        "You feel too weak...\nUpgrade your health and weapon to pass.";

    public Color hintColor = new Color(1f, 0.77f, 0.35f, 1f);  // warm amber
    public float messageDuration = 2.5f;       // seconds visible before fade


    private bool isOpen = false;
    private Coroutine messageRoutine;

    private void Awake()
    {
        // Make sure THIS collider is a trigger
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void Start()
    {
        if (playerHealth == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);
            if (playerObj != null)
            {
                playerHealth = playerObj.GetComponent<PlayerHealth>()
                               ?? playerObj.GetComponentInChildren<PlayerHealth>();
            }
        }

        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged += OnPlayerHealthChanged;
        }
        else
        {
            Debug.LogWarning("StateGate: PlayerHealth could not be found.");
        }

        HideMessageImmediate();
    }


    private void OnDestroy()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged -= OnPlayerHealthChanged;
        }
    }

    // Called whenever the player's health changes
    private void OnPlayerHealthChanged(int current, int max)
    {
        if (isOpen) return;

        // You can swap current with max if you want requirement on maxHealth instead:
        // if (max >= requiredHealth) ...
        if (current >= requiredHealth)
        {
            OpenGate();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Gate trigger hit by: {other.name}");

        if (isOpen)
        {
            Debug.Log("StateGate: Gate already open, ignoring trigger.");
            return;
        }

        // Only react to the player
        if (!other.CompareTag(playerTag))
        {
            Debug.Log($"StateGate: {other.name} does not have tag {playerTag}, ignoring.");
            return;
        }

        // If we somehow didn't grab PlayerHealth in Start(), try again from this collider
        if (playerHealth == null)
        {
            playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth == null)
                playerHealth = other.GetComponentInParent<PlayerHealth>();

            if (playerHealth == null)
            {
                Debug.LogWarning("StateGate: Player collided but PlayerHealth is still NULL.");
                return;
            }

            // now we have it, subscribe to health changes so the gate can auto-open later
            playerHealth.OnHealthChanged += OnPlayerHealthChanged;
        }

        int current = (int)playerHealth.GetCurrentHealth();
        Debug.Log($"StateGate: Player health = {current}, required = {requiredHealth}");

        // Player bumped gate but is too weak → show hint
        if (current < requiredHealth)   
        {
            ShowLockedMessage();
        }
    }      


    private void OpenGate()
    {
        isOpen = true;

        if (wallObject != null)
            wallObject.SetActive(false);  // remove collision + visuals

        HideMessageImmediate();
        Debug.Log("StatGate: Gate opened (health requirement met).");
    }

    private void ShowLockedMessage()
    {
        if (hintText == null) return;

        hintText.text = lockedMessage;
        hintText.color = hintColor;

        CanvasGroup cg = hintText.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = hintText.gameObject.AddComponent<CanvasGroup>();

        if (messageRoutine != null)
            StopCoroutine(messageRoutine);

        messageRoutine = StartCoroutine(MessageRoutine(cg));
    }

    private IEnumerator MessageRoutine(CanvasGroup cg)
    {
        float fadeTime = 0.3f;

        // Instant show
        cg.alpha = 1f;

        // Stay visible
        yield return new WaitForSeconds(messageDuration);

        // Fade out 
        float t = 0f;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            cg.alpha = Mathf.Lerp(1f, 0f, t / fadeTime);
            yield return null;
        }
        cg.alpha = 0f;
    }

    private void HideMessageImmediate()
    {
        if (hintText == null) return;

        CanvasGroup cg = hintText.GetComponent<CanvasGroup>();
        if (cg == null) cg = hintText.gameObject.AddComponent<CanvasGroup>();
        cg.alpha = 0f;
    }
}
