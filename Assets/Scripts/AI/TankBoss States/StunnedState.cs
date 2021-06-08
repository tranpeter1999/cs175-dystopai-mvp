#region

using AI.TankBoss;
using UnityEngine;

#endregion

namespace AI.TankBoss_States
{
	public class StunnedState : AIState
	{
		private float stunnedTimer;

		public StunnedState(AIStateData AIStateData) : base(AIStateData)
		{
			//empty
		}

		protected override string DefaultName
		{
			get { return "StunnedState"; }
		}

		/// <summary>
		///     Stop navMeshAgent and reset stunnedTimer. Make sure kinematic is disabled
		///     to ensure missle knockback
		/// </summary>
		public override void OnEnter()
		{
			SetBool(TransitionKey.shouldBeStunned, false);

			AIRigidbody.isKinematic = false;
			navMeshAgent.isStopped = true;
			stunnedTimer = 0f;
		}

		/// <summary>
		///     Enable kinematic
		/// </summary>
		public override void OnExit() => AIRigidbody.isKinematic = true;

		/// <summary>
		///     Temporarily disable the AI for X duration then reset rigidbody physics
		///     so the NavMeshAgent can properly operate. If the AI's health is in the
		///     critical zone, then enter flee state. Else, enter pursue state
		/// </summary>
		public override void Update()
		{
			stunnedTimer += Time.deltaTime;

			if (stunnedTimer >= AIStateData.AIStats.StunDuration)
			{
				ResetRigidBodyPhysics();

				if (NeedsHealing())
				{
					SetBool(TransitionKey.shouldFlee, true);
				}
				else
				{
					SetBool(TransitionKey.shouldPursue, true);
				}
			}
		}
	}
}