using UnityEngine;

public class BossAi : MonoBehaviour
{
    public Transform player;

    [Header("Movement Settings")]
    public float rotateSpeed = 5f;
    public float moveSpeed = 2f;        
    private float normalMoveSpeed;      

    [Header("Ranges")]
    public float chaseRange = 10f;
    public float attackRange = 2f;

    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
        normalMoveSpeed = moveSpeed;    
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > chaseRange)
        {
            Idle();
        }
        else if (distance > attackRange)
        {
            ChasePlayer();
        }
        else
        {
            AttackPlayer();
        }
    }

    void Idle()
    {
        anim.SetBool("IsWalking", false);

     
        if (moveSpeed == 0f)
            moveSpeed = normalMoveSpeed;
    }

    void ChasePlayer()
    {
        RotateTowardsPlayer();

     
        if (moveSpeed == 0f)
            moveSpeed = normalMoveSpeed;

        anim.SetBool("IsWalking", true);

      
        transform.position += transform.forward * moveSpeed * Time.deltaTime;
    }

    void AttackPlayer()
    {
        RotateTowardsPlayer();

        anim.SetBool("IsWalking", false);

     
        moveSpeed = 0f;

        anim.SetTrigger("Attack");
    }

    void RotateTowardsPlayer()
    {
        Vector3 direction = player.position - transform.position;
        direction.y = 0;

        if (direction.sqrMagnitude < 0.01f) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotateSpeed * Time.deltaTime
        );
    }
}
