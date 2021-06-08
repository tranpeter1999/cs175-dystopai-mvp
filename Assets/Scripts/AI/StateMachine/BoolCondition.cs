#region

using System;

#endregion

namespace AI.StateMachine
{
	public class BoolCondition : TransitionCondition
	{
		private readonly string key;
		private readonly bool val;

		public BoolCondition(string key, bool val)
		{
			if (key == null)
			{
				throw new ArgumentNullException(
					nameof(key),
					"bool transition cannot receive null key");
			}

			if (key.Equals(""))
			{
				throw new ArgumentException(
					"bool transition key cannot be an empty string.",
					nameof(key));
			}

			this.key = key;
			this.val = val;
		}

		public override void OnTransition()
		{
			// pass
		}

		public override bool ShouldTransition() => getBool(key) == val;

		protected override void StateMachineFunctionsSet()
		{
			// pass
		}
	}
}