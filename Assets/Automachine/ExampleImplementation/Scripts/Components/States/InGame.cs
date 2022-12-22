using UnityEngine;
using Zenject;

using Automachine.Scripts.Components;

public class InGame : State<GameState>
{
    public override void StartState()
    {
        base.StartState();
        Debug.Log("ingame state started");
    }

}

