using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    public class EnemyTarget : MonoBehaviour
    {
        public int index;
        public List<Transform> targets = new List<Transform>();
        public List<HumanBodyBones> h_Bones = new List<HumanBodyBones>();

        Animator anim;

        void Start()
        {
            anim = GetComponent<Animator>();
            if(anim.isHuman == false)
            {
                return;
            }

            for(int i = 0; i < h_Bones.Count; i++)
            {
                targets.Add(anim.GetBoneTransform(h_Bones[i])); 
            }
        }

        public Transform GetTarget()
        {
            if(targets.Count == 0)
            {
                return transform;
            }

            int targetIndex = index;

            if(index < targets.Count -1)
            {
                index++;
            }
            else
            {
                index = 0;
                targetIndex = 0;
            }
            return targets[index];
        }
    }
}
