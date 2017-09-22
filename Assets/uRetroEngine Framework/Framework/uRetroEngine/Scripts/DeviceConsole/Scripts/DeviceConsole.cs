using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

public class DeviceConsole : MonoBehaviour
{
    #region Inspector Variables

    [SerializeField]
    private GameObject uiContainer;

    [SerializeField]
    private GameObject logContainer;

    [SerializeField]
    private InputField commandInputField;

    [SerializeField]
    private bool autoFocusInputField = true;

    [SerializeField]
    private Color headerColour;

    [SerializeField]
    private string headerText;

    [SerializeField]
    private DeviceLogUI logPrefab;

    [SerializeField]
    private DeviceLogUI warningLogPrefab;

    [SerializeField]
    private DeviceLogUI errorLogPrefab;

    [SerializeField]
    private DeviceLogUI assertLogPrefab;

    [SerializeField]
    private DeviceLogUI exceptionLogPrefab;

    [SerializeField]
    private DeviceLogUI exceptionStackTracePrefab;

    #endregion Inspector Variables

    #region Member Variables

    private static int MaxCharCount = 15000;

    private List<DeviceLogUI> logs;
    private int curCharCount;
    private int commandHistoryIndex;

    // Variables only used on device for 3 finger tap
    private bool oneDown;

    private bool twoDown;

    #endregion Member Variables

    #region Unity Methods

    private void Awake()
    {
        logs = new List<DeviceLogUI>();

        commandHistoryIndex = 0;

        PrintHeader();
        PrintLogs();

        // Add callback so we can get when new logs are logged.
        DebugLogs.Instance.OnLogAdded += OnLogAdded;
        DebugLogs.Instance.OnLogsCleared += OnLogsCleared;

        // Add the default console commands
        DebugCommands.Instance.AddCommand("help", PrintHelp, "Prints list of commands");
        DebugCommands.Instance.AddCommand("clear", Clear, "Clears all text from the debug console");
        //DebugCommands.Instance.AddCommand("history", PrintHistory, "Prints list of all previous commands");
        //DebugCommands.Instance.AddCommand("logc", PrintLogCount, "Prints the number of logs currently held by DebugLogs");
        //DebugCommands.Instance.AddCommand("loadscene", LoadScene, "Loads the scene with the given name", "<scene_name>");
        DebugCommands.Instance.AddCommand("quit", CloseApp, "Closes the app");
        //DebugCommands.Instance.AddCommand("plogs", PLogs, "Clears the console and prints the logs in the specified range", "<start_index> <end_index>");
        //DebugCommands.Instance.AddCommand("setAF", SetAF, "Sets the boolean value of the Auto Focus Input Field property.", "< true | false >");
        DebugCommands.Instance.AddCommand("save", SaveCartridge, "Save current loaded game as cartidge");
        //DebugCommands.Instance.AddCommand("Reset", ResetGame, "Reset and reload current cartridge");
        //DebugCommands.Instance.AddCommand("Load", LoadGame, "uRE: load cartridge as folder ", "<cartridgeName>");
        //DebugCommands.Instance.AddCommand("Run", RunGame, "uRE: run game", "");
        //DebugCommands.Instance.AddCommand("LoadCart", LoadCartridge, "uRE: load cartridge", "<cartridgeName>");
        DebugCommands.Instance.AddCommand("new", NewCartridge, "Create new cartridge", "<cartridgeName>");
        DebugCommands.Instance.AddCommand("extract", ExtractCartridge, "Extract loaded cartridge to folder", "");
    }

    private void Update()
    {
        if (commandInputField != null)
        {
            // Handles the up/down arrow key presses which displays commands from the history
            if (Input.GetKeyDown(KeyCode.UpArrow) && commandHistoryIndex > 0)
            {
                commandHistoryIndex--;

                commandInputField.text = DebugCommands.Instance.CommandHistory[commandHistoryIndex];
                commandInputField.MoveTextEnd(false);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow) && commandHistoryIndex < DebugCommands.Instance.CommandHistory.Count)
            {
                commandHistoryIndex++;

                if (commandHistoryIndex == DebugCommands.Instance.CommandHistory.Count)
                {
                    commandInputField.text = "";
                }
                else
                {
                    commandInputField.text = DebugCommands.Instance.CommandHistory[commandHistoryIndex];
                    commandInputField.MoveTextEnd(false);
                }
            }
        }

