using UnityEngine;
using Zenject;

using Automachine.Scripts.Components;

public class Idle : State<CharacterState>
{

    [Inject] private readonly Animator animator;

    public override void StartState()
    {
        base.StartState();
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

