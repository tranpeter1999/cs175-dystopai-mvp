#region

using System;
using UnityEngine;

#endregion

namespace AI.StateMachine
{
	public abstract class State
	{
		// these functions are called in implementations to stop the user from 
		// accidentally modifying them
		private Action<string> activateTrigger;
		private Func<string, bool> getBool;
		private Func<string, bool> getTrigger;
		private Action<string, bool> setBool;
		private bool verbose = false;

		/// <summary>
		///     Create a state with the default name
		/// </summary>
		public State()
		{
			Name = DefaultName;
		}

		/// <summary>
		///     Create a state with a custom name rather than the default
		/// </summary>
		/// <param name="name"></param>
		public State(string name)
		{
			Name = name;
		}

		protected abstract string DefaultName { get; }

		/// <summary>
		///     Name of the state. This will either be user defined or the default state
		///     name depending on the constructor used
		/// </summary>
		public string Name { get; }

		/// <summary>
		///     Called when the state is entered
		/// </summary>
		public abstract void OnEnter();

		/// <summary>
		///     Called when the state is exited before the next state is entered
		/// </summary>
		public abstract void OnExit();

		/// <summary>
		///     This can be called every frame or whenever for complex states that
		///     have behavior on a frame by X basis.
		/// </summary>
		public virtual void Update() => Debug.LogError("Update not implemented.");

		/// <summary>
		///     Receive state machine related functions that give states required behaviour
		/// </summary>
		/// <param name="activateTrigger"></param>
		/// <param name="getTrigger"></param>
		/// <param name="getBool"></param>
		/// <param name="setBool"></param>
		public void SetStateMachineFunctions(
			Action<string> activateTrigger,
			Func<string, bool> getTrigger,
			Func<string, bool> getBool,
			Action<string, bool> setBool)
		{
			if (activateTrigger == null)
			{
				throw new ArgumentNullException(
					nameof(activateTrigger),
					"activateTrigger function cannot be null.");
			}

			if (getTrigger == null)
			{
				throw new ArgumentNullException(
					nameof(getTrigger),
					"getTrigger function cannot be null.");
			}

			if (getBool == null)
			{
				throw new ArgumentNullException(
					nameof(getBool),
					"getBool function cannot be null.");
			}

			if (setBool == null)
			{
				throw new ArgumentNullException(
					nameof(setBool),
					"setBool function cannot be null.");
			}

			this.activateTrigger = activateTrigger;
			this.getTrigger = getTrigger;
			this.getBool = getBool;
			this.setBool = setBool;
		}

		/// <summary>
		///     Set whether the state machine is verbose or not
		/// </summary>
		/// <param name="isVerbose"></param>
		public void SetVerbose(bool isVerbose) => verbose = isVerbose;

		/// <summary>
		///     Activate a trigger in the state machine this state is a part of
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		protected void ActivateTrigger(string key) => activateTrigger(key);

		/// <summary>
		///     Set a bool in the state machine this state is a part of
		/// </summary>
		/// <param name="key"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		protected void SetBool(string key, bool val) => setBool(key, val);

		/// <summary>
		///     Get a bool from the state machine this state is a part of
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		protected bool GetBool(string key) => getBool(key);

		/// <summary>
		///     Get a trigger from the state machine this state is a part of
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		protected bool GetTrigger(string key) => getTrigger(key);

		/// <summary>
		///     Call when a function a state has been entered
		/// </summary>
		protected void LogOnEnter()
		{
			if (verbose)
			{
				Debug.Log($"{Name} entered.");
			}
		}

		/// <summary>
		///     Call when a statre has ben exit
		/// </summary>
		protected void LogOnExit()
		{
			if (verbose)
			{
				Debug.Log($"{Name} left.");
			}
		}
	}
}