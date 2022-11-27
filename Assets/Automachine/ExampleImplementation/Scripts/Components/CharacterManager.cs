using UnityEngine;
using Zenject;

using Automachine.Scripts.Components;

public class CharacterManager : AutomachineEntity<CharacterState>
{
    public int xdd = 0;
    public int fdsfsdf = 0;

    public override void Initialize()
    {
        base.Initialize();
        if (stateMachine.IsReady)
        {
            if (!stateMachine.HasTransition(CharacterState.Idle, CharacterState.Walking, () => xdd > 0 && fdsfsdf == 0))
            {
                stateMachine.AddTransition(CharacterState.Idle, CharacterState.Walking, () => xdd > 0 && fdsfsdf == 0);
            }

            stateMachine.AddTransition(CharacterState.Walking, CharacterState.Dead, () => xdd > 10 && fdsfsdf < 0);
            stateMachine.AddAnyStateTransition(CharacterState.Idle, () => xdd > 12);
        }
    }

    protected override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.Y))
        {
            xdd++;
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            fdsfsdf--;
        }
    }


}
