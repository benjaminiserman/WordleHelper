namespace WordleHelper.Classes;
using System.Linq;

internal static class WordHelper
{
	public static bool WordHasNoDuplicateLetters(string s) => s.Length == s.ToHashSet().Count;
}
