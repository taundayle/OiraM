using UnityEngine;
using UnityEngine.AI;

public class DragonBossAI : MonoBehaviour
{
    public enum State { Idle, Chase, Attack }
    public State currentState;

    [Header("Settings")]
    public float chaseRange = 20f;
    public float attackRange = 10f;
    public float attackCooldown = 3f;
    public float rotationSpeed = 5f;

    [Header("Attacks")]
    public string[] groundAttacks = { "Basic Attack", "Claw Attack", "Flame Attack" };

    private Transform player;
    private NavMeshAgent navMeshAgent;
    private Animator animator;
    private float lastAttackTime;

    void Start()
    {
        InitializeComponents();
        currentState = State.Idle;
    }

    void InitializeComponents()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        // Thiết lập NavMeshAgent
        navMeshAgent.speed = 5f;
        navMeshAgent.stoppingDistance = attackRange;
        navMeshAgent.angularSpeed = 120f;
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        UpdateStateMachine(distanceToPlayer);
    }

    void UpdateStateMachine(float distance)
    {
        switch (currentState)
        {
            case State.Idle:
                HandleIdleState(distance);
                break;

            case State.Chase:
                HandleChaseState(distance);
                break;

            case State.Attack:
                HandleAttackState(distance);
                break;
        }
    }

    void HandleIdleState(float distance)
    {
        if (distance <= chaseRange)
        {
            TransitionToState(State.Chase, "Walk");
        }
    }

    void HandleChaseState(float distance)
    {
        // Di chuyển đến player và xoay mặt
        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(player.position);
        FaceTarget();

        // Chuyển sang tấn công nếu đủ gần
        if (distance <= attackRange)
        {
            TransitionToState(State.Attack, "Idle01");
        }
    }

    void HandleAttackState(float distance)
    {
        // Dừng di chuyển và tấn công
        navMeshAgent.isStopped = true;
        FaceTarget();

        if (Time.time > lastAttackTime + attackCooldown)
        {
            PerformAttack();
            lastAttackTime = Time.time;
        }

        // Quay lại đuổi nếu player chạy xa
        if (distance > attackRange * 1.2f)
        {
            TransitionToState(State.Chase, "Walk");
        }
    }

    void TransitionToState(State newState, string animationTrigger)
    {
        ResetAllTriggers();
        currentState = newState;
        animator.SetTrigger(animationTrigger);
    }

    void ResetAllTriggers()
    {
        foreach (var param in animator.parameters)
        {
            if (param.type == AnimatorControllerParameterType.Trigger)
            {
                animator.ResetTrigger(param.name);
            }
        }
    }

    void PerformAttack()
    {
        if (groundAttacks.Length == 0) return;
        int randomIndex = Random.Range(0, groundAttacks.Length);
        animator.SetTrigger(groundAttacks[randomIndex]);
    }

    void FaceTarget()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}