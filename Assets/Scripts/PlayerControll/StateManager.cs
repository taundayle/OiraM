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

        [Header("Stats")]
        public float moveSpeed = 2;
        public float runSpeed = 3.5f;
        public float rotateSpeed = 5;
        public float toGround = 0.5f;

        [Header("States")]
        public bool onGround;
        public bool run;
        public bool lockOn;

        [HideInInspector]
        public Animator anim;
        [HideInInspector]
        public Rigidbody rigid;

        [HideInInspector]
        public float delta;
        [HideInInspector]
        public LayerMask ignoreLayer;

        public void Init()
        {
            SetUpAnimator();
            rigid = GetComponent<Rigidbody>();
            rigid.angularDrag = 999;
            rigid.drag = 4;
            rigid.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

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

            rigid.drag = (moveAmount > 0 || onGround == false) ? 0 : 4;

            float targetSpeed = moveSpeed;
            if(run)
                targetSpeed = runSpeed;

            if(onGround)
                rigid.velocity = moveDir * (targetSpeed * moveAmount);

            if(run)
                lockOn = false; 

            if (!lockOn)
            {
                Vector3 targetDir = moveDir;
                targetDir.y = 0;
                if(targetDir == Vector3.zero)
                    targetDir = transform.forward;
                Quaternion tr = Quaternion.LookRotation(targetDir);
                Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, delta * moveAmount * rotateSpeed);
                transform.rotation = targetRotation;
            }


            HandleMovementAnimator();
        }

        public void Tick(float d)
        {
            delta = d;
            onGround = OnGround();
            anim.SetBool("OnGround", onGround);
        }
        void HandleMovementAnimator()
        {
            anim.SetBool("run", run);
            anim.SetFloat("Vertical", moveAmount, 0.4f, delta);
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

    }

}
