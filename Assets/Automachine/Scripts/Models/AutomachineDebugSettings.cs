using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Automachine.Scripts.Models
{
    [System.Serializable]
    public class AutomachineDebugSettings
    {
        [Header("Installer logs")]
        public bool logFoundMatchingEnums = true;
        public bool logCreatingStates = true;
        public bool logBindingDefaultStates = true;

        [Header("Automachine core")]
        public bool logConnectedMonoType = true;

        [Header("State manager")]
        public bool logCreatedStatesAmount = true;
        public bool logLaunchingDefaultState = true;
        public bool logSwitchingState = true;
    }
}
