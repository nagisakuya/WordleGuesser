using System;
using System.Collections.Generic;
using System.IO;
using WordleGuesser;

namespace WordleGuesserBaseDataBuilder
{
	class Program
	{
		static void Main(string[] args)
		{
			const string ANSWER_WORD_LIST_PATH = @"./data/AnswerWordList.txt";
			const string WORD_LIST_PATH = @"./data/WordList.txt";
			var answerWordList = Word.ImportWords(ANSWER_WORD_LIST_PATH);
			var wordList = new List<Word>(answerWordList);
			wordList.AddRange(Word.ImportWords(WORD_LIST_PATH));
			byte[,] vs = new byte[answerWordList.Count, Hint.COMBINATIONS];
			for (int i = 0; i < answerWordList.Count; i++)
			{
				for (int j = 0; j < Hint.COMBINATIONS; j++)
				{
					vs[i, j] = 0;
				}
			}
			for (int i = 0; i < answerWordList.Count; i++)
			{
				for (int j = 0; j < wordList.Count; j++)
				{
					if (vs[i, (int)answerWordList[i].GetHint(wordList[j])] != byte.MaxValue - 1)
					{
						vs[i, (int)answerWordList[i].GetHint(wordList[j])]++;
					}
				}
			}
			string data_path = @"./data.bytes";
			var file = File.Create(data_path);
			for (int i = 0; i < answerWordList.Count; i++)
			{
				for (int j = 0; j < Hint.COMBINATIONS; j++)
				{
					file.WriteByte(vs[i, j]);
				}
			}
			file.Close();
		}
	}
}
