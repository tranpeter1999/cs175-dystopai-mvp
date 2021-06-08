#region

using AI.StateMachine;
using AI.TankBoss;
using Tank;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

#endregion

namespace AI.TankBoss_States
{
	public abstract class AIState : State
	{
		protected Rigidbody AIRigidbody;
		protected TankShooting AIShooting;
		protected AIStateData AIStateData;
		protected NavMeshAgent navMeshAgent;
		protected TankShooting playerShooting;

		protected AIState(AIStateData AIStateData)
		{
			this.AIStateData = AIStateData;
			navMeshAgent = AIStateData.AI.GetComponent<NavMeshAgent>();
			AIRigidbody = AIStateData.AI.GetComponent<Rigidbody>();
			AIShooting = AIStateData.AI.GetComponent<TankShooting>();
			playerShooting = AIStateData.player.GetComponent<TankShooting>();

			Assert.IsNotNull(navMeshAgent);
			Assert.IsNotNull(AIShooting);
			Assert.IsNotNull(AIRigidbody);
			Assert.IsNotNull(playerShooting);

			navMeshAgent.speed = AIStateData.AIStats.Speed;
			navMeshAgent.angularSpeed = AIStateData.AIStats.TurnSpeed;
		}

		/// <summary>
		///     Determine whether the player is in sight of the AI
		/// </summary>
		protected bool IsPlayerInSight()
		{
			RaycastHit playerHit;

			return Physics.Raycast(
				AIStateData.AI.transform.position,
				AIStateData.AI.transform.TransformDirection(Vector3.forward),
				out playerHit,
				AIStateData.AIStats.SightRange,
				AIStateData.playerLayerMask);
		}

		/// <summary>
		///     Determine whether the AI is hit by a missle. If true, enter stunned
		///     state
		/// </summary>
		protected bool IsHit()
		{
			bool isHit = false;

			if (AIStateData.AIHealth.CurrentHealth < AIStateData.AIHealth.CachedHealth)
			{
				SetBool(TransitionKey.shouldBeStunned, true);

				AIStateData.AIHealth.CachedHealth = AIStateData.AIHealth.CurrentHealth;
				isHit = true;
			}

			return isHit;
		}

		/// <summary>
		///     Determine if there are nearby missles to dodge. If true, enter dodge
		///     state
		/// </summary>
		protected bool ShouldDodge()
		{
			bool missleInRadius = false;

			for (int i = 0; i < playerShooting.missleDataCache.Count; ++i)
			{
				float distanceToMissleDestination = Vector3.Distance(
					AIStateData.AI.transform.position,
					playerShooting.missleDataCache[i].destination);

				if (AIStateData.AIStats.MissleAvoidanceRadius >= distanceToMissleDestination)
				{
					SetBool(TransitionKey.shouldDodge, true);

					missleInRadius = true;

					break;
				}
			}

			return missleInRadius;
		}

		/// <summary>
		///     Set rigidbody velocity and angular velocity to zero. Used to ensure
		///     smooth NavMesh navigation and prevent undefined behavior when the
		///     Rigidbody and NavMesh components try to move the AI at the same
		/// </summary>
		protected void ResetRigidBodyPhysics()
		{
			AIRigidbody.velocity = Vector3.zero;
			AIRigidbody.angularVelocity = Vector3.zero;
		}

		/// <summary>
		///     Determines if the player is in a specified radius of the AI
		/// </summary>
		protected bool IsPlayerInRadius(float radius)
		{
			float distanceToPlayer = Vector3.Distance(
				AIStateData.player.transform.position,
				AIStateData.AI.transform.position);

			return distanceToPlayer <= radius;
		}

		/// <summary>
		///     If the AI is within the stopping distance to the destination, return
		///     true
		/// </summary>
		protected bool HasArrived(Vector3 destination)
		{
			float distanceToDestination = Vector3.Distance(
				destination,
				AIStateData.AI.transform.position);

			return distanceToDestination <= navMeshAgent.stoppingDistance;
		}

		/// <summary>
		///     Determines whether the AI's health is in the critical zone
		/// </summary>
		protected bool NeedsHealing() =>
			AIStateData.AIHealth.CurrentHealth < AIStateData.AIStats.CriticalHealth;
	}
}