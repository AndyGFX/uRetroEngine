using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DebugLogs : ScriptableObject
{
	#region Inner Classes

	[System.Serializable]
	public class Log
	{
		public LogType 	type;
		public string	message;
		public string	stackTrace;
	}

	#endregion

	#region Member Variables

	private static DebugLogs instance;

	// Called everytime a log is added
	public System.Action<Log>	OnLogAdded;

	// Called everytime all the logs are cleared
	public System.Action		OnLogsCleared;

	private List<Log> logs = new List<Log>();

	#endregion

	#region Properties

	public static DebugLogs Instance
	{
		get
		{
			if (instance == null)
			{
				instance = ScriptableObject.CreateInstance<DebugLogs>();
			}

			return instance;
		}
	}

	public List<Log> Logs { get { return logs; } }

	#endregion

	#region Unity Methods

	private void OnEnable()
	{
		Application.logMessageReceived += LogCallback;
	}

	private void OnDestroy()
	{
		Application.logMessageReceived -= LogCallback;
	}

	#endregion

	#region Public Methods

	/// <summary>
	/// Creates an instance of DebugLogs if one has not already been created.
	/// </summary>
	public static DebugLogs Touch()
	{
		return DebugLogs.Instance;
	}

	/// <summary>
	/// Adds a new log.
	/// </summary>
	public void AddLog(Log log)
	{
		logs.Add(log);

		if (OnLogAdded != null)
		{
			OnLogAdded(log);
		}
	}

	/// <summary>
	/// Clears all logs.
	/// </summary>
	public void ClearLogs()
	{
		logs.Clear();

		if (OnLogsCleared != null)
		{
			OnLogsCleared();
		}
	}

	#endregion

	#region Private Methods

	private void LogCallback(string condition, string stackTrace, LogType logType)
	{
		if (!string.IsNullOrEmpty(stackTrace))
		{
			stackTrace = stackTrace.Remove(stackTrace.Length - 1, 1);
		}

		Log log = new Log();

		log.type		= logType;
		log.message		= condition;
		log.stackTrace	= stackTrace;

		AddLog(log);
	}

	#endregion
}
