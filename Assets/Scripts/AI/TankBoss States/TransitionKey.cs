namespace AI.TankBoss_States
{
	public readonly struct TransitionKey
	{
		public const string shouldPatrol = "shouldPatrol";
		public const string shouldScan = "shouldScan";
		public const string shouldPursue = "shouldPursue";
		public const string shouldAttack = "shouldAttack";
		public const string shouldBeStunned = "shouldBeStunned";
		public const string shouldFlee = "shouldFlee";
		public const string shouldHeal = "shouldHeal";
		public const string shouldDodge = "shouldDodge";
	}
}