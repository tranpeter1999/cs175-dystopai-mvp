#region

using System.Collections.Generic;
using System.Text;

#endregion

namespace DystopAI.IllegalMessage
{
	public static class DystopAIIllegalMessageUtility
	{
		public static List<string> GetIllegalWords(HashSet<string> illegalWords, string text)
		{
			string[] words = text.ToLower().Split();

			List<string> badWords = new List<string>();

			for (uint i = 0; i < words.Length; i++)
			{
				if (illegalWords.Contains(words[i]))
				{
					badWords.Add(words[i]);
				}
			}

			return badWords;
		}

		public static string FormatIllegalWords(IReadOnlyList<string> illegalWords)
		{
			StringBuilder illegalWordMessageBuilder = new StringBuilder();

			for (int i = 0; i < illegalWords.Count; i++)
			{
				string badWord = illegalWords[i];

				if (i == (illegalWords.Count - 1))
				{
					illegalWordMessageBuilder.Append(badWord);
				}
				else
				{
					illegalWordMessageBuilder.AppendFormat(badWord, "{0}, ");
				}
			}

			return illegalWordMessageBuilder.ToString();
		}
	}
}