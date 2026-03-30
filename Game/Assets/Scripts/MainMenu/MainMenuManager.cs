using UnityEngine;
using UnityEngine.Video;
using System.Collections;

public class MainMenuManager : MonoBehaviour
{
    public enum TriggerMode { WaitForVideoEnd, DelaySeconds }

    [Header("Video")]
    [Tooltip("Optional - VideoPlayer component that plays your intro video.")]
    [SerializeField] private VideoPlayer videoPlayer;

    [Header("UI (use CanvasGroup for nice fade)")]
    [SerializeField] private CanvasGroup titleCanvasGroup; 
    [SerializeField] private CanvasGroup menuCanvasGroup;  

    [Header("Trigger settings")]
    [SerializeField] private TriggerMode triggerMode = TriggerMode.WaitForVideoEnd;
    [Tooltip("Used if TriggerMode = DelaySeconds")]
    [SerializeField] private float delaySeconds = 3.0f;

    [Header("Show timing (relative to trigger)")]
    [Tooltip("Delay after trigger before showing the title")]
    [SerializeField] private float titleDelay = 0.25f;
    [Tooltip("Delay after trigger before showing the menu")]
    [SerializeField] private float menuDelay = 0.8f;  

    [Header("Fade settings")]
    [SerializeField] private float fadeDuration = 0.5f;

  
    [SerializeField] private Animator titleAnimator;

    private void Awake()
    {
       
        if (titleCanvasGroup != null) SetCanvasGroupAlpha(titleCanvasGroup, 0f, interactable: false, blocksRaycasts: false);
        if (menuCanvasGroup != null) SetCanvasGroupAlpha(menuCanvasGroup, 0f, interactable: false, blocksRaycasts: false);
    }

    private void Start()
    {
       
        if (videoPlayer == null)
            videoPlayer = GetComponentInChildren<VideoPlayer>();

        if (triggerMode == TriggerMode.WaitForVideoEnd && videoPlayer != null)
        {
           
            if (videoPlayer.isLooping)
                Debug.LogWarning("VideoPlayer is looping � loopPointReached will never fire. Disable looping or use DelaySeconds mode.");

            videoPlayer.loopPointReached += OnVideoFinished;
        }
        else
        {
            
            StartCoroutine(TriggerAfterDelay(delaySeconds));
        }
    }

    private IEnumerator TriggerAfterDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        yield return StartCoroutine(ShowUISequence());
    }

    private void OnVideoFinished(VideoPlayer player)
    {
      
        StartCoroutine(ShowUISequence());
    }

    private IEnumerator ShowUISequence()
    {
      
        if (titleCanvasGroup != null)
        {
            yield return new WaitForSeconds(titleDelay);
            yield return StartCoroutine(FadeCanvasGroup(titleCanvasGroup, 0f, 1f, fadeDuration, enableInteraction: true));

            if (titleAnimator != null)
                titleAnimator.enabled = true;
        }

      
        if (menuCanvasGroup != null)
        {
            
            yield return new WaitForSeconds(menuDelay - titleDelay > 0 ? (menuDelay - titleDelay) : 0f);
            yield return StartCoroutine(FadeCanvasGroup(menuCanvasGroup, 0f, 1f, fadeDuration, enableInteraction: true, blocksRaycasts: true));
        }
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float from, float to, float duration, bool enableInteraction = false, bool blocksRaycasts = false)
    {
        if (cg == null) yield break;
        float t = 0f;    
        cg.alpha = from;    
        cg.interactable = false;   
        cg.blocksRaycasts = false;
          
        while (t < duration)
        {
            t += Time.deltaTime;
            cg.alpha = Mathf.Lerp(from, to, duration > 0f ? (t / duration) : 1f);
            yield return null;
        }
        cg.alpha = to;
        cg.interactable = enableInteraction;
        cg.blocksRaycasts = blocksRaycasts;
    }

    private void SetCanvasGroupAlpha(CanvasGroup cg, float a, bool interactable = true, bool blocksRaycasts = true)
    {
        if (cg == null) return;
        cg.alpha = a;
        cg.interactable = interactable;
        cg.blocksRaycasts = blocksRaycasts;
    }

    private void OnDestroy()
    {
        if (videoPlayer != null)
            videoPlayer.loopPointReached -= OnVideoFinished;
    }
}