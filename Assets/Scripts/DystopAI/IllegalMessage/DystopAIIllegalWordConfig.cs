#region

using System;
using UnityEngine;

#endregion

namespace DystopAI.IllegalMessage
{
	[Serializable]
	public class DystopAIIllegalWordConfig
	{
		[SerializeField]
		private string illegalWord = string.Empty;
		[SerializeField]
		private uint punishmentDuration = 0;

		public string IllegalWord
		{
			get { return illegalWord; }
		}

		public uint PunishmentDuration
		{
			get { return punishmentDuration; }
		}
	}
}