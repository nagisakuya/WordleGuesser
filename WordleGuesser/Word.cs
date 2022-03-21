using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordleGuesser
{
	public record Word
	{
		readonly string word;
		public Word(string str)
		{
			word = str;
		}
		public override string ToString() => word;
		public char this[int i]
		{
			get
			{
				return word[i];
			}
		}
		public Hint GetHint(Word inputted_word)
		{
			Hint hint = new(Color.White);
			bool[] used = new bool[5];
			for (int i = 0; i < used.Length; i++)
			{
				used[i] = false;
			}
			for (int i = 0; i < 5; i++)
			{
				if (inputted_word[i] == word[i])
				{
					hint[i] = Color.Green;
					used[i] = true;
				}
			}
			for (int i = 0; i < 5; i++)
			{
				if (hint[i] == Color.Green) continue;
				for (int j = 0; j < 5; j++)
				{
					if (!used[j] && inputted_word[i] == word[j])
					{
						hint[i] = Color.Yellow;
						used[j] = true;
					}
				}
			}
			return hint;
		}
		static public List<Word> ImportWords(string path)
		{
			var list = new List<Word>();
			string data = System.IO.File.ReadAllText(path);
			for (int i = 0; i * 8 <= data.Length; i++)
			{
				list.Add(new Word(data.Substring(i * 8 + 1, 5)));
			}
			return list;
		}
	}
}
