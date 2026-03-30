using UnityEngine;
using System.Collections;
using UnityEditor;

public class PlayerAttacks : MonoBehaviour
{
    [Header("Hit Detectors")]
    [SerializeField] private Collider hitcollider;

    [Header("Animator")]
    [SerializeField] private Animator anim;

    [Header("Combo Settings")]
    [SerializeField] private float maxComboDelay = 0.8f;
    [SerializeField][Range(0f, 0.5f)] private float inputBufferWindow = 0.15f;
    [SerializeField][Range(0f, 1f)] private float minNormalizedTimeToChain = 0.06f;
    [SerializeField] private int maxComboSteps = 4;

    [Header("Animator Mode")]
    [SerializeField] private bool useTriggers = true;

    private int clickCount = 0;
    private float lastClickTime = 0f;
    private int bufferedClicks = 0;
    private bool inputBuffered = false;

    void Update()
    {
        if (anim == null) return;

        if (Input.GetMouseButtonDown(0))
        {
            RegisterClick();
        }

        if (Time.time - lastClickTime > maxComboDelay)
        {
            ResetCombo();
        }

        if (bufferedClicks > 0)
        {
            BufferedClicks();
        }

    }

    void RegisterClick()
    {
        lastClickTime = Time.time;

        if (clickCount == 0)
        {
            clickCount = 1;
            PlayAttack(clickCount);
            return;
        }

        if (clickCount < maxComboSteps)
        {
            bufferedClicks = Mathf.Clamp(bufferedClicks + 1, 0, maxComboSteps - clickCount);
            inputBuffered = true;
        }
    }

    void BufferedClicks()
    {
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);


        bool inAttackState = stateInfo.IsName("LightAttack1") ||
                             stateInfo.IsName("LightAttack2") ||
                             stateInfo.IsName("LightAttack3") ||
                             stateInfo.IsName("LightAttack4");

        if (!inAttackState)
        {
            ExecuteBufferedClick();
            return;
        }

        float normalized = stateInfo.normalizedTime % 1f;

        bool canChainNow = normalized >= minNormalizedTimeToChain || normalized >= (1f - inputBufferWindow);

        if (canChainNow)
        {
            ExecuteBufferedClick();
        }

    }

    void ExecuteBufferedClick()
    {
        if (bufferedClicks <= 0) return;

        int nextIndex = Mathf.Clamp(clickCount + 1, 1, maxComboSteps);

        clickCount = nextIndex;
        bufferedClicks = Mathf.Max(0, bufferedClicks - 1);
        inputBuffered = bufferedClicks > 0;

        PlayAttack(nextIndex);
    }

    void PlayAttack(int index)
    {
        if (useTriggers)
        {
            string trigName = $"LightAttack{index}";
            anim.ResetTrigger("LightAttack1");
            anim.ResetTrigger("LightAttack2");
            anim.ResetTrigger("LightAttack3");
            anim.ResetTrigger("LightAttack4");
            anim.SetTrigger(trigName);
        }
        else
        {
            anim.SetBool("LightAttack1", false);
            anim.SetBool("LightAttack2", false);
            anim.SetBool("LightAttack3", false);
            anim.SetBool("LightAttack4", false);

            anim.SetBool($"LightAttack{index}", true);
        }
    }

    void ResetCombo()
    {
        clickCount = 0;
        bufferedClicks = 0;
        inputBuffered = false;
        lastClickTime = 0f;

        if (anim != null)
        {
            if (useTriggers)
            {
                anim.ResetTrigger("LightAttack1");
                anim.ResetTrigger("LightAttack2");
                anim.ResetTrigger("LightAttack3");
                anim.ResetTrigger("LightAttack4");
            }
            else
            {
                anim.SetBool("LightAttack1", false);
                anim.SetBool("LightAttack2", false);
                anim.SetBool("LightAttack3", false);
                anim.SetBool("LightAttack4", false);
            }
        }
    }

    public void activatecollider()
    {
        hitcollider.enabled = true;
    }

    public void deactivatecollider()
    {
        hitcollider.enabled = false;
    }
}
