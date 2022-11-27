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

    public override void InstallBindings()
    {
        InstallStateMachine();
    }

    private void InstallStateMachine()
    {
        var enumsOnGameObject = SearchForEnumWithAttributeOnGameObject<AutomachineStatesAttribute>();
        Container.BindInstance(debugSettings).AsCached().NonLazy();
        //InstallStates<CharacterState>(typeof(CharacterState));

        foreach (Type type in enumsOnGameObject)
        {
            var methodInfoGameStates = typeof(StateInstaller).GetMethod("InstallStates");//needs a public void
            var gameStatesLauncher = methodInfoGameStates.MakeGenericMethod(type);
            object[] args = { type };

            gameStatesLauncher.Invoke(this, args);
        }
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
            Container.BindInstance(currentType).WhenInjectedInto<TransitionsManager<TState>>();
        }

        Container.BindInterfacesAndSelfTo<TransitionsManager<TState>>().FromNew().AsCached().NonLazy();

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
                    if (debugSettings.logBindingDefaultStates)
                    {
                        AutomachineLogger.Log("Binding default state <color=white>" + state + "</color>");
                    }
                }

                Type baseClassType = field.GetCustomAttribute<StateEntityAttribute>().BaseClassType;
                Container.BindInterfacesAndSelfTo(baseClassType).FromNewComponentOnNewGameObject().AsCached().NonLazy();
                Container.BindInstance(state).WhenInjectedInto(baseClassType);
            }
        }
        Container.BindInterfacesAndSelfTo<StateManager<TState>>().FromNew().AsCached();
        Container.BindInterfacesAndSelfTo<AutomachineCore<TState>>().FromNew().AsCached();
        Container.BindInterfacesAndSelfTo<AutomachineEntity<TState>>().FromComponentInHierarchy().AsCached();
    }

    private IEnumerable<Type> SearchForEnumWithAttributeOnGameObject<T>() where T : Attribute
    {
        List<Type> selectedEnumTypes = new();
        foreach (Type enumType in Assembly.GetExecutingAssembly().GetTypes()
                  .Where(x => x.IsSubclassOf(typeof(Enum)) &&
                  x.GetCustomAttribute<T>() != null))
        {
            var methodInfoValidation = typeof(StateInstaller).GetMethod("ValidateGameObjectHasEntity");
            var entityValidator = methodInfoValidation.MakeGenericMethod(enumType);
            object[] args = { enumType, selectedEnumTypes };

            entityValidator.Invoke(this, args);
        }
        return selectedEnumTypes;
    }

    public void ValidateGameObjectHasEntity<TState>(Type type, List<Type> selectedEnumTypes) where TState : Enum
    {
        if (gameObject.GetComponent<AutomachineEntity<TState>>() != null)
        {
            selectedEnumTypes.Add(type);
        }
    }

    private T ConvertObjectTo<T>(object input)
    {
        return (T)Convert.ChangeType(input, typeof(T));
    }
}
