#region

using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Windows.Speech;

#endregion

namespace DystopAI
{
	public class DystopAIIllegalMessageCoordinator
	{
		private const string MuteMessage =
			"You have been muted for a total of {0} for saying \'<color=red>{1}</color>\'";

		public DystopAIIllegalMessageCoordinator(
			IReadOnlyDictionary<string, uint> punishmentDurationsByIllegalWord,
			HashSet<string> illegalWords,
			TextMeshProUGUI chatBoxTextMesh,
			IDystopAIPlayerProfilePunisher playerProfilePunisher)
		{
			PunishmentDurationsByIllegalWord = punishmentDurationsByIllegalWord;
			IllegalWords = illegalWords;
			ChatBoxTextMesh = chatBoxTextMesh;
			PlayerProfilePunisher = playerProfilePunisher;
		}

		private IDystopAIPlayerProfilePunisher PlayerProfilePunisher { get; }

		private IReadOnlyDictionary<string, uint> PunishmentDurationsByIllegalWord { get; }

		private HashSet<string> IllegalWords { get; }

		private TextMeshProUGUI ChatBoxTextMesh { get; }

		public DictationRecognizer.DictationHypothesisDelegate ParseInput() =>
			text =>
			{
				Debug.LogFormat("Dictation hypothesis: {0}", text);

				IReadOnlyList<string> badWords =
					DystopAIIllegalMessageUtility.GetIllegalWords(IllegalWords, text);

				if (badWords.Count == 0)
				{
					return;
				}

				AdministerPunishment(badWords);

				ChatBoxTextMesh.text = AddChatBoxMessage(ChatBoxTextMesh.text, badWords);
			};

		private string AddChatBoxMessage(string chatBoxText, IReadOnlyList<string> badWords)
		{
			StringBuilder chatBoxMessages = new StringBuilder(chatBoxText);
			string illegalWords = DystopAIIllegalMessageUtility.FormatIllegalWords(badWords);

			TimeSpan timeSpan = TimeSpan.FromSeconds(PlayerProfilePunisher.MuteDuration);

			string displayTime = $"{timeSpan.Hours} hours, {timeSpan.Minutes} minutes, {timeSpan.Seconds} seconds";

			chatBoxMessages.AppendLine(string.Format(MuteMessage, displayTime, illegalWords));

			return chatBoxMessages.ToString();
		}

		private void AdministerPunishment(IReadOnlyList<string> badWords)
		{
			uint muteDuration = 0;

			for (int i = 0; i < badWords.Count; i++)
			{
				muteDuration += PunishmentDurationsByIllegalWord[badWords[i]];
			}

			PlayerProfilePunisher.AddMuteDuration(muteDuration * 60);
		}
	}
}