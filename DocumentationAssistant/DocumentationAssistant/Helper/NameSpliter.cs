using System.Collections.Generic;

namespace DocumentationAssistant.Helper
{
	public class NameSpliter
	{
		public static List<string> Split(string name)
		{
			List<string> words = new List<string>();
			List<char> singleWord = new List<char>();

			foreach (char c in name)
			{
				if (char.IsUpper(c) && singleWord.Count > 0)
				{
					words.Add(new string(singleWord.ToArray()));
					singleWord.Clear();
					singleWord.Add(c);
				}
				else
				{
					singleWord.Add(c);
				}
			}

			words.Add(new string(singleWord.ToArray()));

			return words;
		}
	}
}
