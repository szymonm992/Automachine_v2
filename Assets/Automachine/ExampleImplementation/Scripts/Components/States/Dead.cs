using UnityEngine;
using Zenject;

using Automachine.Scripts.Components;

public class Dead : State<CharacterState>
{
    public override void StartState()
    {
        base.StartState();
        Debug.Log("dead state started ");
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

