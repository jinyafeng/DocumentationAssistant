using ThirdPartPluralizer = Pluralize.NET;

namespace DocumentationAssistant.Helper
{
	/// <summary>
	/// The pluralizer to pluralize word.
	/// </summary>
	public static class Pluralizer
	{
		/// <summary>
		/// Pluralizes word.
		/// </summary>
		/// <param name="word">The word.</param>
		/// <returns>A plural word.</returns>
		public static string Pluralize(string word)
		{
			return new ThirdPartPluralizer.Pluralizer().Pluralize(word);
		}
	}
}
