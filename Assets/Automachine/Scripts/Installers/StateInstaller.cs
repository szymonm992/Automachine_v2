using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

using Automachine.Scripts.Attributes;
using Automachine.Scripts;
using Automachine.Scripts.Components;
using Automachine.Scripts.Models;
using System.ComponentModel;
using Automachine.Scripts.Interfaces;

public class StateInstaller : MonoInstaller
{
    private readonly List<Type> selectedEnumTypes = new List<Type>();

    public override void InstallBindings()
    {
        InstallStateMachine();
    }

    private void InstallStateMachine()
    {
        InstallStates<CharacterState>(typeof(CharacterState));
    }

    public void InstallStates<TState>(Type currentType) where TState : Enum
    {
        if (currentType != null)
        {
            Container.BindInterfacesAndSelfTo<AutomachineEntity<TState>>().FromComponentsInHierarchy().AsCached();

            Array fields = currentType.GetEnumValues();

            foreach (var currentField in fields)
            {
                TState state = ConvertObjectTo<TState>(currentField);
                FieldInfo field = currentType.GetField(state.ToString());

                if (field.IsDefined(typeof(StateEntityAttribute), false))
                {
                    Type baseClassType = field.GetCustomAttribute<StateEntityAttribute>().BaseClassType;
                    Container.BindInterfacesAndSelfTo(baseClassType).FromNewComponentOnNewGameObject().AsCached().NonLazy();
                }
            }
            Container.BindInterfacesAndSelfTo<AutomachineCore<TState>>().FromNew().AsCached();
            Container.BindInterfacesAndSelfTo<AutomachineEntity<TState>>().FromComponentsInHierarchy().AsCached();
        }
    }

    private void SearchForEnumWithAttribute<T>() where T : Attribute
    {
        foreach (Type enumType in Assembly.GetExecutingAssembly().GetTypes()
                  .Where(x => x.IsSubclassOf(typeof(Enum)) &&
                  x.GetCustomAttribute<T>() != null))
        {
            selectedEnumTypes.Add(enumType);
        }
    }

    private T ConvertObjectTo<T>(object input)
    {
        return (T)Convert.ChangeType(input, typeof(T));
    }
}
