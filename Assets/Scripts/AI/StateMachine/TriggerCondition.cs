#region

using System;

#endregion

namespace AI.StateMachine
{
	public class TriggerCondition : TransitionCondition
	{
		private readonly string key;

		public TriggerCondition(string key)
		{
			if (key == null)
			{
				throw new ArgumentNullException(
					nameof(key),
					"TriggerCondition cannot have a null key.");
			}

			if (key.Equals(""))
			{
				throw new ArgumentException(
					"Trigger condition cannot have a key that is an empty string.",
					nameof(key));
			}

			this.key = key;
		}

		public override void OnTransition() => consumeTrigger(key);

		public override bool ShouldTransition() => getTrigger(key);

		protected override void StateMachineFunctionsSet()
		{
			// pass
		}
	}
}