using System.Runtime.CompilerServices;
using InputHandler;
using WordleHelper.Classes;

var words = File.ReadAllLines("words.txt");

var player = new WordlePlayer(words, 6);
var (heuristicValuesByWord, letterFrequencies) = player.GetHeuristicValuesForWords(words);

var lettersSortedByFrequency = letterFrequencies
	.Select(kvp => (letter: kvp.Key, frequency: kvp.Value))
	.OrderByDescending(x => x.frequency)
	.Select(x => x.letter)
	.ToList();

var wordsSortedByHeuristic = heuristicValuesByWord
	.OrderByDescending(kvp => kvp.Value)
	.Select(kvp => kvp.Key)
	.ToList();

var wordsWithNoDuplicateLetters = wordsSortedByHeuristic
	.Where(WordHelper.WordHasNoDuplicateLetters)
	.ToList();

while (true)
{
	Console.WriteLine("Choose an option:\n(letters) to see letter frequencies, (words) to see heuristic values for words, or (play) to cheat at Wordle");
	var got = Input.GetOption<Option>();

	var functionChosen = got switch
	{
		Option.Letters => ShowLetterFrequencies,
		Option.Words => ShowWordHeuristicValues,
		Option.Play => PlayWordle,
		Option.Test => TestWordlePlayer,
		Option.TwentyFive => () => ExperimentalStuff.TwentyFive(wordsWithNoDuplicateLetters, heuristicValuesByWord),
		_ => (Action)(() => { })
	};

	functionChosen();
}

void ShowLetterFrequencies()
{
	Console.WriteLine("Would you like to show letters in descending order? (y/n)");
	var showInDescendingOrder = Input.GetYN();

	var letterQuery = lettersSortedByFrequency.Select((letter, i) => (i, letter, letterFrequencies[letter]));
	if (!showInDescendingOrder)
	{
		letterQuery = letterQuery.Reverse();
	}

	foreach (var (i, letter, frequency) in letterQuery)
	{
		Console.WriteLine($"{i,2}. {letter} - {frequency} occurrences");
	}
}

void ShowWordHeuristicValues()
{
	Console.WriteLine("How many words would you like to show? Choose a negative number to show from the bottom.");
	var numberOfWordsToShow = Input.Get(int.Parse);

	var showInAscendingOrder = numberOfWordsToShow < 0;
	if (numberOfWordsToShow < 0)
	{
		numberOfWordsToShow = -numberOfWordsToShow;
	}

	if (numberOfWordsToShow == 0)
	{
		numberOfWordsToShow = 20;
	}

	var wordQuery = wordsSortedByHeuristic.Select((word, i) => (i, word, heuristicValuesByWord[word]));
	if (showInAscendingOrder)
	{
		wordQuery = wordQuery.Reverse();
	}

	var padRightBy = (int)Math.Ceiling(Math.Log2(numberOfWordsToShow));

	foreach (var (i, word, value) in wordQuery)
	{
		Console.WriteLine($"{i.ToString().PadRight(padRightBy)}. {word} - {value}");
	}
}

void PlayWordle()
{
	Console.WriteLine("Enter the following guess into Wordle, then input Wordle's response. Use g for green squares, y for yellow squares, and w for white squares. Enter \"stop\" to finish early.");

	for (var guessesUsed = 0; true; guessesUsed++)
	{
		var guess = player.MakeGuess(guessesUsed);
		Console.WriteLine(guess);

		var response = Console.ReadLine();
		if (response == "stop")
		{
			break;
		}
		else if (response.Any(c => char.ToLowerInvariant(c) != 'g'))
		{
			player.ConsiderResponse(response, guess);
		}
		else
		{
			break;
		}
	}

	player.Reset(words);
}

void TestWordlePlayer() => new WordlePlayerEvaluator().EvaluateWordlePlayer(new WordlePlayer(words, 6), words, 100);

enum Option
{
	Letters, Words, Unique, Play, TwentyFive, Test
}

