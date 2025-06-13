using UnityEngine;
using UnityEngine.AI;

public class NPCMovement : MonoBehaviour
{
    public NavMeshAgent agent;
    public Animator animator;

    // Thay đổi: Center point cho khu vực di chuyển
    public Vector3 centerPoint; // Vị trí trung tâm của khu vực di chuyển
    public float walkRadius = 5f; // Bán kính khu vực NPC có thể di chuyển (giảm giá trị này)

    public float minWaitTime = 2f;
    public float maxWaitTime = 5f;

    private Vector3 randomDestination;
    private float nextMoveTime;
    private bool isStoppedByInteraction = false;

    void Start()
    {
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        // Nếu centerPoint chưa được đặt (giá trị mặc định là 0,0,0),
        // gán nó là vị trí khởi đầu của NPC.
        // Hoặc bạn có thể đặt thủ công trong Inspector.
        if (centerPoint == Vector3.zero)
        {
            centerPoint = transform.position;
        }

        SetRandomDestination();
    }

    void Update()
    {
        if (animator != null && agent != null)
        {
            if (isStoppedByInteraction)
            {
                animator.SetFloat("Speed", 0f);
            }
            else
            {
                animator.SetFloat("Speed", agent.velocity.magnitude);
            }
        }

        if (!isStoppedByInteraction)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (Time.time >= nextMoveTime)
                {
                    SetRandomDestination();
                }
            }
        }
    }

    // Hàm để tìm một điểm đến ngẫu nhiên trong bán kính cho phép quanh centerPoint
    void SetRandomDestination()
    {
        // Tạo một điểm ngẫu nhiên trong hình cầu có bán kính walkRadius
        Vector3 randomDirection = Random.insideUnitSphere * walkRadius;
        // Cộng với centerPoint để điểm đó nằm trong khu vực mong muốn
        randomDirection += centerPoint;

        NavMeshHit hit;
        // Bán kính để tìm điểm trên NavMesh xung quanh randomDirection.
        // Giá trị này nên lớn hơn walkRadius một chút để đảm bảo tìm được điểm trên NavMesh
        float samplePositionRadius = walkRadius * 2f;
        if (NavMesh.SamplePosition(randomDirection, out hit, samplePositionRadius, NavMesh.AllAreas))
        {
            randomDestination = hit.position;
            agent.SetDestination(randomDestination);
            nextMoveTime = Time.time + Random.Range(minWaitTime, maxWaitTime);
        }
        else
        {
            // Debug.LogWarning("Could not find a valid NavMesh point within the specified radius. Retrying...");
            // Nếu không tìm được điểm, thử lại ngay lập tức
            SetRandomDestination();
        }
    }

    public void StopMovement()
    {
        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = true;
            isStoppedByInteraction = true;
            animator.SetFloat("Speed", 0f);
        }
    }

    public void ResumeMovement()
    {
        if (agent != null && agent.isOnNavMesh)
        {
            isStoppedByInteraction = false;
            agent.isStopped = false;
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                SetRandomDestination();
            }
        }
    }
}








