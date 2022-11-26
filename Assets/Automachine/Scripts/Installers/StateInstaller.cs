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

    [SerializeField] private AutomachineDebugSettings debugSettings = new AutomachineDebugSettings();

    private readonly List<Type> selectedEnumTypes = new List<Type>();

    public override void InstallBindings()
    {
        InstallStateMachine();
    }

    private void InstallStateMachine()
    {
        Container.BindInstance(debugSettings).AsCached().NonLazy();
        InstallStates<CharacterState>(typeof(CharacterState));
    }

    public void InstallStates<TState>(Type currentType) where TState : Enum
    {
        if (currentType == null)
        {
            AutomachineLogger.LogError("Provided type is null! Canceling state machine creation");
            return;
        }
        else
        {
            if (debugSettings.logFoundMatchingEnums)
            {
                AutomachineLogger.Log("Found entity of type <color=white>" + currentType.Name + "</color> that matches Automachine criteria. Creating a state machine...");
            }
            //Container.BindInstance(currentType).WhenInjectedInto(typeof(TransitionsManager<TState>));
        }

        Array fields = currentType.GetEnumValues();

        foreach (var currentField in fields)
        {
            TState state = ConvertObjectTo<TState>(currentField);
            FieldInfo field = currentType.GetField(state.ToString());

            if (field.IsDefined(typeof(StateEntityAttribute), false))
            {
                if (debugSettings.logCreatingStates)
                {
                    AutomachineLogger.Log("Creating state: <color=white>" + state + "</color>");
                }

                if (field.IsDefined(typeof(DefaultStateAttribute), false))
                {
                    Container.BindInstance(state).WithId("AutomachineDefaultState").WhenInjectedInto(typeof(AutomachineCore<TState>));
                    AutomachineLogger.Log("Binding default state <color=white>" + state + "</color>");
                }
                Type baseClassType = field.GetCustomAttribute<StateEntityAttribute>().BaseClassType;
                Container.BindInterfacesAndSelfTo(baseClassType).FromNewComponentOnNewGameObject().AsCached().NonLazy();
                Container.BindInstance(state).WhenInjectedInto(baseClassType);
            }
        }
        Container.BindInterfacesAndSelfTo<AutomachineCore<TState>>().FromNew().AsCached();
        Container.BindInterfacesAndSelfTo<AutomachineEntity<TState>>().FromComponentInHierarchy().AsCached();
        Container.BindInterfacesAndSelfTo<StateManager<TState>>().FromNew().AsCached();
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
