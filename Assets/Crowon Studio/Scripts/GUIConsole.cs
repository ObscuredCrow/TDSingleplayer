using System.Collections.Generic;
using UnityEngine;

internal struct LogEntry
{
    public string message;
    public LogType type;

    public LogEntry(string message, LogType type) {
        this.message = message;
        this.type = type;
    }
}

public class GUIConsole : MonoBehaviour
{
    public int height = 150;
    public int maxLogCount = 50;

    private Queue<LogEntry> log = new Queue<LogEntry>();

    public KeyCode hotKey = KeyCode.F12;

    private bool visible;
    private Vector2 scroll = Vector2.zero;

#if !UNITY_EDITOR
    void Awake()
    {
        Application.logMessageReceived += OnLog;
    }
#endif

    private void OnLog(string message, string stackTrace, LogType type) {
        bool isImportant = type == LogType.Error || type == LogType.Exception;

        if (isImportant && !string.IsNullOrWhiteSpace(stackTrace))
            message += "\n" + stackTrace;

        log.Enqueue(new LogEntry(message, type));

        if (log.Count > maxLogCount)
            log.Dequeue();

        if (isImportant)
            visible = true;

        scroll.y = float.MaxValue;
    }

    private void Update() {
        if (Input.GetKeyDown(hotKey))
            visible = !visible;
    }

    private void OnGUI() {
        if (!visible)
            return;

        scroll = GUILayout.BeginScrollView(scroll, "Box", GUILayout.Width(Screen.width), GUILayout.Height(height));
        foreach (LogEntry entry in log) {
            if (entry.type == LogType.Error || entry.type == LogType.Exception)
                GUI.color = Color.red;
            else if (entry.type == LogType.Warning)
                GUI.color = Color.yellow;
            GUILayout.Label(entry.message);
            GUI.color = Color.white;
        }
        GUILayout.EndScrollView();
    }
}