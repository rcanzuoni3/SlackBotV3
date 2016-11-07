using System.Collections.Generic;

namespace SlackBotV3
{
	class CommandTypeRegistry
	{
		private int TypeID = 0;

		private Dictionary<string, int> CommandNameToID = new Dictionary<string, int>();

		private Dictionary<int, ICommandType> CommandTypes = new Dictionary<int, ICommandType>();

		public void RegisterCommandType(ICommandType commandType)
		{
			foreach (string name in commandType.CommandNames())
				CommandNameToID[name] = TypeID;

			CommandTypes[TypeID++] = commandType;
		}

		public int GetCommandId(string commandName)
		{
			return CommandNameToID[commandName];
		}

		public bool HasHandler(string commandName)
		{
			return CommandNameToID.ContainsKey(commandName);
		}

		public ICommandType GetCommandType(string commandName)
		{
			return CommandTypes[CommandNameToID[commandName]];
		}

		public IEnumerable<string> GetCommandNames()
		{
			return CommandNameToID.Keys;
		}
	}
}