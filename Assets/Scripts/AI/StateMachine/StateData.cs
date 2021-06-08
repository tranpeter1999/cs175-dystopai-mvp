#region

using System.Collections.Generic;

#endregion

namespace AI.StateMachine
{
	public class StateData
	{
		private readonly Dictionary<string, bool> booleans = new Dictionary<string, bool>();
		private readonly Dictionary<string, bool> triggers = new Dictionary<string, bool>();

		public void AddBoolean(string key, bool value) => booleans.Add(key, value);

		public void SetBoolean(string key, bool value) => booleans[key] = value;

		public bool GetBoolean(string key) => booleans[key];

		public void AddTrigger(string key) => triggers.Add(key, false);

		public void ActivateTrigger(string key) => triggers[key] = true;

		public void DeActivateTrigger(string key) => triggers[key] = false;

		public bool GetTrigger(string key) => triggers[key];
	}
}