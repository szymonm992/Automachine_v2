using UnityEngine;
using Zenject;

using Automachine.Scripts.Components;
using Automachine.Scripts.Signals;

public class GameStateManager : AutomachineEntity<GameState>
{

    public override void OnStateMachineInitialized(OnStateMachineInitialized<GameState> OnStateMachineInitialized)
    {
        base.OnStateMachineInitialized(OnStateMachineInitialized);
        if (!stateMachine.HasTransition(GameState.Lobby, GameState.InGame))
        {
            stateMachine.AddAnyStateTransition(GameState.InGame, () => stateMachine != null);
        }
    }

    protected override void Update()
    {
        base.Update();
    }


}

