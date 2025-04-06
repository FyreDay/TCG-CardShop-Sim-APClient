using ApClient;
using ApClient.patches;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ApClientl;

public class APGui : MonoBehaviour
{
    public static bool showGUI = true;
    public static string ipporttext = "localhost:38281";
    public static string password = "";
    public static string slot = "Player1";

    public static string state = "Not Connected";
    void OnGUI()
    {
        if (!showGUI) return;

        // Create a GUI window
        GUI.Box(new Rect(10, 10, 200, 300), "AP Client");

        // Set font size and color
        GUIStyle textStyle = new GUIStyle();
        textStyle.fontSize = 12;
        textStyle.normal.textColor = UnityEngine.Color.white;

        // Display text at position (10,10)
        GUI.Label(new Rect(20, 40, 300, 30), "Address:port", textStyle);
        ipporttext = GUI.TextField(new Rect(20, 60, 180, 25), ipporttext, 25);

        GUI.Label(new Rect(20, 90, 300, 30), "Password", textStyle);
        password = GUI.TextField(new Rect(20, 110, 180, 25), password, 25);

        GUI.Label(new Rect(20, 140, 300, 30), "Slot", textStyle);
        slot = GUI.TextField(new Rect(20, 160, 180, 25), slot, 25);

        if (GUI.Button(new Rect(20, 210, 180, 30), "Connect"))
        {
            Debug.Log("Button Pressed!");
            Plugin.connect(ipporttext, password, slot);
        }


        GUI.Label(new Rect(20, 240, 300, 30), state, textStyle);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F9)) // Press F1 to log scenes
        {
            showGUI = !showGUI;
        }
        if (Input.GetKeyDown(KeyCode.H)) // Press F1 to log scenes
        {
            PopupTextPatches.ShowCustomText("Warehouse Key Found");
        }


    }
}
