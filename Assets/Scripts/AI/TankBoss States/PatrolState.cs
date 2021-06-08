#region

using AI.TankBoss;
using UnityEngine;

#endregion

namespace AI.TankBoss_States
{
	public class PatrolState : AIState
	{
		private readonly Transform[] patrolPoints;
		private int currentPatrolPoint;

		public PatrolState(
			AIStateData AIStateData,
			Transform[] patrolPoints) : base(AIStateData)
		{
			this.patrolPoints = patrolPoints;
			currentPatrolPoint = 0;
		}

		protected override string DefaultName
		{
			get { return "PatrolState"; }
		}

		/// <summary>
		///     Resume NavMeshAgent and set current patrol point
		/// </summary>
		public override void OnEnter()
		{
			SetBool(TransitionKey.shouldPatrol, false);

			navMeshAgent.isStopped = false;
		}

		/// <summary>
		///     Stop NavMeshAgent
		/// </summary>
		public override void OnExit() => navMeshAgent.isStopped = true;

		/// <summary>
		///     If hit by missle, enter stunned state. Else, if the player is within the
		///     alert radius pursue the player. Otherwise, patrol the waypoints
		/// </summary>
		public override void Update()
		{
			if (!IsHit())
			{
				if (IsPlayerInRadius(AIStateData.AIStats.AlertRadius))
				{
					SetBool(TransitionKey.shouldPursue, true);
				}
				else
				{
					Patrol();
				}
			}
		}

		/// <summary>
		///     If the AI has arrived at the current waypoint, increment the currentPatrolPoint
		///     to set the next waypoint and enter scan state. Else, continue to the current
		///     waypoint
		/// </summary>
		private void Patrol()
		{
			if (HasArrived(patrolPoints[currentPatrolPoint].position))
			{
				currentPatrolPoint = (currentPatrolPoint + 1) % patrolPoints.Length;
				SetBool(TransitionKey.shouldScan, true);
			}
			else
			{
				navMeshAgent.destination = patrolPoints[currentPatrolPoint].position;
			}
		}
	}
}