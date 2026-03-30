using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class BossBullKingAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;
    private Transform target;

    [Header("Target Settings")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float stopDistance = 2.5f;
    [SerializeField] private float rotateSpeed = 10f;

    [Header("Detection Settings")]
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float chaseSpeed = 5f;

    [Header("Patrol Settings")]
    [SerializeField] private bool shouldPatrol = true;
    [SerializeField] private float patrolDistance = 5f;
    [SerializeField] private float patrolWaitTime = 2f;

    [Header("Combat Settings")]
    [SerializeField] private float attackRange = 2.7f;
    [SerializeField] private int smallSwingDamage = 1;
    [SerializeField] private int bigSwingDamage = 3;
    [SerializeField] private float attackCooldown = 1f;

    private float nextAttackTime = 0f;
    private bool attacking = false;

    // Patrol state
    private enum MobState { Patrolling, Chasing, Attacking }
    private MobState currentState = MobState.Patrolling;
    private Vector3 patrolPointA;
    private Vector3 patrolPointB;
    private Vector3 currentPatrolTarget;
    private float patrolWaitTimer = -1f;
    private Vector3 startPosition;
    [SerializeField] private string[] attackAnimations = new string[] { "BossAttack1", "BossAttack2", "BossAttack3", "BossAttack4", "BossAttack5" };
    [SerializeField] private float postAttackBuffer = 0.05f;
    public bool isDoingAttack = false;


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        agent.updateRotation = false;
        agent.stoppingDistance = stopDistance;
        agent.speed = patrolSpeed;

   
        startPosition = transform.position;

      
        Vector3 right = transform.right;
        Vector3 desiredPointA = startPosition - right * patrolDistance;
        Vector3 desiredPointB = startPosition + right * patrolDistance;

        UnityEngine.AI.NavMeshHit hitA, hitB;
        if (UnityEngine.AI.NavMesh.SamplePosition(desiredPointA, out hitA, patrolDistance, UnityEngine.AI.NavMesh.AllAreas))
        {
            patrolPointA = hitA.position;
        }
        else
        {
            patrolPointA = startPosition; 
            Debug.LogWarning("Patrol Point A not on NavMesh! Using start position.");
        }

        if (UnityEngine.AI.NavMesh.SamplePosition(desiredPointB, out hitB, patrolDistance, UnityEngine.AI.NavMesh.AllAreas))
        {
            patrolPointB = hitB.position;
        }
        else
        {
            patrolPointB = startPosition; 
            Debug.LogWarning("Patrol Point B not on NavMesh! Using start position.");
        }

        currentPatrolTarget = patrolPointB;


        var playerObj = GameObject.FindWithTag(playerTag);
        if (playerObj != null)
            target = playerObj.transform;
        else
            Debug.LogWarning("No GameObject tagged 'Player' found!");
    }

    void Update()
    {
        if (!target) return;

        float distanceToPlayer = Vector3.Distance(transform.position, target.position);

        
        switch (currentState)
        {
            case MobState.Patrolling:
                HandlePatrolling(distanceToPlayer);
                break;
            case MobState.Chasing:
                HandleChasing(distanceToPlayer);
                break;
            case MobState.Attacking:
                HandleAttacking(distanceToPlayer);
                break;
        }

      
        float normalizedSpeed = agent.velocity.magnitude / agent.speed;
        animator.SetFloat("Speed", normalizedSpeed);

       
        AnimatorStateInfo st = animator.GetCurrentAnimatorStateInfo(0);

        
        isDoingAttack = false;

       
        foreach (var atk in attackAnimations)
        {
            if (st.IsName(atk))
            {
                isDoingAttack = true;
                break;
            }
        }
    }


    void HandlePatrolling(float distanceToPlayer)
    {
        
        if (distanceToPlayer <= detectionRange)
        {
            currentState = MobState.Chasing;
            agent.speed = chaseSpeed;
            agent.isStopped = false;
            return;
        }

        
        if (!shouldPatrol)
        {
            agent.isStopped = true;
            return;
        }

        float distanceToPatrolPoint = Vector3.Distance(transform.position, currentPatrolTarget);

        if (distanceToPatrolPoint < 0.5f)
        {
           
            if (patrolWaitTimer > 0)
            {
               
                agent.isStopped = true;
                patrolWaitTimer -= Time.deltaTime;
            }
            else
            {
                
                currentPatrolTarget = (currentPatrolTarget == patrolPointA) ? patrolPointB : patrolPointA;
                patrolWaitTimer = patrolWaitTime;
                agent.isStopped = false;
                agent.SetDestination(currentPatrolTarget);
            }
        }
        else
        {
           
            agent.isStopped = false;
            agent.SetDestination(currentPatrolTarget);

            
            Vector3 direction = (currentPatrolTarget - transform.position).normalized;
            direction.y = 0;
            if (direction.sqrMagnitude > 0.001f)
            {
                Quaternion lookRot = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, rotateSpeed * Time.deltaTime);
            }
        }
    }

    void HandleChasing(float distanceToPlayer)
    {
        
        Vector3 toPlayer = target.position - transform.position;
        toPlayer.y = 0f;
        if (toPlayer.sqrMagnitude > 0.001f)
        {
            Quaternion lookRot = Quaternion.LookRotation(toPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, rotateSpeed * Time.deltaTime);
        }

        
        if (distanceToPlayer > stopDistance && !attacking)
        {
            agent.isStopped = false;
            agent.SetDestination(target.position);
        }
        else
        {
            agent.isStopped = true;
        }

       
        if (!attacking && distanceToPlayer <= attackRange && Time.time >= nextAttackTime)
            StartCoroutine(AttackRoutine());
    }

    void HandleAttacking(float distanceToPlayer)
    {
        
        Vector3 toPlayer = target.position - transform.position;
        toPlayer.y = 0f;
        if (toPlayer.sqrMagnitude > 0.001f)
        {
            Quaternion lookRot = Quaternion.LookRotation(toPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, rotateSpeed * Time.deltaTime);
        }

        agent.isStopped = true;

        if (distanceToPlayer > attackRange + 0.3f)
        {
            attacking = false;
            currentState = MobState.Chasing;
            agent.isStopped = false;
            return;
        }


        // Attack
        if (!attacking && Time.time >= nextAttackTime)
            StartCoroutine(AttackRoutine());
    }

    // helper coroutine:
    private IEnumerator WaitForAnimationEnd(string stateName)
    {
        
        int safety = 0;
        const int maxSafetyFrames = 300; 
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(stateName) && safety++ < maxSafetyFrames)
        {
            yield return null;
        }

        if (safety >= maxSafetyFrames)
        {
            Debug.LogWarning($"WaitForAnimationEnd: state '{stateName}' not found on layer 0.");
            yield break;
        }

       
        while (animator.GetCurrentAnimatorStateInfo(0).IsName(stateName) &&
               animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            yield return null;
        }

     
        if (postAttackBuffer > 0f) yield return new WaitForSeconds(postAttackBuffer);
    }

    private IEnumerator AttackRoutine()
    {
        if (target == null) yield break;
        if (attacking) yield break;
        if (Time.time < nextAttackTime) yield break;

        attacking = true;
        // animationLocked = true;   
        currentState = MobState.Attacking;

        nextAttackTime = Time.time + attackCooldown;

        // Face the player
        transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z));

        // Stop NavMesh movement during attack
        agent.isStopped = true;

        // Randomly pick attack animation
        string chosenAnim = "BossAttack1";
        if (attackAnimations != null && attackAnimations.Length > 0)
        {
            int idx = Random.Range(0, attackAnimations.Length);
            chosenAnim = attackAnimations[idx];
        }

        // Play chosen attack animation
        float crossfade = 0.08f;
        animator.CrossFade(chosenAnim, crossfade);

        // >>> Wait until the animation actually finishes <<<
        yield return StartCoroutine(WaitForAnimationEnd(chosenAnim));

        // Small polish delay
        if (postAttackBuffer > 0f)
            yield return new WaitForSeconds(postAttackBuffer);

        // Restore movement
        agent.isStopped = false;
        attacking = false;
        // animationLocked = false;
        currentState = MobState.Chasing;
    }


    public void OnSmallSwing1()
    {
        DealDamage(smallSwingDamage);
    }

    public void OnSmallSwing2()
    {
        DealDamage(smallSwingDamage);
    }

    public void OnBigSwing()
    {
        DealDamage(bigSwingDamage);
    }

    private void DealDamage(int damage)
    {
        if (!target) return;

        
        float distance = Vector3.Distance(transform.position, target.position);
        if (distance > attackRange + 0.5f)
        {
            return;
        }

        var playerHealth = target.GetComponent<PlayerHealth>();
        if (playerHealth == null)
        {
            playerHealth = target.GetComponentInParent<PlayerHealth>();
        }

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
        }
    }


    void OnDrawGizmosSelected()
    {
  
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

       
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

      
        if (Application.isPlaying)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(patrolPointA, 0.5f);
            Gizmos.DrawSphere(patrolPointB, 0.5f);
            Gizmos.DrawLine(patrolPointA, patrolPointB);

            // Current target
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, currentPatrolTarget);
        }
        else
        {
            
            Vector3 right = transform.right;
            Vector3 pointA = transform.position - right * patrolDistance;
            Vector3 pointB = transform.position + right * patrolDistance;

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(pointA, 0.5f);
            Gizmos.DrawSphere(pointB, 0.5f);
            Gizmos.DrawLine(pointA, pointB);
        }
    }

    public class SpeedDebug : MonoBehaviour
    {
        public Animator anim;
        void Update()
        {
            anim.SetFloat("Speed", Mathf.PingPong(Time.time * 5f, 5f));
        }
    }


}
