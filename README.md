# WordleHelper
![Downloads](https://img.shields.io/github/downloads/winggar/WordleHelper/total?style=for-the-badge)

A console app that helps you win Wordle every time.

## Prerequisites

Before you begin, ensure you have met the following requirements:
- [You use a machine supported by .NET 6](https://github.com/dotnet/core/blob/main/release-notes/6.0/supported-os.md)
- You have .NET 6 installed
- You have downloaded the file "build.zip" from the latest release

OR

- [You use a **Windows** machine supported by .NET 6](https://github.com/dotnet/core/blob/main/release-notes/6.0/supported-os.md)
- You **do not** need to have .NET 6 installed
- You have downloaded the file "standalone.zip" from the latest release

OR

- [You use a **Linux** machine supported by .NET 6](https://github.com/dotnet/core/blob/main/release-notes/6.0/supported-os.md)
- You **do not** need to have .NET 6 installed
- You have downloaded the file "linux_standalone.zip" from the latest release

Due to low demand, standalone builds for Mac OSX are not provided. If you'd like a standalone build for Mac OSX, [contact me](mailto:winggar1228@gmail.com).

## Installation

Window:
1. Download either "build.zip" or "standalone.zip" from the latest release, depending on your prerequisites.
2. Unzip the file.
3. Find the file "WordleHelper.exe" within the unzipped folder and run it.

Linux:
1. Download "linux_standalone.zip" from the latest release.
2. Unzip the file.
3. Depending on your architecture, open either the x64 or the arm folder.
4. Copy the three files within into a directory of your choice.
5. Run "WordleHelper".

## Usage

WordleHelper has four options: (letters), (words), (unique), and (play).

(letters) gives a frequency table for letters in 5-letter words. Enter a negative number to get reverse order.

(words) gives a value table for 5-letter words. The value of a 5-letter word is determined by the sum of the frequencies of its letters. Enter a negative number to get reverse order.

(unique) gives a list of 5 words that have been calculated to be the best 5 Wordle guesses to use without receiving any information about each guess.

(play) tells you words to enter. After entering a word, enter a string representing what Wordle returned about your guess. For example, if you get the following output:

![image](https://user-images.githubusercontent.com/10767513/153775881-7fb7a0c5-9eb9-483a-be47-13f5788fc775.png)

you should enter "YGYWW" (case insensitive, any character other than 'Y' and 'G' represents a white tile).

Enter any number of 'g's to go back to the menu.

To change the word list, just replace words.txt with any other similarly formatted word list. WordleHelper works for any number of rounds or letters, and any language's characters.

## Contribution
To contribute to WordleHelper, follow these steps:

1. Fork this repository.
2. Create a branch: `git checkout -b <branch_name>`.
3. Make your changes and commit them: `git commit -m '<commit_message>'`
4. Push to the original branch: `git push origin <project_name>/<location>`
5. Create the pull request.

Alternatively see the GitHub documentation on [creating a pull request](https://help.github.com/en/github/collaborating-with-issues-and-pull-requests/creating-a-pull-request).

## Credit
Credit to: https://github.com/charlesreid1/five-letter-words for the default five letter words list.

## License

![License](https://img.shields.io/github/license/winggar/WordleHelper?style=for-the-badge)
