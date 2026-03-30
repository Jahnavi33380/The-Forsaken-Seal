using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ControllerPerson : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] public float speedMeasure = 5f;
    [SerializeField] float timeSmooth = 0.1f;

    [Header("Jump/Gravity")]
    [SerializeField] float heightOfJump = 1.6f;
    [SerializeField] float gravity = -20f;

    [Header("References")]
    [SerializeField] public Transform cameraTransform;
    [SerializeField] public Animator playerAnim;

    CharacterController controller;
    float turnSmoothVelocity;
    Vector3 velocity;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        bool grounded = controller.isGrounded;
        if (grounded && velocity.y < 0f)
            velocity.y = -2f;

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 input = new Vector3(h, 0f, v);
        float inputMagnitude = input.magnitude;

        bool isMoving = inputMagnitude >= 0.1f;

        if (isMoving)
        {
            float targetAngle = Mathf.Atan2(input.x, input.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, timeSmooth);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speedMeasure * Time.deltaTime);
        }

        //Jump with animations
        if (Input.GetButtonDown("Jump") && grounded)
        {
            velocity.y = Mathf.Sqrt(heightOfJump * -2f * gravity);

            playerAnim.SetBool("StartJump", true);
            playerAnim.SetBool("IsLanded", true);
        }
        else
        {
            playerAnim.SetBool("StartJump", false);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(new Vector3(0f, velocity.y, 0f) * Time.deltaTime);

        if (!grounded)
            playerAnim.SetBool("IsLanded", false);
        else
            playerAnim.SetBool("IsLanded", true);

        //Chracter movement animations
        playerAnim.SetBool("isWalking", v > 0);
        playerAnim.SetBool("isWalkingBack", v < 0);
        playerAnim.SetBool("isWalkingLeft", h < 0);
        playerAnim.SetBool("isWalkingRight", h > 0);
    }
}
