#region

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Windows.Speech;

#endregion

namespace DystopAI
{
	[DefaultExecutionOrder(-95)]
	public class DystopAICoordinator : MonoBehaviour
	{
		[Header("Illegal Words Settings")]
		[SerializeField]
		private DystopAIIllegalWordConfig[] illegalWordConfigs = null;

		[Header("UI References")]
		[SerializeField]
		private TextMeshProUGUI chatBoxTextMesh = null;


		private DictationRecognizer DictationRecognizer { get; set; }

		private DystopAIPlayerProfile[] DystopAIPlayerProfiles { get; set; }

		private void Start()
		{
			DystopAIPlayerProfile dystopAIPlayerProfile = new DystopAIPlayerProfile();

			DystopAIPlayerProfiles = new[]
				{ dystopAIPlayerProfile };

			DystopAIIllegalMessageCoordinator illegalMessageCoordinator =
				DystopAIIllegalMessageCoordinatorFactory.CreateInstance(
					illegalWordConfigs,
					chatBoxTextMesh,
					dystopAIPlayerProfile);

			DictationRecognizer = new DictationRecognizer();

			DictationRecognizer.DictationResult += (text, confidence) =>
			{
				Debug.LogFormat("Dictation result: {0}", text);
			};

			DictationRecognizer.DictationHypothesis += illegalMessageCoordinator.ParseInput();

			DictationRecognizer.DictationComplete += completionCause =>
			{
				if (completionCause != DictationCompletionCause.Complete)
				{
					Debug.LogWarningFormat(
						"Dictation completed unsuccessfully: {0}.",
						completionCause);
				}
			};

			DictationRecognizer.DictationError += (error, hresult) =>
			{
				Debug.LogWarningFormat("Dictation error: {0}; HResult = {1}.", error, hresult);
			};

			DictationRecognizer.Start();
		}

		private void FixedUpdate()
		{
			for (uint i = 0; i < DystopAIPlayerProfiles.Length; i++)
			{
				DystopAIPlayerProfiles[i].ElapseMuteDuration(Time.fixedDeltaTime);
			}
		}
	}
}