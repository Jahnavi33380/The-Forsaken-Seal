using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("UI")]
    public Slider healthSlider;
    public Slider easeHealthSlider;

    [Header("Smoothing")]
    public float lerpSpeed = 0.05f;

    [Header("Targeting")]
    [Tooltip("Drag the GameObject that owns the PlayerHealth or MobHealth (for example the Mob parent). " +
             "If left empty, the script will try to find a HealthBarInterface on this GameObject or its parents/children.")]
    [SerializeField] private GameObject targetObject;

    // internal reference to the health interface
    private HealthBarInterface target;

    void Start()
    {
        if (targetObject == null)
        {
            // try to find on same gameobject first
            TryBindTargetFromGameObject(gameObject);
        }
        else
        {
            // if targetObject is provided, try to bind from it
            TryBindTargetFromGameObject(targetObject);
        }

        // if still not bound, try to auto-find nearby (GetComponentInParent/GetComponentInChildren)
        if (target == null)
        {
            // try parent chain
            var foundFromParent = GetComponentInParentHealth();
            if (foundFromParent != null) BindTo(foundFromParent);
        }

        if (target == null)
        {
            var foundFromChildren = GetComponentInChildrenHealth();
            if (foundFromChildren != null) BindTo(foundFromChildren);
        }

        if (target == null)
        {
            Debug.LogError($"HealthBar ({name}): Could not find any HealthBarInterface. Assign the mob/player GameObject to Target Object.");
            enabled = false;
            return;
        }

        // initialize UI values
        healthSlider.maxValue = target.GetMaxHealth();
        easeHealthSlider.maxValue = target.GetMaxHealth();
        healthSlider.value = target.GetCurrentHealth();
        easeHealthSlider.value = healthSlider.value;

        // subscribe to health changes
        target.OnHealthChanged += UpdateInstantBar;
    }

    void OnDestroy()
    {
        if (target != null)
            target.OnHealthChanged -= UpdateInstantBar;
    }

    void Update()
    {
        if (!Mathf.Approximately(easeHealthSlider.value, healthSlider.value))
        {
            easeHealthSlider.value = Mathf.Lerp(
                easeHealthSlider.value,
                healthSlider.value,
                lerpSpeed * Time.deltaTime * 60f
            );
        }
    }

    void UpdateInstantBar(int current, int max)
    {
        // if max changed dynamically, update sliders' max
        healthSlider.maxValue = max;
        easeHealthSlider.maxValue = max;
        healthSlider.value = current;
    }

    // Helper methods

    private void TryBindTargetFromGameObject(GameObject go)
    {
        if (go == null) return;

        var comp = go.GetComponent<MonoBehaviour>();
        var monos = go.GetComponents<MonoBehaviour>();
        foreach (var m in monos)
        {
            if (m is HealthBarInterface hbi)
            {
                BindTo(hbi);
                return;
            }
        }

    }

    private void BindTo(HealthBarInterface hbi)
    {
        if (target != null)
        {
            target.OnHealthChanged -= UpdateInstantBar;
        }

        target = hbi;
    }

    private HealthBarInterface GetComponentInParentHealth()
    {
        Transform t = transform;
        while (t != null)
        {
            var monos = t.GetComponents<MonoBehaviour>();
            foreach (var m in monos)
            {
                if (m is HealthBarInterface hbi) return hbi;
            }
            t = t.parent;
        }
        return null;
    }

    private HealthBarInterface GetComponentInChildrenHealth()
    {
        var monos = GetComponentsInChildren<MonoBehaviour>();
        foreach (var m in monos)
        {
            if (m is HealthBarInterface hbi) return hbi;
        }
        return null;
    }
}
