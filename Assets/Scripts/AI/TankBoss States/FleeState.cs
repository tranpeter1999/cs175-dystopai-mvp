#region

using System.Collections.Generic;
using AI.TankBoss;
using UnityEngine;
using UnityEngine.AI;

#endregion

namespace AI.TankBoss_States
{
	public class FleeState : AIState
	{
		private const float fleeRadius = 40f;
		private const float fleeSearchRange = 14f;
		private const float playerAvoidanceRadius = 5f;
		private const float coverSearchRadius = 5f;
		private const float sampleCoverPositionRange = 4f;
		private const int numOfSampleCoverPositions = 8;

		public FleeState(AIStateData AIStateData) : base(AIStateData)
		{
			// empty
		}

		protected override string DefaultName
		{
			get { return "FleeState"; }
		}

		/// <summary>
		///     Resume navMeshAgent and get flee position and set to destination if possible
		/// </summary>
		public override void OnEnter()
		{
			SetBool(TransitionKey.shouldFlee, false);

			navMeshAgent.isStopped = false;
			Vector3 fleePosition = Vector3.zero;

			if (GetFleePosition(ref fleePosition))
			{
				navMeshAgent.SetDestination(fleePosition);
			}
		}

		/// <summary>
		///     Stop navMeshAgent
		/// </summary>
		public override void OnExit() => navMeshAgent.isStopped = true;

		/// <summary>
		///     If the AI is hit by a missle, enter stunned state. Else, flee.
		/// </summary>
		public override void Update()
		{
			if (!IsHit())
			{
				Flee();
			}
		}

		/// <summary>
		///     If the AI is at the destination and the player is outside the HealRadius,
		///     then enter heal state.
		///     If the AI is at the destination and the player is inside the HealRadius,
		///     then set new flee destination.
		///     If the AI is not at the destination and the AI cannot flee, then
		///     enter pursue state.
		/// </summary>
		private void Flee()
		{
			Vector3 fleePosition = Vector3.zero;
			bool canFlee = GetFleePosition(ref fleePosition);

			if (HasArrived(navMeshAgent.destination))
			{
				if (!IsPlayerInRadius(AIStats.HealRadius))
				{
					SetBool(TransitionKey.shouldHeal, true);
				}
				else if (canFlee)
				{
					navMeshAgent.SetDestination(fleePosition);
				}
			}
			else if (!canFlee)
			{
				SetBool(TransitionKey.shouldPursue, true);
			}
		}

		/// <summary>
		///     Given the flee position, find all colliders within coverSearchRadius. Parse
		///     through the colliders and determine viable cover positions. Choose the
		///     cover position that is furthest from the player and set it as the new
		///     flee position. If no cover positions are found, verify that the path to
		///     the referenced fleePosition has no players near it.
		/// </summary>
		private bool GetFleePosition(ref Vector3 fleePosition)
		{
			bool fleePositionFound = false;

			Vector3 directionToPlayer =
				AIStateData.AI.transform.position - AIStateData.player.transform.position;

			fleePosition = AIStateData.AI.transform.position + directionToPlayer;

			NavMeshHit navMeshHit;

			fleePositionFound = NavMesh.SamplePosition(
				fleePosition,
				out navMeshHit,
				fleeRadius,
				NavMesh.AllAreas);

			if (fleePositionFound)
			{
				fleePosition = navMeshHit.position;
				bool coverPositionFound = GetCoverPosition(ref fleePosition);

				if (!coverPositionFound)
				{
					fleePositionFound = IsPlayerNearPathToDestination(fleePosition);
				}
			}

			return fleePositionFound;
		}

		/// <summary>
		///     Given the flee position, find all colliders within coverSearchRadius. Parse
		///     through the colliders and determine viable cover positions. Choose the
		///     cover position that is furthest from the player and set it as the new
		///     flee position. If no cover positions are found, fleePosition remains
		///     unchanged
		/// </summary>
		private bool GetCoverPosition(ref Vector3 fleePosition)
		{
			bool coverPositionFound = false;

			Collider[] hitColliders = Physics.OverlapSphere(
				fleePosition,
				coverSearchRadius);

			List<Vector3> possibleCoverPositions = PossibleCoverPositions(hitColliders);

			if (possibleCoverPositions.Count > 0)
			{
				Vector3 displacement =
					AIStateData.player.transform.position - AIStateData.AI.transform.position;

				possibleCoverPositions.Sort(SortByDistanceToPlayer);

				fleePosition = possibleCoverPositions[possibleCoverPositions.Count - 1];
				coverPositionFound = true;
			}

			return coverPositionFound;
		}

