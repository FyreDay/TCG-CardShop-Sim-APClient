using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace ApClient;

public class Util : MonoBehaviour
{
    private static readonly Queue<Action> _mainThreadQueue = new Queue<Action>();

    private static Util _instance;

    public static Util Instance
    {
        get
        {
            if (_instance == null)
            {
                var go = new GameObject("CoroutineRunner");
                DontDestroyOnLoad(go);
                _instance = go.AddComponent<Util>();
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

    public static CardData CardRoller(ECollectionPackType collectionPackType)
    {
        ECardExpansionType expansionType = UnityEngine.Random.Range(0F, 1F) > 0.5 ? ECardExpansionType.Tetramon : ECardExpansionType.Destiny;
        return new CardData
        {
            isFoil = UnityEngine.Random.Range(0F, 1F) > 0.5,
            isDestiny = expansionType == ECardExpansionType.Destiny,
            borderType = (ECardBorderType)UnityEngine.Random.Range(0, 6),
            monsterType = (EMonsterType)UnityEngine.Random.Range(0, (int)EMonsterType.MAX),
            expansionType = expansionType,
            isChampionCard = false,
            isNew = true
        };
    }

    public static Texture2D LoadTexture(string resourceName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        using (Stream stream = assembly.GetManifestResourceStream(resourceName))
        {
            if (stream == null)
            {
                Debug.LogError($"Embedded resource not found: {resourceName}");
                return null;
            }

            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(buffer);
            return texture;
        }
    }

    public static void SetTitleInteractable(bool interactable)
    {
        {
            TitleScreen titleScreen = FindFirstObjectByType<TitleScreen>();
            titleScreen.m_LoadGameButton.interactable = interactable;


            GameObject parentObject = GameObject.Find("NewGameBtn");

            if (parentObject != null)
            {
                // Look for the Button component in this object or its children
                UnityEngine.UI.Button myButton = parentObject.GetComponentInChildren<UnityEngine.UI.Button>();
                myButton.interactable = interactable;

            }
        }
    }


    public static void RunTitleInteractableSaveLogic()
    {
        //set buttons correctly
        TitleScreen titleScreen = GameObject.FindFirstObjectByType<TitleScreen>();
        GameObject parentObject = GameObject.Find("NewGameBtn");
        UnityEngine.UI.Button newGame = null;
        if (parentObject != null)
        {
            // Look for the Button component in this object or its children
            newGame = parentObject.GetComponentInChildren<UnityEngine.UI.Button>();

        }
        if (Plugin.SaveHandler.doesSaveExist())
        {
            titleScreen.m_LoadGameButton.interactable = true;
            newGame.interactable = false;
        }
        else
        {
            titleScreen.m_LoadGameButton.interactable = false;
            newGame.interactable = true;

        }
    }
}
