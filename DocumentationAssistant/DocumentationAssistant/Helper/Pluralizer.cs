using ThirdPartPluralizer = Pluralize.NET;

namespace DocumentationAssistant.Helper
{
	public static class Pluralizer
	{
		public static string Pluralize(string word)
		{
			return new ThirdPartPluralizer.Pluralizer().Pluralize(word);
		}
	}
}
