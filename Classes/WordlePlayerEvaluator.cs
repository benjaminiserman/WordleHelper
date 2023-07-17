namespace WordleHelper.Classes;
using System;
using System.Collections.Generic;
using System.Linq;

internal class WordlePlayerEvaluator
{
	public Dictionary<int, int> EvaluateWordlePlayer(WordlePlayer player, string[] wordList, int printInterval)
	{
		var intervalCounter = 0;
		var resultDistribution = new int[] { -1, 1, 2, 3, 4, 5, 6 }
			.ToDictionary(x => x, x => 0);
		var guesses = new string[player.MaxGuesses];
		
		foreach (var word in wordList)
		{
			var wonRound = false;
			foreach (var round in Enumerable.Range(1, 6))
			{
				var guess = player.MakeGuess(round - 1);
				guesses[round - 1] = guess;

				var response = JudgeGuess(guess, word);

				if (response.Any(x => char.ToLowerInvariant(x) != 'g'))
				{
					player.ConsiderResponse(response, guess);
				}
				else
				{
					resultDistribution[round]++;
					wonRound = true;
					break;
				}
			}

			if (!wonRound)
			{
				resultDistribution[-1]++;
				/*Console.WriteLine($"Word: {word}");
				foreach (var guess in guesses)
				{
					Console.WriteLine($"{guess} - {JudgeGuess(guess, word)}");
				}

				break;*/
			}

			player.Reset(wordList);

			if (++intervalCounter % printInterval == 0)
			{
				Console.WriteLine($"After {intervalCounter}:");
				PrintCurrentStats();
			}
		}

		Console.WriteLine("Final:");
		PrintCurrentStats();

		return resultDistribution;

		string JudgeGuess(string guess, string actualWord)
		{
			return new(guess
				.Select((letter, i) =>
				{
					if (actualWord[i] == guess[i])
					{
						return 'g';
					}

					if (actualWord.Contains(guess[i]))
					{
						return 'y';
					}

					return 'w';
				})
				.ToArray());
		}

		void PrintCurrentStats()
		{
			Console.WriteLine($"RATE: {1 - (double)resultDistribution[-1] / resultDistribution.Values.Sum()}");
			Console.WriteLine($"AVG: {(double)resultDistribution.Where(kvp => kvp.Key != -1).Sum(kvp => kvp.Key * kvp.Value) / resultDistribution.Where(kvp => kvp.Key != -1).Select(kvp => kvp.Value).Sum()}");

			foreach (var kvp in resultDistribution)
			{
				Console.WriteLine($"{kvp.Key} => {kvp.Value}");
			}
		}
	}
}
