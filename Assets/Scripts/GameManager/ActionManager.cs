using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace SA
{
    public class ActionManager : MonoBehaviour
    {
        public List<Action> actionslot = new List<Action>();

        StateManager states;
        public void Init(StateManager st)
        {
            states = st;
            UpdateActionOneHanded();
        }

        public void UpdateActionOneHanded()
        {
            EmptyAllSlots();
            Weapon w = states.inventoryManager.curWeapon;

            for (int i = 0; i < w.actions.Count; i++)
            {
                Action a = GetAction(w.actions[i].input);
                a.targetAnim = w.actions[i].targetAnim;
            }
        }
        public void UpdateAcionTwoHanded()
        {
            EmptyAllSlots();
            Weapon w = states.inventoryManager.curWeapon;

            for (int i = 0; i < w.two_handedActions.Count; i++)
            {
                Action a = GetAction(w.two_handedActions[i].input);
                a.targetAnim = w.two_handedActions[i].targetAnim;
            }
        }

        void EmptyAllSlots()
        {
            for (int i = 0; i < 4; i++)
            {
                Action a =GetAction((ActionInput)i);
                a.targetAnim = null;
            }
        }

        ActionManager()
        {
            for(int i = 0; i < 4; i++)
            {
                Action a = new Action();
                a.input = (ActionInput)i;
                actionslot.Add(a);
            }

        }

        public Action GetActionSlot(StateManager st)
        {
            ActionInput a_input = GetActionInput(st);
            return GetAction(a_input);
        }

        Action GetAction(ActionInput inp)
        {
            for(int i = 0; i < actionslot.Count; i++)
            {
                if (actionslot[i].input == inp)
                    return actionslot[i];
            }
            return null;
        }

        public ActionInput GetActionInput(StateManager st)
        {
            if(st.rb)
                return ActionInput.rb;
            if (st.rt)
                return ActionInput.rt;
            if (st.lb)
                return ActionInput.lb;
            if (st.lt)
                return ActionInput.lt;
            return ActionInput.rb;
        }
    }

    public enum ActionInput
    {
        rb, lb, rt, lt, 
    }

    [System.Serializable]
    public class Action
    {
        public ActionInput input;
        public string targetAnim;
    }
}
