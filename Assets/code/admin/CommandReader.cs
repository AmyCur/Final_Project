using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System;

namespace Admin;

public struct Command{
	public object parent;
	public List<object> parentVariables;
	public string command;
	public string[] splitCommand;
}

public static class CommandReader{
	
	public static void GetVariables(this Command cmd){
		PropertyInfo? info = cmd.parent.GetType().GetProperty(cmd.splitCommand[1]);
		info.SetValue(info, cmd.splitCommand[2]);
		

	}


	static string[] SplitCommand(Command command){
		return command.command.Split(" ");
	}

	// first word is set, 2nd word is variable name, 3rd word is value
	static void SetVariable(Command command){
		command.GetVariables();
	}

	static void DetermineAppropriateCommand(Command command){
		switch (command.splitCommand[0].ToLower()){
			case "set":
				SetVariable(command);
				break;
			case "print":
				break;
			default:
				Debug.Log($"Command {command.splitCommand[0]} not found!");
				break;
		}
	} 

	public static void ExecuteCommand(Command command){
		command.splitCommand = SplitCommand(command);
		if(command.splitCommand.Length>0) DetermineAppropriateCommand(command);
	}
}
