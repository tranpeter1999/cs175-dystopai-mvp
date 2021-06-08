#region

using System;
using AI.TankBoss;
using Tank;
using UnityEngine;
using UnityEngine.UI;

#endregion

namespace Managers
{
	[Serializable]
	public class TankManager
	{
		public Color playerColor;
		public Transform spawnPoint;
		[HideInInspector]
		public int playerNumber;
		[HideInInspector]
		public string coloredPlayerText;
		[HideInInspector]
		public GameObject instance;
		[HideInInspector]
		public int wins;
		private AIController AIController;
		private GameObject canvasGameObject;

		private TankMovement movement;
		private TankShooting shooting;

		public void SetupBoss(
			GameObject player,
			Slider bossTankHealthSlider,
			Image bossTankHealthFillImage)
		{
			TankHealth health = instance.GetComponent<TankHealth>();
			health.SetBossHealthHUD(bossTankHealthSlider, bossTankHealthFillImage);
			AIController = instance.GetComponent<AIController>();
			AIController.Setup(player);

			shooting = instance.GetComponent<TankShooting>();
			canvasGameObject = instance.GetComponentInChildren<Canvas>().gameObject;

			shooting.playerNumber = playerNumber;

			coloredPlayerText = "<color=#" +
				ColorUtility.ToHtmlStringRGB(playerColor) +
				">PLAYER " +
				playerNumber +
				"</color>";

			MeshRenderer[] renderers =
				instance.GetComponentsInChildren<MeshRenderer>();

			for (int i = 0; i < renderers.Length; i++)
			{
				renderers[i].material.color = playerColor;
			}
		}

		public void SetupPlayer()
		{
			AIController = null;

			movement = instance.GetComponent<TankMovement>();
			shooting = instance.GetComponent<TankShooting>();
			canvasGameObject = instance.GetComponentInChildren<Canvas>().gameObject;

			movement.playerNumber = playerNumber;
			shooting.playerNumber = playerNumber;

			coloredPlayerText = "<color=#" +
				ColorUtility.ToHtmlStringRGB(playerColor) +
				">PLAYER " +
				playerNumber +
				"</color>";

			MeshRenderer[] renderers =
				instance.GetComponentsInChildren<MeshRenderer>();

			for (int i = 0; i < renderers.Length; i++)
			{
				renderers[i].material.color = playerColor;
			}
		}

		public void DisableControl()
		{
			if (AIController == null)
			{
				movement.enabled = false;
			}
			else
			{
				AIController.enabled = false;
			}

			shooting.enabled = false;

			canvasGameObject.SetActive(false);
		}

		public void EnableControl()
		{
			if (AIController == null)
			{
				movement.enabled = true;
			}
			else
			{
				AIController.enabled = true;
			}

			shooting.enabled = true;

			canvasGameObject.SetActive(true);
		}

		public void LevelUpAI() => AIController.LevelUpAI();

		public void Reset()
		{
			instance.transform.position = spawnPoint.position;
			instance.transform.rotation = spawnPoint.rotation;

			instance.SetActive(false);
			instance.SetActive(true);
		}
	}
}