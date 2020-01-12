using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;


public class Debugging
{
    [Conditional("UNITY_EDITOR")]
    public static void Log(object msg)
    {
        UnityEngine.Debug.Log(msg);
    }
    [Conditional("UNITY_EDITOR")]
    public static void LogSystem(object msg)
    {
        UnityEngine.Debug.Log("<color='green'>=====<System>=====</color>");
        UnityEngine.Debug.Log("<color='green'>" + msg + "</color>");
    }
    [Conditional("UNITY_EDITOR")]
    public static void LogSystemWarning(object msg)
    {
        UnityEngine.Debug.Log("<color='red'>=====<SystemWarning>=====</color>");
        UnityEngine.Debug.Log("<color='red'>" + msg + "</color>");
    }
    [Conditional("UNITY_EDITOR")]
    public static void LogWarning(object msg)
    {
        UnityEngine.Debug.LogWarning("<color=yellow>" + msg + "</color>");
    }
    [Conditional("UNITY_EDITOR")]
    public static void LogError(object msg)
    {
        UnityEngine.Debug.LogError("<color=red>" + msg + "</color>");
    }
}
