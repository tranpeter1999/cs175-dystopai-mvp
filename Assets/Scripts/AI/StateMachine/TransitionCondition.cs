#region

using System;

#endregion

namespace AI.StateMachine
{
	public abstract class TransitionCondition
	{
		protected Action<string> consumeTrigger;
		protected Func<string, bool> getBool;
		protected Func<string, bool> getTrigger;

		// @note: null checks are not done because the state machine has given these to the transition
		public void SetStateMachineFunctions(
			Func<string, bool> getBool,
			Func<string, bool> getTrigger,
			Action<string> consumeTrigger)
		{
			this.getBool = getBool;
			this.getTrigger = getTrigger;
			this.consumeTrigger = consumeTrigger;
			StateMachineFunctionsSet();
		}

		/// <summary>
		///     Called before the transition for anything useful to be done such as
		///     consumeing triggers
		/// </summary>
		public abstract void OnTransition();

		public abstract bool ShouldTransition();

		protected abstract void StateMachineFunctionsSet();
	}
}