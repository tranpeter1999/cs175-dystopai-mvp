#region

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#endregion

namespace Managers
{
	public class GameManager : MonoBehaviour
	{
		private const string bossLevelText = "bobbby2004 (LVL {0}) ";

		[SerializeField]
		private CameraControl cameraControl;
		[SerializeField]
		private Text messageText;
		[SerializeField]
		private Text bossText;
		[SerializeField]
		private Slider bossTankHealthSlider;
		[SerializeField]
		private Image bossTankHealthFillImage;
		[SerializeField]
		private GameObject[] tankPrefab;
		[SerializeField]
		private TankManager[] tanks;
		private readonly float endDelay = 3f;

		private readonly int numRoundsToWin = 10;
		private readonly float startDelay = 3f;
		private WaitForSeconds endWait;
		private TankManager gameWinner;
		private int roundNumber;
		private TankManager roundWinner;
		private WaitForSeconds startWait;

		private void Start()
		{
			startWait = new WaitForSeconds(startDelay);
			endWait = new WaitForSeconds(endDelay);

			SpawnAllTanks();
			SetCameraTargets();

			StartCoroutine(GameLoop());
		}

		private void SpawnAllTanks()
		{
			tanks[TankType.player].instance =
				Instantiate(
					tankPrefab[TankType.player],
					tanks[TankType.player].spawnPoint.position,
					tanks[TankType.player].spawnPoint.rotation);

			tanks[TankType.player].playerNumber = 1;
			tanks[TankType.player].SetupPlayer();

			tanks[TankType.AI].instance =
				Instantiate(
					tankPrefab[TankType.AI],
					tanks[TankType.AI].spawnPoint.position,
					tanks[TankType.AI].spawnPoint.rotation);

			tanks[TankType.AI].playerNumber = 2;

			tanks[TankType.AI].
				SetupBoss(
					tanks[TankType.player].instance,
					bossTankHealthSlider,
					bossTankHealthFillImage);
		}

		private void SetCameraTargets()
		{
			Transform[] targets = new Transform[tanks.Length];

			for (int i = 0; i < targets.Length; i++)
			{
				targets[i] = tanks[i].instance.transform;
			}

			cameraControl.targets = targets;
		}

		private IEnumerator GameLoop()
		{
			yield return StartCoroutine(RoundStarting());
			yield return StartCoroutine(RoundPlaying());
			yield return StartCoroutine(RoundEnding());

			if (gameWinner != null)
			{
				SceneManager.LoadScene(0);
			}
			else
			{
				StartCoroutine(GameLoop());
			}
		}

		private IEnumerator RoundStarting()
		{
			ResetAllTanks();
			DisableTankControl();

			cameraControl.SetStartPositionAndSize();

			roundNumber++;
			messageText.text = "ROUND " + roundNumber;

			bossText.text = string.Format(
				bossLevelText,
				tanks[TankType.player].wins + 1);

			yield return startWait;
		}

		private IEnumerator RoundPlaying()
		{
			EnableTankControl();

			messageText.text = string.Empty;

			while (!OneTankLeft())
			{
				yield return null;
			}
		}

		private IEnumerator RoundEnding()
		{
			DisableTankControl();

			roundWinner = null;

			roundWinner = GetRoundWinner();

			if (roundWinner != null)
			{
				roundWinner.wins++;

				if (roundWinner.playerNumber == TankType.AI)
				{
					tanks[TankType.AI].LevelUpAI();
				}
			}

			gameWinner = GetGameWinner();

			messageText.text = EndMessage();

			yield return endWait;
		}

		private bool OneTankLeft()
		{
			int numTanksLeft = 0;

			for (int i = 0; i < tanks.Length; i++)
			{
				if (tanks[i].instance.activeSelf)
				{
					numTanksLeft++;
				}
			}

			return numTanksLeft <= 1;
		}

		private TankManager GetRoundWinner()
		{
			for (int i = 0; i < tanks.Length; i++)
			{
				if (tanks[i].instance.activeSelf)
				{
					return tanks[i];
				}
			}

			return null;
		}

		private TankManager GetGameWinner()
		{
			for (int i = 0; i < tanks.Length; i++)
			{
				if (tanks[i].wins == numRoundsToWin)
				{
					return tanks[i];
				}
			}

			return null;
		}

		private string EndMessage()
		{
			string message = "DRAW!";

			if (roundWinner != null)
			{
				message = roundWinner.coloredPlayerText + " WINS THE ROUND!";
			}

			message += "\n\n\n\n";

			for (int i = 0; i < tanks.Length; i++)
			{
				message += tanks[i].coloredPlayerText +
					": " +
					tanks[i].wins +
					" WINS\n";
			}

			if (gameWinner != null)
			{
				message = gameWinner.coloredPlayerText + " WINS THE GAME!";
			}

			return message;
		}

		private void ResetAllTanks()
		{
			for (int i = 0; i < tanks.Length; i++)
			{
				tanks[i].Reset();
			}
		}

		private void EnableTankControl()
		{
			for (int i = 0; i < tanks.Length; i++)
			{
				tanks[i].EnableControl();
			}
		}

		private void DisableTankControl()
		{
			for (int i = 0; i < tanks.Length; i++)
			{
				tanks[i].DisableControl();
			}
		}

		private readonly struct TankType
		{
			public const int player = 0;
			public const int AI = 1;
		}
	}
}