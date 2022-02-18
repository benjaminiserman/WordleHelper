using System;
using System.Collections.Generic;
using InputHandler;

string[] words = File.ReadAllLines("words.txt");

var (values, frequencies) = Global.GetValues(words);

var sortedLetters = (from kvp in frequencies orderby kvp.Value select kvp.Key).ToArray();
var sortedWords = (from kvp in values orderby kvp.Value select kvp.Key).ToArray();

var uniqueWords = (from s in sortedWords where Global.Unique(s) select s).Reverse().ToArray();

while (true)
{
	Console.WriteLine("Choose an option: (letters), (words), (unique), or (play)");
	Option got = Input.GetOption<Option>();

	switch (got)
	{
		case Option.Letters:
		{
			Console.WriteLine("How many letters to show?");
			int num = Input.Get(int.Parse);

			bool ascending = num < 0;
			if (num < 0) num = -num;
			if (num == 0) num = 26;

			if (ascending)
			{
				for (int i = 0; i < num; i++)
				{
					Console.WriteLine($"{sortedLetters[i]} - {frequencies[sortedLetters[i]]}");
				}
			}
			else
			{
				for (int i = 25; i >= 26 - num; i--)
				{
					Console.WriteLine($"{sortedLetters[i]} - {frequencies[sortedLetters[i]]}");
				}
			}

			break;
		}
		case Option.Words:
		{
			Console.WriteLine("How many words to show?");
			int num = Input.Get(int.Parse);

			bool ascending = num < 0;
			if (num < 0) num = -num;
			if (num == 0) num = 26;

			if (ascending)
			{
				for (int i = 0; i < num; i++)
				{
					Console.WriteLine($"{sortedWords[i]} - {values[sortedWords[i]]}");
				}
			}
			else
			{
				for (int i = sortedWords.Length - 1; i >= sortedWords.Length - num; i--)
				{
					Console.WriteLine($"{sortedWords[i]} - {values[sortedWords[i]]}");
				}
			}

			break;
		}
		case Option.Unique:
		{
			HashSet<char> usedLetters = new();
			List<string> goodWords = new();

			Console.WriteLine("How many words would you like?");
			int num = Input.Get(int.Parse);

			foreach (string s in uniqueWords)
			{
				if (s.Any(x => usedLetters.Contains(x))) continue;
				else
				{
					goodWords.Add(s);
					foreach (char c in s) usedLetters.Add(c);

					if (goodWords.Count == num) break;
				}
			}

			while (goodWords.Count < num)
			{
				string s = uniqueWords.MinBy(s => s.Count(x => usedLetters.Contains(x)));

				goodWords.Add(s);
				foreach (char c in s) usedLetters.Add(c);
			}

			foreach (string s in goodWords)
			{
				Console.WriteLine(s);
			}

			break;
		}
		case Option.Play:
		{
			Player p = new(words);

			while (true)
			{
				Console.WriteLine(p.Guess());

				string s = Console.ReadLine();
				if (s.Any(x => char.ToLowerInvariant(x) != 'g')) p.Response(s);
				else break;
			}

			break;
		}
		case Option.Test:
		{
			var dist = Global.Test(new Player(words), words, 100);

			break;
		}
		case Option.TwentyFive:
		{
			HashSet<char> usedLetters = new();
			List<string> bestWords = new();
			int bestScore = 0;
			int searched = -1;

			Recurse(usedLetters, bestWords, null);

			void Recurse(HashSet<char> inLetters, List<string> inWords, string chosen)
			{
				HashSet<char> usedLetters = new(inLetters);
				List<string> words = new(inWords);

				searched++;
				if (searched % 100 == 0) Console.WriteLine($"searched: {searched}... best = {bestScore}/{bestWords.Count}");

				if (chosen is not null)
				{
					words.Add(chosen);
					foreach (char c in chosen) usedLetters.Add(c);
				}

				if (words.Count < 5)
				{
					foreach (string s in uniqueWords)
					{
						if (s.Any(x => usedLetters.Contains(x))) continue;
						else
						{
							Recurse(usedLetters, words, s);
						}
					}
				}

				if (bestWords.Count < words.Count)
				{
					bestWords = words;
					bestScore = words.Select(x => values[x]).Sum();

					Console.WriteLine($"After {searched}, found ({bestScore}):");

					foreach (string word in bestWords)
					{
						Console.WriteLine(word);
					}

				}
				else if (bestWords.Count == words.Count)
				{
					int thisScore = words.Select(x => values[x]).Sum();
					if (thisScore > bestScore)
					{
						bestWords = words;
						bestScore = thisScore;

						Console.WriteLine($"After {searched}, found ({bestScore}):");

						foreach (string word in bestWords)
						{
							Console.WriteLine(word);
						}
					}
				}
			}

			break;
		}
	}
}

