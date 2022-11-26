using UnityEngine;
using Zenject;

using Automachine.Scripts.Components;

public class Idle : State<CharacterState>
{
    public override void Start()
    {
        base.Start();
        Debug.Log("idle state started for entity "+ stateMachine.ConnectedEntity.gameObject.name);
    }

    public override void Tick()
    {
        base.Tick();
    }

    public override void Initialize()
    {
        base.Initialize();
    }

}

