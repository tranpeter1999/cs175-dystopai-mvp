namespace DystopAI
{
	public interface IDystopAIPlayerProfilePunisher
	{
		void AddMuteDuration(uint muteDuration);
		
		float MuteDuration { get; }
	}
}