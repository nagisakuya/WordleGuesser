using System;
using WordleGuesser;

namespace WordleDataViewer
{
	class Program
	{
		static void Main(string[] args)
		{
			const string ANSWER_WORD_LIST_PATH = @"./data/AnswerWordList.txt";
			const string DATA_PATH = @"./data/data.bytes";
			var answer_word_list = Word.ImportWords(ANSWER_WORD_LIST_PATH);
			var middle_data = WordleGuesser.Program.Import_data(DATA_PATH, answer_word_list.Count);

			while (true)
			{
				string answer_word = Console.ReadLine();
				if (answer_word == "") return;
				int answer_word_pos = answer_word_list.IndexOf(new Word(answer_word));
				for (int i = 0; i < Hint.COMBINATIONS; i++)
				{
					Console.WriteLine($"{(Hint)i}={middle_data[answer_word_pos, i]}");
				}
			}
		}
	}
}
