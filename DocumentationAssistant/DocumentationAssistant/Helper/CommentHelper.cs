using System.Collections.Generic;
using System.Linq;

namespace DocumentationAssistant.Helper
{
	public static class CommentHelper
	{
		public static string GetClassComment(string name)
		{
			return GetCommonComment(name);
		}

		public static string GetFieldComment(string name)
		{
			return GetCommonComment(name);
		}

		public static string GetConstructorComment(string name, bool isPrivate)
		{
			if (isPrivate)
			{
				return $"Prevents a default instance of the <see cref=\"{name}\"/> class from being created.";
			}
			else
			{
				return $"Initializes a new instance of the <see cref=\"{name}\"/> class.";
			}
		}

		public static string GetPropertyComment(string name, bool isBoolean, bool hasSetter)
		{
			string comment = "Gets";
			if (hasSetter)
			{
				comment += " or sets";
			}

			if (isBoolean)
			{
				comment += GetPropertyBooleanPart(name);
			}
			else
			{
				comment += " the " + string.Join(" ", SpilitNameAndToLower(name, true));
			}
			return comment + ".";
		}

		public static string GetMethodComment(string name)
		{
			List<string> parts = SpilitNameAndToLower(name, false);
			parts[0] = parts[0] + "s";
			return string.Join(" ", parts) + ".";
		}

		public static string GetParameterComment(string name)
		{
			return GetCommonComment(name);
		}

		private static string GetPropertyBooleanPart(string name)
		{
			string booleanPart = " a value indicating whether ";

			List<string> parts = SpilitNameAndToLower(name, true).ToList();

			string isWord = parts.FirstOrDefault(o => o == "is");
			if (isWord != null)
			{
				parts.Remove(isWord);
				parts.Insert(parts.Count - 1, isWord);
			}

			booleanPart += string.Join(" ", parts);
			return booleanPart;
		}

		private static string GetCommonComment(string name)
		{
			return $"The {string.Join(" ", SpilitNameAndToLower(name, true))}.";
		}

		private static List<string> SpilitNameAndToLower(string name, bool includeFirst)
		{
			List<string> parts = NameSpliter.Split(name);

			int i = includeFirst ? 0 : 1;
			for (; i < parts.Count; i++)
			{
				parts[i] = parts[i].ToLower();
			}
			return parts;
		}
	}
}
