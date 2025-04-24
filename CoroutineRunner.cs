using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ApClient;

public class CoroutineRunner : MonoBehaviour
{

    private static readonly Queue<Action> _mainThreadQueue = new Queue<Action>();

    private static CoroutineRunner _instance;

    public static CoroutineRunner Instance
    {
        get
        {
            if (_instance == null)
            {
                var go = new GameObject("CoroutineRunner");
                UnityEngine.Object.DontDestroyOnLoad(go);
                _instance = go.AddComponent<CoroutineRunner>();
            }
            return _instance;
        }
    }

    public static void RunOnMainThread(Action action)
    {
        var _ = Instance;
        lock (_mainThreadQueue)
        {
            _mainThreadQueue.Enqueue(action);
        }
    }

    private void Update()
    {
        lock (_mainThreadQueue)
        {
            while (_mainThreadQueue.Count > 0)
            {
                _mainThreadQueue.Dequeue().Invoke();
            }
        }
    }
}
