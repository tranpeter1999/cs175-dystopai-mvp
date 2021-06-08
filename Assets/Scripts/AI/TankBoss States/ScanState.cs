#region

using AI.TankBoss;
using UnityEngine;

#endregion

namespace AI.TankBoss_States
{
	public class ScanState : AIState
	{
		private const int rangeDivider = 2;

		private float degreesRotated;
		private float degreeToRotate;
		private bool hasTurnLeft;
		private bool hasTurnRight;

		public ScanState(AIStateData AIStateData) : base(AIStateData)
		{
			// empty
		}

		protected override string DefaultName
		{
			get { return "ScanState"; }
		}

		/// <summary>
		///     Set scan settings and reset rigidbody physics
		/// </summary>
		public override void OnEnter()
		{
			SetBool(TransitionKey.shouldScan, false);

			ResetRigidBodyPhysics();
			degreesRotated = 0f;
			hasTurnLeft = false;
			hasTurnRight = false;
		}

		public override void OnExit()
		{
			// empty
		}

		/// <summary>
		///     If hit by missle, enter stunned state. Else, scan for the player
		/// </summary>
		public override void Update()
		{
			if (!IsHit())
			{
				Scan();
			}
		}

		/// <summary>
		///     If the player is in sight, pursue the player. Else, turn to X degrees
		///     then to -X degrees, then continue patrolling.
		/// </summary>
		private void Scan()
		{
			if (IsPlayerInSight())
			{
				SetBool(TransitionKey.shouldPursue, true);
			}
			else
			{
				if (!hasTurnLeft)
				{
					degreeToRotate = AIStateData.AIStats.ScanDegrees / rangeDivider;
					hasTurnLeft = Turn(degreeToRotate);
				}
				else if (!hasTurnRight)
				{
					degreeToRotate = -(AIStateData.AIStats.ScanDegrees / rangeDivider);
					hasTurnRight = Turn(degreeToRotate);
				}
				else
				{
					SetBool(TransitionKey.shouldPatrol, true);
				}
			}
		}

		/// <summary>
		///     Turn left or right to X degree
		/// </summary>
		private bool Turn(float degreeToRotate)
		{
			bool hasTurned = false;

			float turn = Mathf.Sign(degreeToRotate) *
				AIStateData.AIStats.ScanSpeed *
				Time.deltaTime;

			if (degreeToRotate >= 0)
			{
				hasTurned = degreesRotated >= degreeToRotate;
			}
			else
			{
				hasTurned = degreesRotated <= degreeToRotate;
			}

			if (!hasTurned)
			{
				AIStateData.AI.transform.Rotate(0f, turn, 0f);
				degreesRotated += turn;
			}

			return hasTurned;
		}
	}
}