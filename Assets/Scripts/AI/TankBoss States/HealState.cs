#region

using AI.TankBoss;
using UnityEngine;

#endregion

namespace AI.TankBoss_States
{
	public class HealState : AIState
	{
		public HealState(AIStateData AIStateData) : base(AIStateData)
		{
			//empty
		}

		protected override string DefaultName
		{
			get { return "HealState"; }
		}

		/// <summary>
		///     Start navMeshAgent and reset rigidbody physics
		/// </summary>
		public override void OnEnter()
		{
			SetBool(TransitionKey.shouldHeal, false);

			ResetRigidBodyPhysics();
		}

		public override void OnExit()
		{
			// empty
		}

		/// <summary>
		///     If the AI is hit by missle, enter stunned state. If there are missles to be
		///     dodged, then enter dodge state. Else, heal.
		/// </summary>
		public override void Update()
		{
			if (!IsHit() && !ShouldDodge())
			{
				Heal();
			}
		}

		/// <summary>
		///     If health is fully recovered then pursue player. Else if player is in
		///     the heal radius, flee. Else, heal.
		/// </summary>
		private void Heal()
		{
			if (AIStateData.AIHealth.CurrentHealth < AIStateData.AIStats.Health)
			{
				if (IsPlayerInRadius(AIStats.HealRadius))
				{
					SetBool(TransitionKey.shouldFlee, true);
				}
				else
				{
					float healAmount = AIStateData.AIStats.HealthRegeneration * Time.deltaTime;
					AIStateData.AIHealth.Heal(healAmount);
				}
			}
			else
			{
				SetBool(TransitionKey.shouldPursue, true);
			}
		}
	}
}