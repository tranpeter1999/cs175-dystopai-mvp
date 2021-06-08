#region

using AI.TankBoss;
using UnityEngine;

#endregion

namespace AI.TankBoss_States
{
	public class PursueState : AIState
	{
		public PursueState(AIStateData AIStateData) : base(AIStateData)
		{
			//empty
		}

		protected override string DefaultName
		{
			get { return "PursueState"; }
		}

		/// <summary>
		///     Resume navMeshAgent and reset rigidbody physics
		/// </summary>
		public override void OnEnter()
		{
			SetBool(TransitionKey.shouldPursue, false);

			ResetRigidBodyPhysics();
			navMeshAgent.isStopped = false;
		}

		/// <summary>
		///     Stop navMeshAgent
		/// </summary>
		public override void OnExit() => navMeshAgent.isStopped = true;

		/// <summary>
		///     If the AI is hit by missle, enter stunned state. If there are missles to be
		///     dodged, then enter dodge state. Else, pursue the player
		/// </summary>
		public override void Update()
		{
			if (!IsHit() && !ShouldDodge())
			{
				Pursue();
			}
		}

		/// <summary>
		///     If the player is within attack range and is in sight, attack the player.
		///     Else, if close to the player, face the player. Otherwise, continue
		///     pursuing the player.
		/// </summary>
		private void Pursue()
		{
			if (IsPlayerInRadius(AIStateData.AIStats.AttackRange) && IsPlayerInSight())
			{
				SetBool(TransitionKey.shouldAttack, true);
			}
			else
			{
				if (HasArrived(AIStateData.player.transform.position))
				{
					FacePlayer();
				}
				else
				{
					navMeshAgent.destination = AIStateData.player.transform.position;
				}
			}
		}

		/// <summary>
		///     Rotate to face the player
		/// </summary>
		private void FacePlayer()
		{
			Quaternion rotation = Quaternion.LookRotation(
				AIStateData.player.transform.position - AIStateData.AI.transform.position);

			float turn = AIStateData.AIStats.TurnSpeed * Time.deltaTime;

			AIStateData.AI.transform.rotation = Quaternion.Slerp(
				AIStateData.AI.transform.rotation,
				rotation,
				turn);
		}
	}
}