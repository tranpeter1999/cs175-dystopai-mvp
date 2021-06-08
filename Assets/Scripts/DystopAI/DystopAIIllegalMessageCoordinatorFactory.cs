#region

using System.Collections.Generic;
using TMPro;

#endregion

namespace DystopAI
{
	public static class DystopAIIllegalMessageCoordinatorFactory
	{
		public static DystopAIIllegalMessageCoordinator CreateInstance(
			DystopAIIllegalWordConfig[] illegalWordConfigs,
			TextMeshProUGUI chatBoxTextMesh,
			IDystopAIPlayerProfilePunisher playerProfilePunisher)
		{
			Dictionary<string, uint> punishmentDurationsByIllegalWord =
				new Dictionary<string, uint>(illegalWordConfigs.Length);

			for (uint i = 0; i < illegalWordConfigs.Length; i++)
			{
				DystopAIIllegalWordConfig illegalWordConfig = illegalWordConfigs[i];

				punishmentDurationsByIllegalWord.Add(
					illegalWordConfig.IllegalWord,
					illegalWordConfig.PunishmentDuration);
			}

			HashSet<string> illegalWords = new HashSet<string>(punishmentDurationsByIllegalWord.Keys);

			return new DystopAIIllegalMessageCoordinator(
				punishmentDurationsByIllegalWord,
				illegalWords,
				chatBoxTextMesh,
				playerProfilePunisher);
		}
	}
}