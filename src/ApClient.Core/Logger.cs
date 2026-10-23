using System;
using System.Collections.Generic;
using System.Text;

namespace ApClient.Core;

public class Logger
{
    public static List<Action<string>> callbacks { get; } = new List<Action<string>>();
    public static List<Action<string>> displayCallbacks { get; } = new List<Action<string>>();

    public static void Log(string message)
    {
        foreach (var callback in callbacks)
        {
            callback(message);
        }
    }

    public static void Display(string message)
    {
        foreach (var callback in displayCallbacks)
        {
            callback(message);
        }
    }

    public static void RegisterLogCallback(Action<string> callback)
    {
        callbacks.Add(callback);
    }

    public static void RegisterDisplayCallback(Action<string> callback)
    {
        displayCallbacks.Add(callback);
    }
}
