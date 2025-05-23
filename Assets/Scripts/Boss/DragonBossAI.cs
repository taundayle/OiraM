using UnityEngine;
using UnityEngine.AI;

public class DragonBossAI : MonoBehaviour
{
    public enum State { Idle, Chase, Attack, ReturnToStart }
    public State currentState;

    [Header("AI Settings")]
    public float chaseRange = 20f;
    public float attackRange = 10f;
    public float attackCooldown = 3f;
    public float rotationSpeed = 5f;
    public float returnSpeed = 4f;

    [Header("Attacks")]
    public string[] groundAttacks = { "Basic Attack", "Flame Attack" };

    [Header("VFX Settings")]
    public ParticleSystem flameVFX;
    public float flameRadius = 3f;
    public LayerMask playerLayer;

    [Header("SFX")]
    public AudioClip flameSound;

    private Transform player;
    private NavMeshAgent navMeshAgent;
    private Animator animator;
    private AudioSource audioSource;
    private Vector3 startPosition;
    private float lastAttackTime;

    void Start()
    {
        InitializeComponents();
        startPosition = transform.position;
        currentState = State.Idle;
        if (flameVFX != null) flameVFX.Stop();
    }

    void InitializeComponents()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        navMeshAgent.stoppingDistance = attackRange;
        navMeshAgent.speed = returnSpeed;
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

            case State.ReturnToStart:
                HandleReturnState();
                break;
        }
    }

    void HandleIdleState(float distance)
    {
        if (distance <= chaseRange)
        {
            TransitionToState(State.Chase, "Walk");
        }
        else
        {
            animator.SetTrigger("Idle01");
        }
    }

    void HandleChaseState(float distance)
    {
        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(player.position);
        FaceTarget();

        if (distance <= attackRange)
        {
            TransitionToState(State.Attack, "Idle01");
        }
        else if (distance > chaseRange)
        {
            TransitionToState(State.ReturnToStart, "Walk");
        }
    }

    void HandleAttackState(float distance)
    {
        navMeshAgent.isStopped = true;
        FaceTarget();

        if (Time.time > lastAttackTime + attackCooldown)
        {
            PerformAttack();
            lastAttackTime = Time.time;
        }

        if (distance > attackRange * 1.2f)
        {
            TransitionToState(distance > chaseRange ? State.ReturnToStart : State.Chase, "Walk");
        }
    }

    void HandleReturnState()
    {
        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(startPosition);
        FaceTarget();

        if (Vector3.Distance(transform.position, startPosition) < 1f)
        {
            TransitionToState(State.Idle, "Idle01");
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
        string attackTrigger = groundAttacks[randomIndex];
        animator.SetTrigger(attackTrigger);

        if (attackTrigger == "Flame Attack")
        {
            flameVFX.Play();
            PlayFlameSound();
        }
    }

    void FaceTarget()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            lookRotation,
            Time.deltaTime * rotationSpeed
        );
    }

    void PlayFlameSound()
    {
        if (flameSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(flameSound);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        if (flameVFX != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(flameVFX.transform.position, flameRadius);
        }
    }
}