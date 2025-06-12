using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator), typeof(NavMeshAgent), typeof(AudioSource))]
public class BossAI : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    private Animator animator;
    private NavMeshAgent agent;
    private AudioSource audioSource;

    [Header("Ranges & Timers")]
    public float chaseRange = 15f;
    public float stoppingDistance = 4f;
    [Tooltip("Bắt đầu Combat khi ≤ giá trị này")]
    public float combatEnterRange = 8f;
    [Tooltip("Tầm Claw Attack")]
    public float clawRange = 5f;
    [Tooltip("Tầm Flame Attack")]
    public float flameRange = 10f;
    [Tooltip("Cooldown giữa các đòn đánh (s)")]
    public float attackCooldown = 3f;

    [Header("Audio Clips")]
    public AudioClip screamSound;
    public AudioClip flameSound;

    [Header("Flame Effect")]
    [Tooltip("Prefab particle cho lửa")]
    public GameObject flameEffectPrefab;
    [Tooltip("Thời gian tồn tại (s) của hiệu ứng lửa")]
    public float flameEffectDuration = 1.5f;

    [Header("Movement Smoothing")]
    public float rotationSpeed = 5f;
    public string speedParameter = "Speed";

    // Internal state
    private bool inCombat = false;
    private bool hasScreamedOnApproach = false;
    private float lastAttackTime = 0f;

    private enum AttackType { Claw, Flame }
    private List<AttackType> attackOrder = new List<AttackType> { AttackType.Claw, AttackType.Flame };
    private int nextAttackIndex = 0;

    void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();

        agent.stoppingDistance = stoppingDistance;
        agent.updateRotation = false;
    }

    void Start()
    {
        animator.SetBool("PlayerInRange", false);
        animator.SetBool("InCombat", false);
        animator.SetFloat(speedParameter, 0f);
    }

    void Update()
    {
        float dist = Vector3.Distance(transform.position, player.position);
        bool playerInRange = dist <= chaseRange;
        animator.SetBool("PlayerInRange", playerInRange);

        // 1) Scream once when player enters chaseRange
        if (playerInRange && !hasScreamedOnApproach)
        {
            TriggerScream();
            hasScreamedOnApproach = true;
        }
        else if (!playerInRange)
        {
            hasScreamedOnApproach = false;
        }

        if (playerInRange)
        {
            HandleMovement(dist);

            if (dist <= combatEnterRange)
            {
                if (!inCombat)
                {
                    inCombat = true;
                    animator.SetBool("InCombat", true);
                }
                HandleCombat(dist);
            }
            else if (inCombat)
            {
                ResetCombat();
            }
        }
        else
        {
            ResetCombat();
        }

        // Blend run/idle
        animator.SetFloat(speedParameter, agent.velocity.magnitude);
    }

    private void TriggerScream()
    {
        animator.SetTrigger("Scream");
        // audio sẽ được phát bởi Animation Event
    }

    /// <summary>
    /// Gọi từ Animation Event trên clip Scream để phát âm thanh đúng timing
    /// </summary>
    public void PlayScreamAudioEvent()
    {
        if (screamSound != null)
            audioSource.PlayOneShot(screamSound);
    }

    private void HandleMovement(float dist)
    {
        if (dist > stoppingDistance)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
            RotateTowards(agent.desiredVelocity);
        }
        else
        {
            agent.isStopped = true;
        }
    }

    private void RotateTowards(Vector3 dir)
    {
        if (dir.sqrMagnitude > 0.01f)
        {
            Quaternion target = Quaternion.LookRotation(dir.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, target, rotationSpeed * Time.deltaTime);
        }
    }

    private void HandleCombat(float dist)
    {
        if (Time.time < lastAttackTime + attackCooldown)
            return;

        for (int i = 0; i < attackOrder.Count; i++)
        {
            int idx = (nextAttackIndex + i) % attackOrder.Count;
            AttackType type = attackOrder[idx];

            if ((type == AttackType.Claw && dist <= clawRange) ||
                (type == AttackType.Flame && dist <= flameRange))
            {
                ExecuteAttack(type);
                nextAttackIndex = (idx + 1) % attackOrder.Count;
                break;
            }
        }
    }

    private void ExecuteAttack(AttackType type)
    {
        // Quay mặt boss về player
        Vector3 dir = (player.position - transform.position).normalized;
        if (dir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(dir);

        if (type == AttackType.Claw)
        {
            animator.SetTrigger("ClawAttack");
        }
        else // Flame
        {
            animator.SetTrigger("FlameAttack");
            // effect và sound sẽ được gọi qua Animation Event
        }

        lastAttackTime = Time.time;
    }

    private void ResetCombat()
    {
        agent.isStopped = true;
        if (inCombat)
        {
            inCombat = false;
            animator.SetBool("InCombat", false);
        }
    }

    /// <summary>
    /// Gọi từ Animation Event trên clip FlameAttack để tạo effect + phát sound
    /// </summary>
    public void PlayFlameEffectEvent()
    {
        if (flameSound != null)
            audioSource.PlayOneShot(flameSound);

        if (flameEffectPrefab != null)
        {
            Vector3 pos = transform.position + transform.forward * 1.5f + Vector3.up * 1f;
            GameObject flame = Instantiate(flameEffectPrefab, pos, Quaternion.LookRotation(transform.forward));
            Destroy(flame, flameEffectDuration);
        }
    }

    // Gọi ngoài nếu cần xử lý HP
    public void TakeDamage(float damage) { }
}