internal static class Global
{
	public static bool Unique(string s) // return true if string contains no duplicate characters
	{
		for (int i = 0; i < s.Length - 1; i++)
		{
			for (int j = i + 1; j < s.Length; j++)
			{
				if (s[i] == s[j]) return false;
			}
		}

		return true;
	}

	public static HashSet<string> RestrictSet(HashSet<string> set, string s, string word)
	{
		for (int i = 0; i < s.Length; i++)
		{
			switch (char.ToLowerInvariant(s[i]))
			{
				case 'y':
				{
					set.RemoveWhere(x => !x.Contains(word[i]) || x[i] == word[i]);
					break;
				}
				case 'g':
				{
					set.RemoveWhere(x => x[i] != word[i]);
					break;
				}
				default:
				{
					set.RemoveWhere(x => x.Contains(word[i]));
					break;
				}
			}
		}

		return set;
	}

	public static Dictionary<int, int> Test(Player p, string[] words, int interval)
	{
		int w = 0;
		Dictionary<int, int> dist = new int[] { -1, 1, 2, 3, 4, 5, 6 }.ToDictionary(x => x, x => 0);
		foreach (var s in words)
		{
			for (int i = 0; i < 6; i++)
			{
				string guess = p.Guess();

				string response = new(guess.Select((c, i) => Match((c, i), s)).ToArray());

				if (response.Any(x => char.ToLowerInvariant(x) != 'g')) p.Response(response);
				else
				{
					dist[i + 1]++;
					goto Next;
				}
			}

			dist[-1]++;

			Next:;
			p.Clear(words);

			if (++w % interval == 0)
			{
				Console.WriteLine($"After {w}:");
				Log();
			}
		}

		Console.WriteLine("Final:");
		Log();

		return dist;

		char Match((char c, int i) t, string s)
		{
			(char c, int i) = t;

			if (s[i] == c) return 'g';
			if (s.Contains(c)) return 'y';
			return 'w';
		}

		void Log()
		{
			Console.WriteLine($"RATE: {1 - (double)dist[-1] / dist.Values.Sum()}");
			Console.WriteLine($"AVG: {(double)dist.Where(kvp => kvp.Key != -1).Sum(kvp => kvp.Key * kvp.Value) / dist.Where(kvp => kvp.Key != -1).Select(kvp => kvp.Value).Sum()}");

			foreach (var kvp in dist)
			{
				Console.WriteLine($"{kvp.Key} => {kvp.Value}");
			}
		}
	}

	public static (Dictionary<string, int>, Dictionary<char, int>) GetValues(IEnumerable<string> words)
	{
		Dictionary<string, int> values = words.ToDictionary(x => x, y => 0);

		Dictionary<char, int> frequencies = new();
		foreach (string word in words) // get letter frequencies
		{
			foreach (char c in word)
			{
				char lower = char.ToLowerInvariant(c);

				if (frequencies.ContainsKey(lower)) frequencies[lower]++;
				else frequencies.Add(lower, 1);
			}
		}

		foreach (string word in words) // get word values
		{
			foreach (char c in word)
			{
				values[word] += frequencies[c];
			}
		}

		return (values, frequencies);
	}
}

enum Option
{
	Letters, Words, Unique, Play, TwentyFive, Test
}

class Player
{
	HashSet<string> set;
	IEnumerable<string> sorted;
	int r;
	string word;
	Dictionary<string, int> values;

	public Player(string[] words)
	{
		(values, _) = Global.GetValues(words);
		var sortedWords = (from kvp in values orderby kvp.Value select kvp.Key).ToArray();

		set = new(sortedWords);
	}

	public virtual string Guess()
	{
		if (r++ == 0) word = "arose";
		else
		{
			try
			{
				word = sorted.First(x => Global.Unique(x));
			}
			catch
			{
				word = null;
			}

			word ??= sorted.First();
		}

		return word;
	}

	public virtual void Response(string s)
	{
		Global.RestrictSet(set, s, word);
		(values, _) = Global.GetValues(set.ToArray());
		sorted = from x in set orderby values[x] descending select x;
	}

	public virtual void Clear(string[] words)
	{
		set = new(words);
		r = 0;
		word = null;
	}
}