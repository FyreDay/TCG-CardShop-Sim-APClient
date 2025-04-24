using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;
using System;
using System.ComponentModel;
using Archipelago.MultiClient.Net.Converters;
using System.Text.RegularExpressions;
using System.Linq;
using ApClient;

public class APConsole : MonoBehaviour
{

    [Serializable]
    public class LogEntry
    {
        public double Timestamp;
        public float Lifetime = 50f;
        public float TotalLifetime = 50f;
        public float RiseSpeed = 5f;
        public TextMeshProUGUI RenderedText;
        public string Message;

        public LogEntry(string message, double timestamp)
        {
            Message = message;
            Timestamp = timestamp;
        }
    }

    static readonly Dictionary<string, string> keywordColors = new()
    {
        { "license", "#FF5151" },
        { "card", "#3089FF" },
        { "money", "#FFBF62" }
    };

    static readonly Regex keywordRegex = new Regex(
      string.Join("|", keywordColors.Keys.Select(Regex.Escape)),
      RegexOptions.IgnoreCase
    );

    private bool showConsole = true;

    private Transform messageParent;
    private readonly List<LogEntry> visibleEntries = new();
    private readonly Queue<LogEntry> cachedEntries = new();
    private Queue<TextMeshProUGUI> messagePool = new();

    private float lastUpdate;
    private const float MessageDelay = 0.05f;
    private const float MessageTime = 6f;

    private static float animationDuration = 6f;
    private static float MessageHeight = 10f;
    private static float ConsoleHeight = 280f;
    private static int MaxMessages = (int) (ConsoleHeight / (MessageHeight * 1.5));

    public static APConsole Instance { get; private set; }

    public static void Create()
    {
        if (Instance != null )
        {
            return;
        }
        var go = new GameObject("ArchipelagoConsoleUI");
        DontDestroyOnLoad(go);
        var ui = go.AddComponent<APConsole>();
        ui.CreateConsoleCanvas();
        Instance = ui;
    }

    private void Update()
    {
        if (Time.time - lastUpdate >= MessageTime/(ConsoleHeight / (MessageHeight * 2)))
        {
            UpdateVisibleMessages();
            lastUpdate = Time.time;
        }

        UpdateUI();

        if (Input.GetKeyDown(Settings.Instance.ConsoleHotkey.Value)) 
        {
            if (showConsole)
            {
                Log("Disabling AP Console");
                showConsole = false;
            }
            else
            {
                showConsole = true;
                Log("AP Console Enabled");
            }
        }
    }

    private void UpdateUI()
    {
        foreach (var entry in visibleEntries)
        {
            if (entry.RenderedText)
            {
                Animate(entry.RenderedText, entry.Timestamp);
            }

            entry.RenderedText.text = Colorize(entry.Message);
        }
    }

    private void Animate(TextMeshProUGUI text, double timestamp)
    {
        float elapsed = (float)(DateTime.Now.ToUnixTimeStamp() - timestamp);
        float t = Mathf.Clamp01(elapsed / animationDuration);
        float offset = t * ConsoleHeight;
        var pos = text.rectTransform.anchoredPosition;
        pos.y = offset;
        text.rectTransform.anchoredPosition = pos;
        //fade out in last 2 secs
        text.alpha = Mathf.Lerp(1f, 0f, Mathf.Clamp01(elapsed - animationDuration + 2f));
    }

    string Colorize(string input)
    {
        return keywordRegex.Replace(input, match =>
        {
            var keyword = match.Value.ToLower();
            if (keywordColors.TryGetValue(keyword, out string hex))
                return $"<color={hex}>{match.Value}</color>";
            return match.Value;
        });
    }

    private void UpdateVisibleMessages()
    {
        double now = DateTime.Now.ToUnixTimeStamp();
        
        visibleEntries.RemoveAll(e => { 
            
            if ((now - e.Timestamp) > MessageTime)
            {
                RecycleMessage(e);
                return true;
            }
            return false;
        });

        if (visibleEntries.Count >= MaxMessages || cachedEntries.Count == 0 )
            return;

        LogEntry newEntry = cachedEntries.Dequeue();
        newEntry.Timestamp = now;
        GameObject go = new GameObject("ConsoleMessage");
        go.transform.SetParent(messageParent, false);
        var textGui = GetPooledMessage();
        textGui.enableWordWrapping = false;
        textGui.text = newEntry.Message;
        textGui.fontSize = 20;
        textGui.alignment = TextAlignmentOptions.Left;
        textGui.color = new Color(1f, 1f, 1f, 1f);
        textGui.rectTransform.anchoredPosition = new Vector2(MessageHeight, 0f);
        newEntry.RenderedText = textGui;
        
        visibleEntries.Add(newEntry);
    }

    public void Log(string text)
    {
        if (showConsole)
        {
            LogEntry entry = new(text, DateTime.Now.ToUnixTimeStamp());

            cachedEntries.Enqueue(entry);
        }
    }

    private void CreateConsoleCanvas()
    {
        var canvasObject = new GameObject("ArchipelagoConsoleCanvas");
        canvasObject.transform.SetParent(transform);

        var canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1000;

        var scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        canvasObject.AddComponent<GraphicRaycaster>();

        GameObject messageContainer = new GameObject("Messages");
          var rect = messageContainer.AddComponent<RectTransform>();
        rect.SetParent(canvasObject.transform, false);
        rect.anchorMin = new Vector2(0f, 0f);   // Bottom left
        rect.anchorMax = new Vector2(0f, 0f);
        rect.pivot = new Vector2(0f, 0f);
        rect.anchoredPosition = new Vector2(60f, 35f); // Slight offset from corner
        messageParent = messageContainer.transform;
    }

    private TextMeshProUGUI GetPooledMessage()
    {
        if (messagePool.Count > 0)
        {
            var tmp = messagePool.Dequeue();
            tmp.gameObject.SetActive(true);
            return tmp;
        }

        GameObject go = new GameObject("ConsoleMessage");
        go.transform.SetParent(messageParent, false);
        var tmpNew = go.AddComponent<TextMeshProUGUI>();
        tmpNew.enableWordWrapping = false;
        return tmpNew;
    }

    private void RecycleMessage(LogEntry msg)
    {
        msg.RenderedText.gameObject.SetActive(false);
        messagePool.Enqueue(msg.RenderedText);
    }
}
