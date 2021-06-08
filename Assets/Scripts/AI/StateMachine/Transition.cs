#region

using System;

#endregion

namespace AI.StateMachine
{
	public class Transition
	{
		protected Action<string> ConsumeTrigger;

		protected Func<string, bool> GetBool;
		protected Func<string, bool> GetTrigger;
		private readonly State targetState;
		private readonly TransitionCondition[] transitionConditions;

		/// <summary>
		///     Construct abstract transtion to define path
		/// </summary>
		/// <param name="fromState"></param>
		/// <param name="targetState"></param>
		public Transition(State targetState, params TransitionCondition[] transitionConditions)
		{
			if (targetState == null)
			{
				throw new ArgumentNullException(
					nameof(targetState),
					"Transition target state cannot be null.");
			}

			if (transitionConditions == null)
			{
				throw new ArgumentNullException(
					nameof(transitionConditions),
					"Transition conditions canot be null.");
			}

			for (int i = 0; i < transitionConditions.Length; ++i)
			{
				if (transitionConditions[i] == null)
				{
					throw new ArgumentNullException(
						nameof(transitionConditions),
						$"Transition conditions element {i} is null and should not be.");
				}
			}

			this.targetState = targetState;
			this.transitionConditions = transitionConditions;
		}

		/// <summary>
		///     Get name of the state this transition goes to
		/// </summary>
		public string TargetState
		{
			get { return targetState.Name; }
		}

		/// <summary>
		///     Add state machine data which is required for checking info
		/// </summary>
		/// <param name="stateData"></param>
		public void SetStateDataRetrievers(
			Func<string, bool> getBool,
			Func<string, bool> getTrigger,
			Action<string> consumeTrigger)
		{
			GetBool = getBool;
			GetTrigger = getTrigger;
			ConsumeTrigger = consumeTrigger;

			for (int i = 0; i < transitionConditions.Length; ++i)
			{
				transitionConditions[i].
					SetStateMachineFunctions(getBool, getTrigger, consumeTrigger);
			}
		}

		/// <summary>
		///     Test whether or not this transition should occur
		/// </summary>
		/// <returns>True if a transition should occur</returns>
		public bool ShouldTransition()
		{
			bool shouldTransfer = true;

			for (int i = 0; i < transitionConditions.Length; ++i)
			{
				shouldTransfer &= transitionConditions[i].ShouldTransition();

				if (shouldTransfer == false)
				{
					break;
				}
			}

			return shouldTransfer;
		}

		/// <summary>
		///     Right before a transition this function is called so the
		///     transition and clean anything it has done up. For example:
		///     deactivating any triggers it has used is required
		/// </summary>
		public void OnTransition()
		{
			for (int i = 0; i < transitionConditions.Length; ++i)
			{
				transitionConditions[i].OnTransition();
			}
		}
	}
}