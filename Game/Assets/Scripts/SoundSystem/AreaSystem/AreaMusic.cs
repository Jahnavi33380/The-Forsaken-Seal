using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(AudioSource))]
public class AreaMusic : MonoBehaviour
{
    [Header("Setup")]
    public string playerTag = "Player";

    [Header("Type of zone")]
    public bool isCombatZone = false;

    [Header("Fade Settings")]
    [SerializeField] private float areaFadeDuration = 1.5f;
    [SerializeField] private float combatFadeDuration = 0.75f;
    [SerializeField] private float ambientVolumeDuringCombat = 0.25f;

    private AudioSource source;


    private static AudioSource currentAmbient;
    private static AudioSource currentCombat;

    private static AreaMusic fadeRunner;
    private static Coroutine ambientFade;
    private static Coroutine combatFade;

    private void Awake()
    {
        source = GetComponent<AudioSource>();

        var col = GetComponent<Collider>();
        col.isTrigger = true;

        source.playOnAwake = false;
        source.loop = true;


        if (fadeRunner == null)
            fadeRunner = this;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        if (isCombatZone)
        {

            SwitchMusic(ref currentCombat, areaFadeDuration);
        }
        else
        {

            SwitchMusic(ref currentAmbient, areaFadeDuration);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;


        if (!isCombatZone && currentAmbient == source)
        {
            if (fadeRunner != null)
            {
                if (ambientFade != null) fadeRunner.StopCoroutine(ambientFade);
                ambientFade = fadeRunner.StartCoroutine(
                    fadeRunner.FadeVolume(source, 0f, areaFadeDuration, true));
            }
            else
            {
                source.Stop();
            }

            currentAmbient = null;
        }
    }

    private void SwitchMusic(ref AudioSource current, float fadeTime)
    {
        if (fadeRunner == null) fadeRunner = this;


        if (current == source) return;


        if (!isCombatZone)
        {
            if (ambientFade != null) fadeRunner.StopCoroutine(ambientFade);
            ambientFade = fadeRunner.StartCoroutine(
                fadeRunner.Crossfade(current, source, fadeTime));
        }
        else
        {
            if (combatFade != null) fadeRunner.StopCoroutine(combatFade);
            combatFade = fadeRunner.StartCoroutine(
                fadeRunner.Crossfade(current, source, fadeTime));
        }

        current = source;
    }



    private IEnumerator Crossfade(AudioSource from, AudioSource to, float duration)
    {
        if (to == null) yield break;

        if (!to.isPlaying)
            to.Play();

        float time = 0f;
        float fromStartVol = (from != null) ? from.volume : 0f;
        float toStartVol = 0f;
        to.volume = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / duration);


            float smoothT = t * t * (3f - 2f * t);

            if (from != null)
                from.volume = Mathf.Lerp(fromStartVol, 0f, smoothT);

            to.volume = Mathf.Lerp(toStartVol, 0.15f, smoothT);

            yield return null;
        }

        if (from != null)
        {
            from.Stop();
            from.volume = fromStartVol;
        }

        to.volume = 0.15f;
    }

    private IEnumerator FadeVolume(AudioSource src, float targetVolume,
                               float duration, bool stopAtEndIfZero = false)
    {
        if (src == null) yield break;

        if (!src.isPlaying && targetVolume > 0f)
            src.Play();

        float startVolume = src.volume;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / duration);
            float smoothT = t * t * (3f - 2f * t);

            src.volume = Mathf.Lerp(startVolume, targetVolume, smoothT);
            yield return null;
        }

        src.volume = targetVolume;

        if (stopAtEndIfZero && Mathf.Approximately(targetVolume, 0f))
            src.Stop();
    }



    public static void StartCombat(AudioSource combatSource)
    {
        if (combatSource == null || fadeRunner == null) return;


        if (combatFade != null) fadeRunner.StopCoroutine(combatFade);
        AudioSource from = currentCombat;
        currentCombat = combatSource;
        combatFade = fadeRunner.StartCoroutine(
            fadeRunner.Crossfade(from, combatSource, fadeRunner.combatFadeDuration));


        if (currentAmbient != null)
        {
            if (ambientFade != null) fadeRunner.StopCoroutine(ambientFade);
            ambientFade = fadeRunner.StartCoroutine(
                fadeRunner.FadeVolume(currentAmbient,
                                      fadeRunner.ambientVolumeDuringCombat,
                                      fadeRunner.combatFadeDuration));
        }
    }

    public static void StopCombat()
    {
        if (fadeRunner == null) return;


        if (currentCombat != null)
        {
            if (combatFade != null) fadeRunner.StopCoroutine(combatFade);
            combatFade = fadeRunner.StartCoroutine(
                fadeRunner.FadeVolume(currentCombat, 0f,
                                      fadeRunner.combatFadeDuration, true));
            currentCombat = null;
        }


        if (currentAmbient != null)
        {
            if (ambientFade != null) fadeRunner.StopCoroutine(ambientFade);
            ambientFade = fadeRunner.StartCoroutine(
                fadeRunner.FadeVolume(currentAmbient, 1f,
                                      fadeRunner.combatFadeDuration));
        }
    }
}
