using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    public class StateManager : MonoBehaviour
    {
        [Header("Inits")]
        public GameObject activeModel;

        [Header("Inputs")]
        public float horizontal;
        public float vertical;
        public float moveAmount;
        public Vector3 moveDir;
        public bool rt, rb, lt, lb;

        [Header("Stats")]
        public float moveSpeed = 2;
        public float runSpeed = 3.5f;
        public float rotateSpeed = 5;
        public float toGround = 0.5f;

        [Header("States")]
        public bool onGround;
        public bool run;
        public bool lockOn;
        public bool inAction;
        public bool canMove;
        public bool isTwoHanded;

        [Header("Other")]
        public EnemyTarget lockOnTarget;

        [HideInInspector]
        public Animator anim;
        [HideInInspector]
        public Rigidbody rigid;
        [HideInInspector]
        public AnimatorHook a_hook;
        [HideInInspector]
        public ActionManager actionManager;
        [HideInInspector]
        public InventoryManager inventoryManager;

        [HideInInspector]
        public float delta;
        [HideInInspector]
        public LayerMask ignoreLayer;

        float _actionDelay;

        public void Init()
        {
            SetUpAnimator();
            rigid = GetComponent<Rigidbody>();
            rigid.angularDrag = 999;
            rigid.drag = 4;
            rigid.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

            inventoryManager = GetComponent<InventoryManager>();
            inventoryManager.Init();

            actionManager = GetComponent<ActionManager>();
            actionManager.Init(this);

            a_hook = activeModel.AddComponent<AnimatorHook>();
            a_hook.Init(this);

            gameObject.layer = 8;
            ignoreLayer = ~(1 << 9);

            anim.SetBool("onGround", true);
        }
        void SetUpAnimator()
        {
            if(activeModel == null)
            {
                anim = GetComponentInChildren<Animator>();
                if (anim == null)
                {
                    Debug.Log("No model found");
                }
                else
                {
                    activeModel = anim.gameObject;
                }
            }
            if(anim == null)
                anim = activeModel.GetComponent<Animator>();

            anim.applyRootMotion = false;
        }

        public void FixedTick(float d)
        { 
            delta = d;

            DetectAction();

            if (inAction)
            {
                anim.applyRootMotion = true;

                _actionDelay += delta;
                if (_actionDelay > 0.3f)
                {
                    inAction = false;
                    _actionDelay = 0;
                }
                else
                {
                    return;

                }

            }

            canMove = anim.GetBool("canMove");


            if (!canMove)
                return;
            
            anim.applyRootMotion = false;
            rigid.drag = (moveAmount > 0 || onGround == false) ? 0 : 4;

            float targetSpeed = moveSpeed;
            if(run)
                targetSpeed = runSpeed;

            if(onGround)
                rigid.velocity = moveDir * (targetSpeed * moveAmount);

            if(run)
                lockOn = false; 

            Vector3 targetDir = (lockOn == false)? moveDir : lockOnTarget.transform.position - transform.position;
            targetDir.y = 0;
            if(targetDir == Vector3.zero)
                targetDir = transform.forward;
            Quaternion tr = Quaternion.LookRotation(targetDir);
            Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, delta * moveAmount * rotateSpeed);
            transform.rotation = targetRotation;

            anim.SetBool("lockon", lockOn);

            if (lockOn == false)
                HandleMovementAnimations();
            else
            {
                HandleLockOnAnimations(moveDir);
            }
        }

        public void DetectAction()
        {
            if (canMove == false)
                return;

            if(rb == false && rt == false && lt == false && lb == false)
                return;



            string targetAnim = null;

            Action slot = actionManager.GetActionSlot(this);
            if(slot == null)
                return;
            targetAnim = slot.targetAnim;

            if (string.IsNullOrEmpty(targetAnim))
                return;

            canMove = false;
            inAction = true;
            anim.CrossFade(targetAnim, 0.2f);
            //rigid.velocity = Vector3.zero;
        }

        public void Tick(float d)
        {
            delta = d;
            onGround = OnGround();
            anim.SetBool("onGround", onGround);
        }
        void HandleMovementAnimations()
        {
            anim.SetBool("run", run);
            anim.SetFloat("Vertical", moveAmount, 0.4f, delta);
        }

        void HandleLockOnAnimations(Vector3 moveDir)
        {
            Vector3 relativeDir = transform.InverseTransformDirection(moveDir);
            float h = relativeDir.x;
            float v = relativeDir.z;

            anim.SetFloat("Vertical", v, 0.2f, delta);
            anim.SetFloat("Horizontal", h, 0.2f, delta);
        }

        public bool OnGround()
        {
            bool r = false;

            Vector3 orgin = transform.position + (Vector3.up * 0.1f * toGround);
            Vector3 dir = -Vector3.up;
            float dis = toGround + 0.3f;
            RaycastHit hit;
            Debug.DrawRay(orgin, dir * dis);
            if (Physics.Raycast(orgin, dir, out hit, dis, ignoreLayer))
            {
                r = true;
                Vector3 targerPosition = hit.point;
                transform.position = targerPosition;
            }
            else
            {
                r = false;
            }

            return r;
        }

        public void HandleTwoHanded()
        {
            anim.SetBool("two_handed", isTwoHanded); 

            if(isTwoHanded)
            {
                actionManager.UpdateAcionTwoHanded();
            }
            else
            {
                actionManager.UpdateActionOneHanded();
            }
        }

    }

}
