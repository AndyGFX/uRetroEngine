using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DebugCommands : ScriptableObject
{
	#region Inner Classes

	[System.Serializable]
	public class Command
	{
		public string		name;
		public DebugCommand	method;
		public string		description;
		public string		argsDescription;
	}

	#endregion

	#region Delegates

	public delegate void DebugCommand(string[] args);

	#endregion

	#region Member Variables

	private static DebugCommands instance = null;

	private List<Command>	commands 		= new List<Command>();
	private List<string>	commandHistory	= new List<string>();

	#endregion

	#region Properties

	public static DebugCommands Instance
	{
		get
		{
			if (instance == null)
			{
				instance = ScriptableObject.CreateInstance<DebugCommands>();
			}

			return instance;
		}
	}

	public List<Command>	Commands		{ get { return commands; } }
	public List<string>		CommandHistory	{ get { return commandHistory; } }

	#endregion

	#region Public Methods

	/// <summary>
	/// Returns true if the given command name has already been added toe this instance, false otherwise.
	/// </summary>
	public bool HasCommand(string cmdName)
	{
		cmdName = cmdName.ToLower();

		for (int i = 0; i < commands.Count; i++)
		{
			if (cmdName == commands[i].name)
			{
				return true;
			}
		}

		return false;
	}

	/// <summary>
	/// Executes a command, the first string in the array is the command to run and all other strings are the arguements. Returns true if the command was successfully, false otherwise.
	/// </summary>
	public bool ExecuteCommand(string[] args)
	{
		if (args == null || args.Length == 0)
		{
			return false;
		}

		args[0] = args[0].ToLower();

		string cmdName = args[0];

		commandHistory.Add(cmdName);

		for (int i = 0; i < commands.Count; i++)
		{
			if (cmdName == commands[i].name)
			{
				commands[i].method(args);

				return true;
			}
		}

		return false;
	}

	/// <summary>
	/// Adds a new command that can be executed.
	/// </summary>
	public void AddCommand(string cmdName, DebugCommand method, string desc = "", string argDesc = "")
	{
		cmdName = cmdName.ToLower();

		if (HasCommand(cmdName))
		{
			return;
		}

		Command cmd = new Command();

		cmd.name		= cmdName;
		cmd.method		= method;
		cmd.description = desc;
		cmd.argsDescription		= argDesc;

		commands.Add(cmd);
	}

	public void RemoveCommand(string cmdName)
	{
		cmdName = cmdName.ToLower();

		for (int i = 0; i < commands.Count; i++)
		{
			if (cmdName == commands[i].name)
			{
				commands.RemoveAt(i);
				break;
			}
		}
	}

	#endregion
}
