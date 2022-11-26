using UnityEngine;
using Zenject;

using Automachine.Scripts.Components;

public class Walking : State<CharacterState>
{

    public override void StartState()
    {
        base.StartState();
        Debug.Log("walking state started ");
    }

}

