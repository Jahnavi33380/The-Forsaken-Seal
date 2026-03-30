// using UnityEngine;

// [RequireComponent(typeof(Collider))]
// public class Level1KalshiIntoTrigger : MonoBehaviour
// {
//     [Header("Settings")]
//     public string playerTag = "Player";
//     [TextArea]
//     public string introText = "LEVEL I � KALSHI BOSS";

//     private bool alreadyTriggered = false;

//     private void Awake()
//     {
//         var col = GetComponent<Collider>();
//         col.isTrigger = true;
//     }

//     private void OnTriggerEnter(Collider other)
//     {
//         if (alreadyTriggered) return;
//         if (!other.CompareTag(playerTag)) return;

//         alreadyTriggered = true;
//         Level1KalshiBanner.Show(introText);
//         FindObjectOfType<TriggerShowUILevel1>().ShowBar();
//     }
// }

using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Level1KalshiIntoTrigger : MonoBehaviour
{
    [Header("Settings")]
    public string playerTag = "Player";
    [TextArea]
    public string introText = "LEVEL I � KALSHI BOSS";

    private bool alreadyTriggered = false;

    // assign this in the Inspector to the Level1BossHealthBar GameObject (the root of the UI)
    public GameObject bossHealthBar;
    public GameObject bossLockdown;

    private void Awake()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;

        // ensure it's off at start (safety if you forgot to set in inspector)
        if (bossHealthBar != null)
            bossHealthBar.SetActive(false);
        if (bossLockdown != null)
            bossLockdown.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (alreadyTriggered) return;
        if (!other.CompareTag(playerTag)) return;

        alreadyTriggered = true;

        Level1KalshiBanner.Show(introText);
        FindObjectOfType<TriggerShowUILevel1>()?.ShowBar();

        if (bossHealthBar != null)
            bossHealthBar.SetActive(true);
        if (bossLockdown != null)
            bossLockdown.SetActive(true);
    }
}