#region

#region

using AI.StateMachine;
using AI.TankBoss_States;
using Tank;
using UnityEngine;
using UnityEngine.Assertions;

#endregion

namespace AI.TankBoss
{

	#endregion

	public struct AIStateData
	{
		public GameObject AI;
		public GameObject player;
		public LayerMask playerLayerMask;
		public AIStats AIStats;
		public TankHealth AIHealth;

		public AIStateData(
			GameObject AI,
			GameObject player,
			LayerMask playerLayerMask,
			AIStats AIStats,
			TankHealth AIHealth)
		{
			this.AI = AI;
			this.player = player;
			this.playerLayerMask = playerLayerMask;
			this.AIStats = AIStats;
			this.AIHealth = AIHealth;
		}
	}

	public class AIController : MonoBehaviour
	{
		[Header("AI State Data")]
		[SerializeField]
		private Transform[] patrolPoints;
		[SerializeField]
		private LayerMask playerLayerMask;

		private Rigidbody AIRigidbody;

		private AIStateData AIStateData;

		private AIStats AIStats;

		private StateMachine.StateMachine stateMachine;

		private void Awake()
		{
			AIRigidbody = GetComponent<Rigidbody>();
			Assert.IsNotNull(AIRigidbody);

			stateMachine = new StateMachine.StateMachine();

			EntryState entryState = new EntryState(AIStateData);

			entryState.SetStateMachineFunctions(
				stateMachine.ActivateTrigger,
				stateMachine.GetTrigger,
				stateMachine.GetBool,
				stateMachine.SetBool);

			PatrolState patrolState = new PatrolState(AIStateData, patrolPoints);

			patrolState.SetStateMachineFunctions(
				stateMachine.ActivateTrigger,
				stateMachine.GetTrigger,
				stateMachine.GetBool,
				stateMachine.SetBool);

			ScanState scanState = new ScanState(AIStateData);

			scanState.SetStateMachineFunctions(
				stateMachine.ActivateTrigger,
				stateMachine.GetTrigger,
				stateMachine.GetBool,
				stateMachine.SetBool);

			PursueState pursueState = new PursueState(AIStateData);

			pursueState.SetStateMachineFunctions(
				stateMachine.ActivateTrigger,
				stateMachine.GetTrigger,
				stateMachine.GetBool,
				stateMachine.SetBool);

			AttackState attackState = new AttackState(AIStateData);

			attackState.SetStateMachineFunctions(
				stateMachine.ActivateTrigger,
				stateMachine.GetTrigger,
				stateMachine.GetBool,
				stateMachine.SetBool);

			StunnedState stunnedState = new StunnedState(AIStateData);

			stunnedState.SetStateMachineFunctions(
				stateMachine.ActivateTrigger,
				stateMachine.GetTrigger,
				stateMachine.GetBool,
				stateMachine.SetBool);

			FleeState fleeState = new FleeState(AIStateData);

			fleeState.SetStateMachineFunctions(
				stateMachine.ActivateTrigger,
				stateMachine.GetTrigger,
				stateMachine.GetBool,
				stateMachine.SetBool);

			HealState healState = new HealState(AIStateData);

			healState.SetStateMachineFunctions(
				stateMachine.ActivateTrigger,
				stateMachine.GetTrigger,
				stateMachine.GetBool,
				stateMachine.SetBool);

			DodgeState dodgeState = new DodgeState(AIStateData);

			dodgeState.SetStateMachineFunctions(
				stateMachine.ActivateTrigger,
				stateMachine.GetTrigger,
				stateMachine.GetBool,
				stateMachine.SetBool);

			stateMachine.AddEntryState(entryState);
			stateMachine.AddState(patrolState);
			stateMachine.AddState(scanState);
			stateMachine.AddState(pursueState);
			stateMachine.AddState(attackState);
			stateMachine.AddState(stunnedState);
			stateMachine.AddState(fleeState);
			stateMachine.AddState(healState);
			stateMachine.AddState(dodgeState);

			stateMachine.AddBool(TransitionKey.shouldPatrol, false);
			stateMachine.AddBool(TransitionKey.shouldScan, false);
			stateMachine.AddBool(TransitionKey.shouldPursue, false);
			stateMachine.AddBool(TransitionKey.shouldAttack, false);
			stateMachine.AddBool(TransitionKey.shouldBeStunned, false);
			stateMachine.AddBool(TransitionKey.shouldFlee, false);
			stateMachine.AddBool(TransitionKey.shouldHeal, false);
			stateMachine.AddBool(TransitionKey.shouldDodge, false);

			BoolCondition shouldPatrol =
				new BoolCondition(TransitionKey.shouldPatrol, true);

			stateMachine.AddAnyStateTransition(patrolState, shouldPatrol);

			BoolCondition shouldScan =
				new BoolCondition(TransitionKey.shouldScan, true);

			stateMachine.AddAnyStateTransition(scanState, shouldScan);

			BoolCondition shouldPursue =
				new BoolCondition(TransitionKey.shouldPursue, true);

			stateMachine.AddAnyStateTransition(pursueState, shouldPursue);

			BoolCondition shouldAttack =
				new BoolCondition(TransitionKey.shouldAttack, true);

			stateMachine.AddAnyStateTransition(attackState, shouldAttack);

			BoolCondition shouldBeStunned =
				new BoolCondition(TransitionKey.shouldBeStunned, true);

			stateMachine.AddAnyStateTransition(stunnedState, shouldBeStunned);

			BoolCondition shouldFlee =
				new BoolCondition(TransitionKey.shouldFlee, true);

			stateMachine.AddAnyStateTransition(fleeState, shouldFlee);

			BoolCondition shouldHeal =
				new BoolCondition(TransitionKey.shouldHeal, true);

			stateMachine.AddTransition(fleeState, healState, shouldHeal);

			BoolCondition shouldDodge =
				new BoolCondition(TransitionKey.shouldDodge, true);

			stateMachine.AddAnyStateTransition(dodgeState, shouldDodge);
		}

		/// <summary>
		///     Update the current stateMachine state
		/// </summary>
		private void Update() => stateMachine.Update();

		/// <summary>
		///     Unfreeze AI and start StateMachine
		/// </summary>
		private void OnEnable()
		{
			AIRigidbody.isKinematic = true;
			stateMachine.Start();
		}

		/// <summary>
		///     Freeze AI and reset StateMachine
		/// </summary>
		private void OnDisable()
		{
			AIRigidbody.isKinematic = true;
			stateMachine.Reset();
		}

		/// <summary>
		///     Set player gameobject and instantiate stats
		/// </summary>
		public void Setup(GameObject player)
		{
			AIStats = new AIStats();

			AIStateData = new AIStateData(
				gameObject,
				player,
				playerLayerMask,
				AIStats,
				GetComponent<TankHealth>());
		}

		/// <summary>
		///     Level up the AI
		/// </summary>
		public void LevelUpAI() => AIStats.LevelUp();
	}
}