/*using UnityEngine;
using UnityEngine.AI;

public class NPCMovement : MonoBehaviour
{
    public NavMeshAgent agent;
    public Animator animator;

    public float walkRadius = 10f;
    public float minWaitTime = 2f;
    public float maxWaitTime = 5f;

    private Vector3 randomDestination;
    private float nextMoveTime;
    private bool isStoppedByInteraction = false; // THÊM: Biến kiểm tra có bị dừng bởi tương tác không

    void Start()
    {
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        // Đặt điểm đến ngẫu nhiên đầu tiên
        SetRandomDestination();
    }

    void Update()
    {
        if (animator != null && agent != null)
        {
            // Cập nhật tham số Speed cho Animator
            // Nếu bị dừng bởi tương tác, tốc độ sẽ là 0
            if (isStoppedByInteraction)
            {
                animator.SetFloat("Speed", 0f);
            }
            else
            {
                animator.SetFloat("Speed", agent.velocity.magnitude);
            }
        }

        // Chỉ tiếp tục logic di chuyển nếu không bị dừng bởi tương tác
        if (!isStoppedByInteraction)
        {
            // Nếu NPC đã đến đích
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                // Kiểm tra xem đã đến lúc di chuyển tiếp chưa
                if (Time.time >= nextMoveTime)
                {
                    SetRandomDestination();
                }
            }
        }
    }

    void SetRandomDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * walkRadius;
        randomDirection += transform.position;

        NavMeshHit hit;
        float samplePositionRadius = walkRadius * 1.5f;
        if (NavMesh.SamplePosition(randomDirection, out hit, samplePositionRadius, NavMesh.AllAreas))
        {
            randomDestination = hit.position;
            agent.SetDestination(randomDestination);
            nextMoveTime = Time.time + Random.Range(minWaitTime, maxWaitTime);
        }
        // else { Debug.LogWarning("Could not find a valid NavMesh point."); }
    }

    // THÊM: Hàm để dừng di chuyển của NPC
    public void StopMovement()
    {
        if (agent != null && agent.isOnNavMesh) // Kiểm tra agent có trên NavMesh không
        {
            agent.isStopped = true; // Dừng NavMeshAgent
            isStoppedByInteraction = true; // Đánh dấu là bị dừng bởi tương tác
            animator.SetFloat("Speed", 0f); // Đảm bảo animation là Idle
        }
        else
        {
            // Debug.LogWarning("NavMeshAgent not on NavMesh or null when trying to stop movement.");
        }
    }

    // THÊM: Hàm để tiếp tục di chuyển của NPC
    public void ResumeMovement()
    {
        if (agent != null && agent.isOnNavMesh)
        {
            isStoppedByInteraction = false; // Bỏ đánh dấu bị dừng bởi tương tác
            agent.isStopped = false; // Cho phép NavMeshAgent tiếp tục
            // Ngay lập tức đặt một điểm đến mới hoặc tiếp tục điểm cũ
            // Để tránh tình trạng đứng yên quá lâu
            if (agent.remainingDistance <= agent.stoppingDistance) // Nếu đang ở đích
            {
                SetRandomDestination(); // Đặt điểm đến mới ngay lập tức
            }
            // animator.SetFloat("Speed", agent.velocity.magnitude); // Sẽ được Update() xử lý
        }
        else
        {
            // Debug.LogWarning("NavMeshAgent not on NavMesh or null when trying to resume movement.");
        }
    }
}











*//*using UnityEngine;
using UnityEngine.AI; // Để sử dụng NavMeshAgent

public class NPCMovement : MonoBehaviour
{
    public NavMeshAgent agent; // Tham chiếu đến NavMeshAgent
    public Animator animator; // Tham chiếu đến Animator

    public float walkRadius = 10f; // Bán kính khu vực NPC có thể di chuyển
    public float minWaitTime = 2f; // Thời gian chờ tối thiểu giữa các lần di chuyển
    public float maxWaitTime = 5f; // Thời gian chờ tối đa

    private Vector3 randomDestination;
    private float nextMoveTime;

    void Start()
    {
        // Lấy tham chiếu đến NavMeshAgent và Animator nếu chưa được gán trong Inspector
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        // Đặt điểm đến ngẫu nhiên đầu tiên
        SetRandomDestination();
    }

    void Update()
    {
        // Cập nhật tham số Speed cho Animator dựa trên tốc độ thực tế của NavMeshAgent
        if (animator != null && agent != null)
        {
            // Sử dụng agent.velocity.magnitude để lấy tốc độ thực tế
            animator.SetFloat("Speed", agent.velocity.magnitude);
        }

        // Nếu NPC đã đến đích
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            // Kiểm tra xem đã đến lúc di chuyển tiếp chưa
            if (Time.time >= nextMoveTime)
            {
                SetRandomDestination();
            }
        }
    }

    void SetRandomDestination()
    {
        // Thay vì transform.position, có thể thử dùng agent.nextPosition
        // hoặc đơn giản là vị trí hiện tại nếu bạn muốn nó luôn tìm quanh NPC
        Vector3 randomDirection = Random.insideUnitSphere * walkRadius;
        randomDirection += transform.position; // Hoặc agent.nextPosition;

        NavMeshHit hit;
        // samplePositionRadius là bán kính để tìm điểm trên NavMesh xung quanh randomDirection
        // Nó nên lớn hơn walkRadius nếu walkRadius đã bao gồm toàn bộ khu vực khả dụng
        float samplePositionRadius = walkRadius * 1.5f; // Tăng bán kính lấy mẫu
        if (NavMesh.SamplePosition(randomDirection, out hit, samplePositionRadius, NavMesh.AllAreas))
        {
            randomDestination = hit.position;
            agent.SetDestination(randomDestination);
            nextMoveTime = Time.time + Random.Range(minWaitTime, maxWaitTime);
        }
        // else { Debug.LogWarning("Could not find a valid NavMesh point."); } // Debug nếu không tìm được
    }


    // Hàm để tìm một điểm đến ngẫu nhiên trong bán kính cho phép
    *//*  void SetRandomDestination()
      {
          Vector3 randomDirection = Random.insideUnitSphere * walkRadius;
          randomDirection += transform.position; // Điểm ngẫu nhiên quanh vị trí hiện tại của NPC

          NavMeshHit hit;
          // Tìm điểm gần nhất trên NavMesh
          if (NavMesh.SamplePosition(randomDirection, out hit, walkRadius, NavMesh.AllAreas))
          {
              randomDestination = hit.position;
              agent.SetDestination(randomDestination); // Đặt điểm đến cho NavMeshAgent

              // Đặt thời gian chờ tiếp theo
              nextMoveTime = Time.time + Random.Range(minWaitTime, maxWaitTime);
          }
      }*//*
}*/