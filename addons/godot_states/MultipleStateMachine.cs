using Godot;
using System;

namespace GodotStates
{
    public class MultipleStateMachine : StateMachine
    {
        public override bool _Supports(Node node)
        {
            return true;
        }

        public override void _EnterState(StateMachine previous, params object[] list)
        {
            foreach (Node n in GetChildren())
            {
                StateMachine s = (StateMachine)n;
                if (!(s._Supports(Referer)) || s.Disabled)
                {
                    continue;
                }
                s.Enabled = true;
                s._EnterState(previous);
            }
        }

        public override void _ExitState(StateMachine next)
        {
            foreach (Node n in GetChildren())
            {
                if (!(n is StateMachine))
                {
                    continue;
                }

                StateMachine s = (StateMachine)n;
                s.Enabled = false;
                s._ExitState(next);
            }
        }
    }

}
