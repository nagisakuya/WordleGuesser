using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreTweet;
using System.Linq;

namespace WordleGuesser
{
	class Program
	{
		
		const string TOKEN_PATH = @"./TOKEN.txt";
		const string ANSWER_WORD_LIST_PATH = @"./data/AnswerWordList.txt";
		const string DATA_PATH = @"./data/data.bytes";
		const int MAX_REQUEST = 10;
		const string WORDLE_ID = "275";
		static void Main(string[] args)
		{
			var answerWordList = Word.ImportWords(ANSWER_WORD_LIST_PATH);
			byte[,] vs = new byte[answerWordList.Count, Hint.COMBINATIONS];
			//data load
			var data_file = System.IO.File.OpenRead(DATA_PATH);
			for (int i = 0; i < answerWordList.Count; i++)
			{
				for (int j = 0; j < Hint.COMBINATIONS; j++)
				{
					vs[i, j] = (byte)data_file.ReadByte();
				}
			}
			data_file.Close();

			//hints initialize
			//byte[] hints = new byte[Hint.COMBINATIONS];
			/*for (int i = 0; i < hints.Length; i++)
			{
				hints[i] = 0;
			}*/
			/*for (int i = 0; ; i++)
			{
				string str = Console.ReadLine();
				if (str == "") break;
				hints[(int)new Hint(str)]++;
			}*/
			var tokens = System.IO.File.ReadAllText(TOKEN_PATH).Split("\r\n");
			var twitter_client = Tokens.Create(tokens[1], tokens[3], tokens[5], tokens[7]);
			List<Status> result = new();
			for (int i = 0; i < MAX_REQUEST; i++)
			{
				var time = result.Count == 0 ?
					DateTime.Now.ToUniversalTime()
					: result.Last().CreatedAt;
				//string u = $"{time.Year}-{time.Month}-{time.Day}_{time.Hour}";
				string u = time.ToString("yyyy-M-d_H:mm:ss_UTC");
				var temp = twitter_client.Search.TweetsAsync(count => 100, q => $"\"Wordle {WORDLE_ID}\"", until => u).Result.ToList();
				result.AddRange(temp);
				Console.WriteLine($"Request:key=\"Wordle {WORDLE_ID}\",until={u},result={temp.Count}");
				if (temp.Count <= 1) break;
			}
			Console.WriteLine($"Result:total {result.Count} tweets found");
			Console.WriteLine("===========================");

			Dictionary<int, int> possible_words = new();
			for (int i = 0; i < answerWordList.Count; i++)
			{
				possible_words[i] = 0;
			}
			foreach (var item in result)
			{
				if (!item.Text.Contains($"Wordle {WORDLE_ID}")) continue;
				var temp = item.Text.Split("\n");
				var hints = new byte[Hint.COMBINATIONS];
				string formatted_text = "";
				for (int i = 0; i < hints.Length; i++)
				{
					hints[i] = 0;
				}
				bool temp_flag = true;
				int counter = item.Text[item.Text.IndexOf($"Wordle {WORDLE_ID}") + $"Wordle {WORDLE_ID}".Length + 1] - '0';
				foreach (var line in temp)
				{
					if (line.Contains(EnumColorExtention.BLACK_BOX) || line.Contains(EnumColorExtention.WHITE_BOX) || line.Contains(EnumColorExtention.YELLOW_BOX) || line.Contains(EnumColorExtention.GREEN_BOX))
					{
						var colors = EnumColorExtention.BoxsToColor(line).ToArray();
						if (colors.Length != 5)
						{
							temp_flag = false;
							break;
						}
						hints[(int)new Hint(colors)]++;
						formatted_text += new Hint(colors) + "\n";
						counter--;
					}
					else
					{
						formatted_text += line + "\n";
					}
				}

				if (!temp_flag || counter<0) continue;
				for (int i = 0; i < answerWordList.Count; i++)
				{
					for (int j = 0; ; j++)
					{
						if (!(j < Hint.COMBINATIONS))
						{
							possible_words[i]++;
							break;
						}
						if (vs[i, j] < hints[j]) break;
					}
				}
				Console.WriteLine($"tweet_id:{item.Id}");
				Console.WriteLine(item.CreatedAt);
				Console.WriteLine(formatted_text);
				Console.WriteLine("===========================");
			}

			//search
			/*List<Word> possible_words 
			for (int i = 0; i < answerWordList.Count; i++)
			{
				for (int j = 0; ; j++)
				{
					if (!(j < Hint.COMBINATIONS))
					{
						possible_words.Add(answerWordList[i]);
						break;
					}
					if (vs[i, j] < hints[j]) break;
				}
			}*/

			//output
			var temp_list = possible_words.ToList();
			temp_list.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));
			temp_list.Reverse();
			for (int i = 0; i < 5; i++)
			{
				Console.WriteLine(answerWordList[temp_list[i].Key] + " " + temp_list[i].Value);
			}
		}
	}
}
