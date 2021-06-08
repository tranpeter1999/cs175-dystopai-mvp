#region

using AI.TankBoss;
using UnityEngine;

#endregion

namespace AI.TankBoss_States
{
	public class DodgeState : AIState
	{
		public DodgeState(AIStateData AIStateData) : base(AIStateData)
		{
			// empty
		}

		protected override string DefaultName
		{
			get { return "DodgeState"; }
		}

		/// <summary>
		///     Resume NavMeshAgent
		/// </summary>
		public override void OnEnter()
		{
			SetBool(TransitionKey.shouldDodge, false);

			navMeshAgent.isStopped = false;
		}

		/// <summary>
		///     Stop NavMeshAgent
		/// </summary>
		public override void OnExit() => navMeshAgent.isStopped = true;

		/// <summary>
		///     If the AI is hit by missle, enter stunned state. If there are missles to be
		///     dodged, then dodge. Else, enter pursue state
		/// </summary>
		public override void Update()
		{
			if (!IsHit() && !Dodge())
			{
				SetBool(TransitionKey.shouldPursue, true);
			}
		}

		/// <summary>
		///     If there are missles to dodge, then dodge.
		///     <summary>
		private bool Dodge()
		{
			bool dodged = false;

			Vector3 dodgeDirection = CalculateFlockingSeparation();

			if (dodgeDirection != Vector3.zero)
			{
				Vector3 dodgeDestination =
					AIStateData.AI.transform.position +
					(dodgeDirection * AIStateData.AIStats.DodgeDistance);

				navMeshAgent.SetDestination(dodgeDestination);

				dodged = true;
			}

			return dodged;
		}

		/// <summary>
		///     Calculates the direction the AI should move based on the nearby projectiles
		///     using Boids Separation. For every missle in the MissleAvoidanceRadius,
		///     add the distance between AI and said missle to the AI direction vector.
		///     If there are no said missles, then return zero vector. Else, negate the
		///     resulting direction vector to steer away from the missles then normalize
		/// </summary>
		private Vector3 CalculateFlockingSeparation()
		{
			Vector3 direction = Vector3.zero;
			int numOfProjectiles = 0;

			for (int i = 0; i < playerShooting.missleDataCache.Count; ++i)
			{
				Vector3 missleDestination = playerShooting.missleDataCache[i].destination;

				float distanceToMissleDestination = Vector3.Distance(
					AIStateData.AI.transform.position,
					missleDestination);

				if (AIStateData.AIStats.MissleAvoidanceRadius >= distanceToMissleDestination)
				{
					Vector3 displacement = missleDestination - AIStateData.AI.transform.position;
					direction += displacement;
					numOfProjectiles++;
				}
			}

			if (numOfProjectiles > 0)
			{
				direction /= numOfProjectiles;
				direction.y = 0;
				direction *= -1;
				direction.Normalize();
			}

			return direction;
		}
	}
}