        // Toggle visibility of the DeviceConsole when the '~' key is pressed in editor and 3 finger tap on device
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            SetVisible(!uiContainer.activeInHierarchy);
        }

        // First finger down
        if (!oneDown && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            oneDown = true;
        }

        if (oneDown && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            oneDown = false;
        }

        // Second finger down
        if (!twoDown && Input.touchCount > 1 && Input.GetTouch(1).phase == TouchPhase.Began)
        {
            twoDown = true;
        }

        if (twoDown && Input.touchCount > 1 && Input.GetTouch(1).phase == TouchPhase.Ended)
        {
            twoDown = false;
        }

        // Third finger down
        if (Input.touchCount == 3)
        {
            if (oneDown &&
                twoDown &&
                Input.GetTouch(2).phase == TouchPhase.Began)
            {
                SetVisible(!uiContainer.activeInHierarchy);
            }
        }
    }

    private void OnDestroy()
    {
        DebugLogs.Instance.OnLogAdded -= OnLogAdded;
        DebugLogs.Instance.OnLogsCleared -= OnLogsCleared;

        // Do not need to remove the other commands because they point to a static method.
        DebugCommands.Instance.RemoveCommand("plogs");
        DebugCommands.Instance.RemoveCommand("setAF");
    }

    #endregion Unity Methods

    #region Public Methods

    /// <summary>
    /// Sets this instances visibility
    /// </summary>
    public void SetVisible(bool visible)
    {
        // Set its visibility
        uiContainer.SetActive(visible);
        InputFieldObtainFocus();
        if (commandInputField != null)
        {
            // Clear any user typed text
            commandInputField.text = "";

            // If it became visible, focus the input field
            if (visible)
            {
                InputFieldObtainFocus();
            }
        }
    }

    /// <summary>
    /// Called when the input field is done editing (eg. user pressed enter)
    /// </summary>
    public void OnEndEdit()
    {
        // If the UI Container is not active then don't process the command.
        if (!uiContainer.activeSelf)
        {
            return;
        }

        string ciText = commandInputField.text;
        int newlineIndex = ciText.IndexOf('\n');

        if (newlineIndex >= 0)
        {
            ciText = ciText.Remove(newlineIndex, 1);
        }

        // If the text is empty just return
        if (string.IsNullOrEmpty(ciText))
        {
            return;
        }

        string[] strs = ciText.Split(' ');
        string command = strs[0];

        Debug.Log(string.Format("$ {0}", ciText));

        // Execute the command
        if (!DebugCommands.Instance.ExecuteCommand(strs))
        {
            Debug.LogWarningFormat("{0}: Command Not Found", command);
        }

        commandHistoryIndex = DebugCommands.Instance.CommandHistory.Count;

        commandInputField.text = "";

        InputFieldObtainFocus();
    }

    #endregion Public Methods

    #region Private Methods

    private void PrintToConsole(string text, DeviceLogUI prefab = null)
    {
        if (logContainer != null)
        {
            AddLogToContainer((prefab != null) ? prefab : logPrefab, text);
        }
    }

    private void PrintToConsole(DebugLogs.Log log, string prefix = "")
    {
        if (logContainer != null)
        {
            DeviceLogUI prefab = null;

            switch (log.type)
            {
                case LogType.Log:
                    prefab = logPrefab;
                    break;

                case LogType.Warning:
                    prefab = warningLogPrefab;
                    break;

                case LogType.Error:
                    prefab = errorLogPrefab;
                    break;

                case LogType.Assert:
                    prefab = assertLogPrefab;
                    break;

                case LogType.Exception:
                    prefab = exceptionLogPrefab;
                    break;
            }

            string logMessage = log.message;

            logMessage = prefix + logMessage;

            AddLogToContainer(prefab, logMessage);

            if (log.type == LogType.Exception)
            {
                PrintToConsole(log.stackTrace, exceptionStackTracePrefab);
            }

            return;
        }
    }

    private void PrintHeader()
    {
        PrintToConsole(string.Format("<color=#{0}>{1}</color>", ColourToHex(headerColour), headerText));
    }

    private void PrintLogs()
    {
        for (int i = 0; i < DebugLogs.Instance.Logs.Count; i++)
        {
            PrintToConsole(DebugLogs.Instance.Logs[i]);
        }
    }

    private void DestroyLogs()
    {
        if (logs != null)
        {
            for (int i = 0; i < logs.Count; i++)
            {
                Destroy(logs[i].gameObject);
            }
        }

        logs.Clear();

        curCharCount = 0;
    }

    private void AddLogToContainer(DeviceLogUI prefab, string text)
    {
        DeviceLogUI deviceLogUI = Instantiate<DeviceLogUI>(prefab);
        deviceLogUI.textUI.text = text;
        deviceLogUI.transform.SetParent(logContainer.transform);
        deviceLogUI.transform.localScale = Vector3.one;

        curCharCount += deviceLogUI.textUI.text.Length;

        while (curCharCount > MaxCharCount)
        {
            curCharCount -= logs[0].textUI.text.Length;
            Destroy(logs[0].gameObject);
            logs.RemoveAt(0);
        }

        logs.Add(deviceLogUI);
    }

    private void OnLogAdded(DebugLogs.Log log)
    {
        PrintToConsole(log);
    }

    private void OnLogsCleared()
    {
        DestroyLogs();
        PrintHeader();
    }

    private void InputFieldObtainFocus()
    {
        //if (autoFocusInputField && commandInputField != null)
        {
            EventSystem.current.SetSelectedGameObject(commandInputField.gameObject, new PointerEventData(EventSystem.current));
            commandInputField.OnPointerClick(new PointerEventData(EventSystem.current));
            commandInputField.OnPointerEnter(new PointerEventData(EventSystem.current));
            commandInputField.ActivateInputField();
            commandInputField.Select();
        }
    }

    private string ColourToHex(Color32 color)
    {
        string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
        return hex;
    }

    #region Command Methods

    private void PLogs(string[] args)
    {
        if (args.Length != 3)
        {
            Debug.Log("Need to specify a start index and end index.");
            return;
        }

        int start;
        int end;

        if (!int.TryParse(args[1], out start))
        {
            Debug.Log("Start index must in an integer.");
            return;
        }

        if (!int.TryParse(args[2], out end))
        {
            Debug.Log("End index must in an integer.");
            return;
        }

        if (start > end)
        {
            int temp = start;
            start = end;
            end = temp;
            return;
        }

        if (start < 0)
        {
            start = 0;
        }

        if (end >= DebugLogs.Instance.Logs.Count)
        {
            end = DebugLogs.Instance.Logs.Count - 1;
        }

        DestroyLogs();

        for (int i = start; i <= end; i++)
        {
            PrintToConsole(DebugLogs.Instance.Logs[i], i + ":");
        }
    }

    private void SetAF(string[] args)
    {
        if (args.Length != 2)
        {
            Debug.LogError("Please specify either t or f.");
            return;
        }

        if (args[1] == "t" || args[1] == "true")
        {
            autoFocusInputField = true;
        }
        else if (args[1] == "f" || args[1] == "false")
        {
            autoFocusInputField = false;
        }
        else
        {
            Debug.LogError("Please specify either t or f.");
        }
    }

    private static void PrintHelp(string[] args)
    {
        for (int i = 0; i < DebugCommands.Instance.Commands.Count; i++)
        {
            DebugCommands.Command command = DebugCommands.Instance.Commands[i];

            string helpStr = string.Format("{0}", "<color=white>" + command.name + "</color>");

            if (!string.IsNullOrEmpty(command.argsDescription))
            {
                helpStr += string.Format(" {0}", "<color=white>" + command.argsDescription + "</color>");
            }

            if (!string.IsNullOrEmpty(command.description))
            {
                helpStr += string.Format(" - {0}", "<color=orange>" + command.description + "</color>");
            }

            Debug.Log(helpStr);
        }
    }

    private static void Clear(string[] args)
    {
        DebugLogs.Instance.ClearLogs();
    }

    private static void PrintHistory(string[] args)
    {
        for (int i = 0; i < DebugCommands.Instance.CommandHistory.Count; i++)
        {
            Debug.Log(DebugCommands.Instance.CommandHistory[i]);
        }
    }

    private static void PrintLogCount(string[] args)
    {
        Debug.Log(string.Format("There are {0} reported logs.", DebugLogs.Instance.Logs.Count));
    }

    /*
    private static void LoadScene(string[] args)
    {
        if (args.Length != 2)
        {
            Debug.LogError("Please specify a scene name.");
            return;
        }

        Application.LoadLevel(args[1]);
    }
    */

    private static void CloseApp(string[] args)
    {
        Application.Quit();
    }

    private static void SaveCartridge(string[] args)
    {
        uRetroEngine.uRetroSystem.SaveCartridge();
    }

    private static void ResetGame(string[] args)
    {
        uRetroEngine.uRetroSystem.ResetGame();
    }

    private static void LoadGame(string[] args)
    {
        if (args.Length != 2)
        {
            Debug.LogError("Please specify a cartridge name.");
            return;
        }
        uRetroEngine.uRetroSystem.LoadGame(args[1]);
    }

    private static void RunGame(string[] args)
    {
        uRetroEngine.uRetroConsole.Hide();
        uRetroEngine.uRetroSystem.RunGame();
    }

    public static void LoadCartridge(string[] args)
    {
        if (args.Length != 2)
        {
            Debug.LogError("Please specify a cartridge name.");
            return;
        }
        uRetroEngine.uRetroSystem.LoadCartridge(args[1]);
    }

    public static void NewCartridge(string[] args)
    {
        if (args.Length != 2)
        {
            Debug.LogError("Please specify a cartridge name.");
            return;
        }
        uRetroEngine.uRetroSystem.CreateGame(args[1]);
    }

    public static void ExtractCartridge(string[] args)
    {
        uRetroEngine.uRetroSystem.ExtractCartridge();
    }

    #endregion Command Methods

    #endregion Private Methods
}