using Godot;
using System;
using Godot.Collections;

namespace GodotStates
{
    public class StateMachine : Node
    {
        private Node referer;

        public Node Referer
        {
            get { return referer; }
        }

        private bool enabled;
        public bool Enabled
        {
            get
            {
                return enabled;
            }
            set
            {
                enabled = value;
                SetPhysicsProcess(enabled);
                SetProcess(enabled);
                SetProcessInput(enabled);
                SetProcessUnhandledInput(enabled);
                if (referer != null && referer.HasSignal("_forces_integrated"))
                {
                    if (enabled)
                    {
                        referer.Connect("_forces_integrated", this, nameof(_IntegrateForces));
                    }
                    else
                    {
                        referer.Disconnect("_forces_integrated", this, nameof(_IntegrateForces));
                    }
                }
            }
        }

        private bool disabled = false;

        [Export]
        public bool Disabled
        {
            get
            {
                return disabled;
            }
            set
            {
                disabled = value;
                if (referer == null)
                {
                    return;
                }
                if (Enabled && disabled)
                {
                    Enabled = false;
                    CallDeferred("_ExitState", new object[] { null, new object[] { } });
                }
                else if (!disabled && !enabled)
                {
                    if (ShouldBeEnabledByDefault())
                    {
                        Enabled = true;
                        CallDeferred("_EnterState", new object[] { null, new object[] { } });
                    }
                    else
                    {
                        Enabled = false;
                    }
                }
            }
        }

        private bool ShouldBeEnabledByDefault()
        {
            Node parent = GetParent();
            return (!(parent is StateMachine) && _Supports(parent) || (parent is MultipleStateMachine && ((MultipleStateMachine)parent).Enabled && _Supports(Referer)));
        }

        public override void _EnterTree()
        {
            Node parent = GetParent();
            enabled = ShouldBeEnabledByDefault();
            if (enabled)
            {
                referer = parent;
            }
            else if (parent is StateMachine)
            {
                referer = ((StateMachine)parent).referer;
            }
        }
        public override void _Ready()
        {
            Enabled = enabled;
            if (Enabled)
            {
                CallDeferred("_EnterState", new object[] { null, new object[] { } });
            }
        }

        public virtual void ChangeState(string stateName, params object[] list)
        {
            Node parent = this;
            do
            {
                parent = parent.GetParent();
            } while (parent != null && !(parent is SwitchableStateMachine));

            if (parent != null)
            {
                ((SwitchableStateMachine)parent).ChangeState(stateName, list);
            }

        }

        public Array<StateMachine> GetRefererStatesInGroup(string groupName)
        {
            Array<StateMachine> refererStates = new Array<StateMachine>();
            foreach (Node n in GetTree().GetNodesInGroup(groupName))
            {
                StateMachine st;
                if (n is StateMachine && (st = (StateMachine)n).Referer == Referer)
                {
                    refererStates.Add(st);
                }
            }
            return refererStates;
        }

        public void DisableGroup(string groupName, bool disabled = true)
        {
            foreach (StateMachine st in GetRefererStatesInGroup(groupName))
            {
                st.Disabled = disabled;
            }
        }

        public virtual bool _Supports(Node node)
        {
            return false;
        }

        public virtual void _EnterState(StateMachine previous, params object[] list)
        {

        }
        public virtual void _ExitState(StateMachine next)
        {

        }

        public virtual void _IntegrateForces(Godot.Object state)
        {

        }
    }
}
