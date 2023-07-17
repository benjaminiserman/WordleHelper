namespace WordleHelper.Classes;
using System;
using System.Collections.Generic;
using System.Linq;

internal static class ExperimentalStuff
{
	public static void TwentyFive(List<string> wordsWithNoDuplicateLetters, Dictionary<string, int> heuristicValuesByWord)
	{
		HashSet<char> usedLetters = new();
		List<string> bestWords = new();
		var bestScore = 0;
		var searched = -1;

		Recurse(usedLetters, bestWords, null);

		void Recurse(HashSet<char> inLetters, List<string> inWords, string chosen)
		{
			HashSet<char> usedLetters = new(inLetters);
			List<string> words = new(inWords);

			searched++;
			if (searched % 100 == 0)
			{
				Console.WriteLine($"searched: {searched}... best = {bestScore}/{bestWords.Count}");
			}

			if (chosen is not null)
			{
				words.Add(chosen);
				foreach (var c in chosen)
				{
					usedLetters.Add(c);
				}
			}

			if (words.Count < 5)
			{
				foreach (var s in wordsWithNoDuplicateLetters)
				{
					if (s.Any(x => usedLetters.Contains(x)))
					{
						continue;
					}
					else
					{
						Recurse(usedLetters, words, s);
					}
				}
			}

			if (bestWords.Count < words.Count)
			{
				bestWords = words;
				bestScore = words.Select(x => heuristicValuesByWord[x]).Sum();

				Console.WriteLine($"After {searched}, found ({bestScore}):");

				foreach (var word in bestWords)
				{
					Console.WriteLine(word);
				}

			}
			else if (bestWords.Count == words.Count)
			{
				var thisScore = words.Select(x => heuristicValuesByWord[x]).Sum();
				if (thisScore > bestScore)
				{
					bestWords = words;
					bestScore = thisScore;

					Console.WriteLine($"After {searched}, found ({bestScore}):");

					foreach (var word in bestWords)
					{
						Console.WriteLine(word);
					}
				}
			}
		}
	}
}
