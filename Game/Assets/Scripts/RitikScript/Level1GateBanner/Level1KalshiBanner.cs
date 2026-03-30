using UnityEngine;
using TMPro;
using System.Collections;

public class Level1KalshiBanner : MonoBehaviour
{
    public static Level1KalshiBanner Instance { get; private set; }

    [Header("References")]
    [SerializeField] private TextMeshProUGUI bannerText;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Timing")]
    [SerializeField] private float fadeInTime = 0.5f;
    [SerializeField] private float holdTime = 2.0f;
    [SerializeField] private float fadeOutTime = 0.7f;

    private Coroutine currentRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)   
        {   
            Destroy(gameObject);
            return;   
        }
        Instance = this;
           
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

      
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }

    /// <summary>

    /// </summary>
    public static void Show(string message)
    {
        if (Instance == null)
        {
            Debug.LogWarning("LevelBannerUI: no instance in scene.");
            return;
        }
        Instance.ShowInternal(message);
    }

    private void ShowInternal(string message)
    {
        if (bannerText != null)
            bannerText.text = message;

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(BannerRoutine());
    }

    private IEnumerator BannerRoutine()
    {
        if (canvasGroup == null) yield break;

        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

    
        float t = 0f;
        while (t < fadeInTime)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, t / fadeInTime);
            yield return null;
        }
        canvasGroup.alpha = 1f;


        yield return new WaitForSeconds(holdTime);

  
        t = 0f;
        while (t < fadeOutTime)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t / fadeOutTime);
            yield return null;
        }
        canvasGroup.alpha = 0f;
    }
}










