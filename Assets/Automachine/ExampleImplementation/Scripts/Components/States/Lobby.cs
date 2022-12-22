using UnityEngine;
using Zenject;

using Automachine.Scripts.Components;

public class Lobby : State<GameState>
{
    public override void StartState()
    {
        base.StartState();
        Debug.Log("lobby started for: " + stateMachine.ConnectedEntity.gameObject.name);
    }

}

