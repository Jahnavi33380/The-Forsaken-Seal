using UnityEngine;

public class RotateUFO : MonoBehaviour
{
    public float rotationSpeed = 50f;
    public float hoverHeight = 0.5f;
    public float hoverSpeed = 2f;

    float startY;

    void Start()
    {
        startY = transform.position.y;
    }

    void Update()
    {
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);

        float newY = startY + Mathf.Sin(Time.time * hoverSpeed) * hoverHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}
