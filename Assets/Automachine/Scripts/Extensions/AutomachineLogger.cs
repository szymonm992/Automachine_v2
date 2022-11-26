using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Automachine.Scripts
{
    public static class AutomachineLogger
    {
        public static void Log(string content)
        {
            Debug.Log("<b><color=orange>[Automachine]</color></b> " + content);
        }

        public static void Log(string title, string content)
        {
            Debug.Log("<b><color=orange>[" + title + "]</color></b> " + content);
        }

        public static void LogError(string content)
        {
            Debug.LogError("<b><color=red>[Automachine]</color></b> " + content);
        }

    }
}
