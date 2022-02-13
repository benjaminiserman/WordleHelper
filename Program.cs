﻿using InputHandler;

string[] words = File.ReadAllLines("words.txt");
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

var sortedLetters = (from kvp in frequencies orderby kvp.Value select kvp.Key).ToArray();
var sortedWords = (from kvp in values orderby kvp.Value select kvp.Key).ToArray();

var uniqueWords = (from s in sortedWords where Unique(s) select s).Reverse().ToArray();

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
			HashSet<string> set = new(words);

			for (int r = 0; r < 6; r++)
			{
				string word;

				if (r == 0) word = "arose";
				else
				{
					try
					{
						word = set.First(x => Unique(x));
					}
					catch
					{
						word = null;
					}

					word ??= set.First();
				}

				Console.WriteLine(word);

				string s = Console.ReadLine();

				if (s.Any(c => char.ToLowerInvariant(c) != 'g'))
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
				}
				else break;
			}

			break;
		}
	}
}

static bool Unique(string s) // return true if string contains no duplicate characters
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

enum Option
{
	Letters, Words, Unique, Play
}