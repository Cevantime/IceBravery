#if TOOLS
using Godot;
using System;

namespace GodotStates
{
    [Tool]
    public class Plugin : EditorPlugin
    {
        public override void _EnterTree()
        {
            var scriptState = GD.Load<Script>("addons/godot_states/StateMachine.cs");
            var textureState = GD.Load<Texture>("addons/godot_states/state_opt.svg");
            var scriptSwitchableState = GD.Load<Script>("addons/godot_states/SwitchableStateMachine.cs");
            var textureSwitchableState = GD.Load<Texture>("addons/godot_states/switchablestate_opt.svg");
            var scriptMultipleState = GD.Load<Script>("addons/godot_states/MultipleStateMachine.cs");
            var textureMultipleState = GD.Load<Texture>("addons/godot_states/multistate_opt.svg");
            AddCustomType("StateMachine", "Node", scriptState, textureState);
            AddCustomType("SwitchableStateMachine", "Node", scriptSwitchableState, textureSwitchableState);
            AddCustomType("MultipleStateMachine", "Node", scriptMultipleState, textureMultipleState);
        }

        public override void _ExitTree()
        {
            RemoveCustomType("StateMachine");
            RemoveCustomType("SwitchableStateMachine");
            RemoveCustomType("MultipleStateMachine");
        }
    }
}
#endif
