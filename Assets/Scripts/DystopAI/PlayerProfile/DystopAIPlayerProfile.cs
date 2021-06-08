namespace DystopAI.PlayerProfile
{
	public class DystopAIPlayerProfile : IDystopAIPlayerProfilePunisher
	{
		public float MuteDuration { get; set; }

		public void AddMuteDuration(uint muteDuration) => MuteDuration += muteDuration;

		public void ElapseMuteDuration(float timeElapsed)
		{
			if (MuteDuration < 0)
			{
				return;
			}

			MuteDuration -= timeElapsed;

			if (MuteDuration < 0)
			{
				MuteDuration = 0;
			}
		}
	}
}