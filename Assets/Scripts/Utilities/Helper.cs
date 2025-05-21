using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    public class Helper : MonoBehaviour
    {
        [Range(0, 1)]
        public float vertical;

        [Range(-1, 1)]
        public float horizontal;

        public bool playAnim;
        public string[] oh_attack;
        public string[] th_attack;


        public bool twoHanded;
        public bool enableRM;
        public bool useItem;
        public bool interacting;
        public bool lookon;

        Animator anim;
        void Start()
        {
            anim = GetComponent<Animator>();
        }
        void Update()
        {
            enableRM = !anim.GetBool("canMove");
            anim.applyRootMotion = enableRM;

            interacting = anim.GetBool("interacting");

            if (lookon == false)
            {
                horizontal = 0;
                vertical = Mathf.Clamp01(vertical);
            }

            anim.SetBool("lockon", lookon);

            if (enableRM)
                return;


            if(useItem)
            {
                anim.Play("drink");
                useItem = false;
            }

            if (interacting)
            {
                playAnim = false;
                vertical = Mathf.Clamp(vertical, 0, 0.5f);
            }
            anim.SetBool("TwoHanded", twoHanded);

            if (playAnim)
            {
                string targetAnim;

                if (!twoHanded)
                {
                    int r = Random.Range(0, oh_attack.Length);
                    targetAnim = oh_attack[r];
                    if(vertical > 0.5f )
                        targetAnim = "attack6";
                }
                else
                {
                    
                    int r = Random.Range(0, th_attack.Length);
                    targetAnim = th_attack[r];
                }

                vertical = 0;
                anim.CrossFade(targetAnim, 0.2f);
                //anim.SetBool("canMove", false);
                //enableRM = true;

                playAnim = false;
            }
            anim.SetFloat("Vertical", vertical);
            anim.SetFloat("Horizontal", horizontal);
        }
    }
}
