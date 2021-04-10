using Godot;
using System;

namespace GodotStates
{
    public class SwitchableStateMachine : StateMachine
    {
        protected StateMachine selectedState;

        public override bool _Supports(Node node)
        {
            return true;
        }

        public override void _EnterState(StateMachine previous, params object[] list)
        {
            if (selectedState == null)
            {
                foreach (Node n in GetChildren())
                {
                    StateMachine s = (StateMachine)n;
                    if (s._Supports(Referer) && !s.Disabled)
                    {
                        ChangeState(s.Name);
                        break;
                    }
                    else
                    {
                        GD.PrintErr("state " + s.Name + " has been added to " + Referer.Name + " but doesn't support it!");
                    }
                }
            }
            else if (!selectedState.Disabled)
            {
                selectedState.Enabled = true;
                selectedState._EnterState(previous);
            }
        }

        public override void ChangeState(string stateName, params object[] list)
        {
            StateMachine next = GetNode<StateMachine>(stateName);
            if (selectedState != null)
            {
                if (selectedState == next || selectedState.Disabled)
                {
                    return;
                }
                selectedState.Enabled = false;
                selectedState._ExitState(next);
            }

            StateMachine previous = selectedState;

            if (next._Supports(Referer))
            {
                selectedState = next;
                selectedState.Enabled = Enabled;
                selectedState._EnterState(previous, list);
            }
        }

        public override void _ExitState(StateMachine next)
        {
            if (selectedState != null)
            {
                selectedState.Enabled = false;
                selectedState._ExitState(next);
            }
        }
    }

}