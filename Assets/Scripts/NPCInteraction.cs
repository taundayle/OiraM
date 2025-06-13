using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NPCInteraction : MonoBehaviour
{
    public GameObject interactButton;
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public Button skipButton;

    private bool playerInRange = false;
    private bool inDialogue = false;

    [TextArea(3, 10)]
    public string[] dialogueLines;
    private int currentDialogueIndex = 0;

    // THÊM: Tham chiếu đến script NPCMovement
    private NPCMovement npcMovement;

    void Start()
    {
        // Lấy tham chiếu đến NPCMovement khi khởi tạo
        npcMovement = GetComponent<NPCMovement>();

        if (interactButton != null)
        {
            interactButton.SetActive(false);
        }
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }

        if (skipButton != null)
        {
            skipButton.onClick.AddListener(SkipDialogue);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (playerInRange && !inDialogue)
            {
                StartDialogue();
            }
            else if (inDialogue)
            {
                SkipDialogue();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (!inDialogue && interactButton != null)
            {
                interactButton.SetActive(true);
            }

            // THÊM: Khi Player vào tầm, yêu cầu NPC dừng lại
            if (npcMovement != null)
            {
                npcMovement.StopMovement();
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (interactButton != null)
            {
                interactButton.SetActive(false);
            }
            if (inDialogue)
            {
                EndDialogue();
            }

            // THÊM: Khi Player rời đi, yêu cầu NPC tiếp tục di chuyển
            if (npcMovement != null)
            {
                npcMovement.ResumeMovement();
            }
        }
    }

    void StartDialogue()
    {
        inDialogue = true;
        if (interactButton != null)
        {
            interactButton.SetActive(false);
        }
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(true);
        }

        // Đảm bảo NPC đứng yên hoàn toàn khi bắt đầu hội thoại
        if (npcMovement != null)
        {
            npcMovement.StopMovement(); // Gọi lại để chắc chắn
        }

        currentDialogueIndex = 0;
        DisplayDialogueLine();
    }

    void DisplayDialogueLine()
    {
        if (currentDialogueIndex < dialogueLines.Length)
        {
            if (dialogueText != null)
            {
                dialogueText.text = dialogueLines[currentDialogueIndex];
            }
        }
        else
        {
            EndDialogue();
        }
    }

    void SkipDialogue()
    {
        currentDialogueIndex++;
        DisplayDialogueLine();
    }

    void EndDialogue()
    {
        inDialogue = false;
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }

        // THÊM: Khi hội thoại kết thúc, nếu Player vẫn trong tầm, hiện lại nút
        // Và nếu Player đã rời đi, NPC sẽ tự động tiếp tục di chuyển (được xử lý bởi OnTriggerExit)
        if (playerInRange && interactButton != null)
        {
            interactButton.SetActive(true);
        }
        // Nếu Player đã rời đi trước khi hội thoại kết thúc (trường hợp hiếm)
        else if (!playerInRange && npcMovement != null)
        {
            npcMovement.ResumeMovement();
        }
    }
}








/*using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NPCInteraction : MonoBehaviour
{
    public GameObject interactButton;
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public Button skipButton;

    private bool playerInRange = false;
    private bool inDialogue = false;

    [TextArea(3, 10)]
    public string[] dialogueLines;
    private int currentDialogueIndex = 0;

    void Start()
    {

        if (interactButton != null)
        {
            interactButton.SetActive(false);
        }
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }


        if (interactButton != null)
        {
            interactButton.GetComponent<Button>().onClick.AddListener(StartDialogue);
        }
        if (skipButton != null)
        {
            skipButton.onClick.AddListener(SkipDialogue);
        }
    }

    void Update()
    {
        // Nếu người chơi nhấn phím E
        if (Input.GetKeyDown(KeyCode.E))
        {
            // Trường hợp 1: Player ở gần NPC và CHƯA trong hội thoại
            if (playerInRange && !inDialogue)
            {
                StartDialogue(); // Bắt đầu hội thoại
            }
            // Trường hợp 2: Đang trong hội thoại
            else if (inDialogue)
            {
                SkipDialogue(); // Bỏ qua dòng hội thoại
            }
        }
    }

    // Khi Player bước vào vùng Trigger của NPC
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Đảm bảo Player của bạn có Tag là "Player"
        {
            playerInRange = true;
            if (!inDialogue && interactButton != null)
            {
                interactButton.SetActive(true);
            }
        }
    }

    // Khi Player rời khỏi vùng Trigger của NPC
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (interactButton != null)
            {
                interactButton.SetActive(false);
            }
            if (inDialogue)
            {
                EndDialogue();
            }
        }
    }

    void StartDialogue()
    {
        inDialogue = true;
        if (interactButton != null)
        {
            interactButton.SetActive(false);
        }
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(true);
        }

        currentDialogueIndex = 0;
        DisplayDialogueLine();
    }

    void DisplayDialogueLine()
    {
        if (currentDialogueIndex < dialogueLines.Length)
        {
            if (dialogueText != null)
            {
                dialogueText.text = dialogueLines[currentDialogueIndex];
            }
        }
        else
        {
            EndDialogue();
        }
    }

    void SkipDialogue()
    {
        currentDialogueIndex++;
        DisplayDialogueLine();
    }

    void EndDialogue()
    {
        inDialogue = false;
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }

        if (playerInRange && interactButton != null)
        {
            interactButton.SetActive(true);
        }
    }
}*/