		/// <summary>
		///     Given each collider, get sample positions around the collider and
		///     determine the nearest edge from each sample position. If an edge is found,
		///     then add it to the list if it is a viable cover position and if the path
		///     to that edge does not have a player near it
		/// </summary>
		private List<Vector3> PossibleCoverPositions(Collider[] hitColliders)
		{
			List<Vector3> possibleCoverPositions = new List<Vector3>();

			Vector3 displacement =
				AIStateData.player.transform.position - AIStateData.AI.transform.position;

			for (int i = 0; i < hitColliders.Length; ++i)
			{
				Vector3 currentColliderPosition = hitColliders[i].transform.position;
				NavMeshHit hit;

				for (int j = 1; j <= numOfSampleCoverPositions; ++j)
				{
					Vector3 samplePosition = GetSampleCoverPosition(currentColliderPosition, j);

					bool nearestEdgeFound =
						NavMesh.FindClosestEdge(samplePosition, out hit, NavMesh.AllAreas);

					if (nearestEdgeFound &&
						IsViableCover(hit.normal, displacement) &&
						!IsPlayerNearPathToDestination(hit.position))
					{
						if (!possibleCoverPositions.Contains(hit.position))
						{
							possibleCoverPositions.Add(hit.position);
						}
					}
				}
			}

			return possibleCoverPositions;
		}

		/// <summary>
		///     Given the current sampleCoverPositionNum, calculate an angle and return
		///     the point where the line (starting from center and extending to a length
		///     of sampleCoverPositionRange) should hit the circle
		///     ///
		///     <summary>
		private Vector3 GetSampleCoverPosition(Vector3 center, int sampleCoverPositionNum)
		{
			float angle = 360 / sampleCoverPositionNum;
			Vector3 sampleCoverPosition;

			sampleCoverPosition.x =
				center.x + (sampleCoverPositionRange * Mathf.Sin(angle * Mathf.Deg2Rad));

			sampleCoverPosition.y = center.y;

			sampleCoverPosition.z =
				center.z + (sampleCoverPositionRange * Mathf.Cos(angle * Mathf.Deg2Rad));

			return sampleCoverPosition;
		}

		/// <summary>
		///     Get the waypoints from the AI's current position to the destination.
		///     From the line extended from one waypoint to the next, calculate the nearest
		///     point on said line to the player. If the playerAvoidanceRadius is greater
		///     than the distance to that nearest point, then we can assume that the AI
		///     will be near the player on the path from that waypoint to the next.
		/// </summary>
		private bool IsPlayerNearPathToDestination(Vector3 destination)
		{
			NavMeshPath path = new NavMeshPath();
			navMeshAgent.CalculatePath(destination, path);

			for (int i = 0; i < (path.corners.Length - 1); ++i)
			{
				Vector3 lineStart = path.corners[i];
				Vector3 lineEnd = path.corners[i + 1];

				Vector3 nearestPointFromPlayerToPathLine = NearestPointOnLine(
					AIStateData.player.transform.position,
					lineStart,
					lineEnd);

				float distanceToNearestPointFromPlayerToPathLine = Vector3.Distance(
					nearestPointFromPlayerToPathLine,
					AIStateData.player.transform.position);

				if (playerAvoidanceRadius >= distanceToNearestPointFromPlayerToPathLine)
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		///     Perform a projection from a vector A (lineStart to point) onto a vector B
		///     (lineStart to lineEnd). Because the line is finite, clamp the dot product
		/// </summary>
		private static Vector3 NearestPointOnLine(
			Vector3 point,
			Vector3 lineStart,
			Vector3 lineEnd)
		{
			Vector3 line = lineEnd - lineStart;
			float lineLength = line.magnitude;
			line.Normalize();

			Vector3 projection = point - lineStart;
			float dot = Vector3.Dot(line, projection);
			dot = Mathf.Clamp(dot, 0f, lineLength);

			return lineStart + (line * dot);
		}

		/// <summary>
		///     Determines whether the vectors are pointing in somewhat opposite directions.
		///     If the dot product returns -1, then they are pointing in completely
		///     opposite directions.
		/// </summary>
		private bool IsViableCover(Vector3 normal, Vector3 displacement) =>
			Vector3.Dot(normal, displacement) <= AIStateData.AIStats.CoverQuality;

		/// <summary>
		///     Sorts a position based off its distance to the player
		/// </summary>
		private int SortByDistanceToPlayer(Vector3 a, Vector3 b)
		{
			float distanceToA = Vector3.Distance(
				a,
				AIStateData.player.transform.position);

			float distanceToB = Vector3.Distance(
				b,
				AIStateData.player.transform.position);

			return distanceToA.CompareTo(distanceToB);
		}
	}
}