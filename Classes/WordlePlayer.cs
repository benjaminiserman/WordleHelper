namespace WordleHelper.Classes;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using WordleHelper.Enums;

internal class WordlePlayer
{
    private HashSet<string> _remainingWords;
    public ImmutableHashSet<string> RemainingWords => _remainingWords.ToImmutableHashSet();

    private List<string> _remainingWordsSortedByHeuristic = new();
	private List<string> _wordList = new();

	private List<string> _guesses = new();
	public ImmutableList<string> Guesses => _guesses.ToImmutableList();

	public static string FirstGuess => "arose";

    public int MaxGuesses { get; private set; }

    public WordlePlayer(string[] words, int maxGuesses)
    {
        MaxGuesses = maxGuesses;

		Reset(words);
    }

	public static HashSet<string> FilterImpossibleGuessesFromSet(HashSet<string> remainingWords, Response[] wordleResponse, string wordleGuess)
	{
        foreach (var ((letterGuessed, responseForLetter), i) in wordleGuess
            .Zip(wordleResponse)
            .Select((x, i) => (x, i)))
        {
            remainingWords.RemoveWhere(responseForLetter switch
            {
                Response.Green => word => word[i] != letterGuessed,
                Response.Yellow => word => !word.Contains(letterGuessed) || word[i] == letterGuessed,
                Response.White => word => word.Contains(letterGuessed),
                _ => throw new Exception($"Unsupported Response type {responseForLetter}")
            });
        }

		return remainingWords;
	}

	private string ChooseGuess(int guessesUsed)
	{
		if (guessesUsed == 0)
		{
			return FirstGuess;
		}

		if (_remainingWords.Count < MaxGuesses - guessesUsed)
		{
			return _remainingWords.First();
		}

		//Console.WriteLine($"remaining: {_remainingWords.Aggregate((a, b) => a + " " + b)}");
		var variance = JudgeSetVariance(_remainingWords, _guesses);
		//Console.WriteLine($"variance: {variance}");
		if (variance > MaxGuesses - guessesUsed + 2)
		{
			int CountNewLetters(string word) => word.Count(letter => _guesses.Any(guess => guess.Contains(letter)));

			var (wordHeuristicValues, _) = GetHeuristicValuesForWords(_wordList);
			var possibleSecondGuesses = _wordList
				.Where(WordHelper.WordHasNoDuplicateLetters)
				.GroupBy(CountNewLetters)
				.OrderBy(group => group.Key)
				.First()
				.OrderByDescending(word => JudgeSetVariance(_remainingWords, _guesses.Concat(Enumerable.Repeat(word, 1))))
				.ToList();

			if (possibleSecondGuesses.Count > 1 && variance - JudgeSetVariance(_remainingWords, _guesses.Concat(possibleSecondGuesses.Take(1))) > 1)
			{
				//Console.WriteLine($"guess: {possibleSecondGuesses.First()}, new_v: {JudgeSetVariance(_remainingWords, _guesses.Concat(possibleSecondGuesses.Take(1)))}");
				return possibleSecondGuesses.First();
			}
		}

		var guess = _remainingWordsSortedByHeuristic.FirstOrDefault(WordHelper.WordHasNoDuplicateLetters);
		guess ??= _remainingWordsSortedByHeuristic.FirstOrDefault();
		guess ??= FirstGuess;

		return guess;
	}

	public virtual string MakeGuess(int guessesUsed)
	{
		var guess = ChooseGuess(guessesUsed);
		_guesses.Add(guess);
		return guess;
    }

    public virtual void ConsiderResponse(string wordleResponse, string wordleGuess)
    {
        var convertedResponse = wordleResponse.ConvertToResponseArray();

        _remainingWords = FilterImpossibleGuessesFromSet(_remainingWords, convertedResponse, wordleGuess);

        var (wordHeuristicValues, _) = GetHeuristicValuesForWords(_remainingWords);
		_remainingWordsSortedByHeuristic = _remainingWords
			.OrderByDescending(word => wordHeuristicValues[word])
			.ToList();
    }

    public virtual void Reset(string[] words)
    {
		_wordList = words.ToList();
		_guesses.Clear();

		var (values, _) = GetHeuristicValuesForWords(words);
		var sortedWords = values
			.OrderBy(kvp => kvp.Value)
			.Select(kvp => kvp.Key)
			.ToList();

		_remainingWords = new(sortedWords);
	}

	public (Dictionary<string, int>, Dictionary<char, int>) GetHeuristicValuesForWords(IEnumerable<string> words)
	{
		var values = words.ToDictionary(x => x, y => 0);

		Dictionary<char, int> frequencies = new();
		foreach (var word in words) // get letter frequencies
		{
			foreach (var c in word)
			{
				var lowercaseLetter = char.ToLowerInvariant(c);

				if (frequencies.ContainsKey(lowercaseLetter))
				{
					frequencies[lowercaseLetter]++;
				}
				else
				{
					frequencies.Add(lowercaseLetter, 1);
				}
			}
		}

		foreach (var word in words) // get word values
		{
			foreach (var c in word)
			{
				values[word] += frequencies[c];
			}
		}

		return (values, frequencies);
	}

	public static int JudgeSetVariance(IEnumerable<string> words, IEnumerable<string> guesses)
	{
		var wordListLetterSet = new HashSet<char>();

		foreach (var word in words)
		{
			foreach (var letter in word)
			{
				wordListLetterSet.Add(letter);
			}
		}

		var guessLetterSet = new HashSet<char>();

		foreach (var word in guesses)
		{
			foreach (var letter in word)
			{
				guessLetterSet.Add(letter);
			}
		}

		return wordListLetterSet.Except(guessLetterSet).Count();
	}
}