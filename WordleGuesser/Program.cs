using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreTweet;
using System.Linq;

namespace WordleGuesser
{
	public class Program
	{
		const string TOKEN_PATH = @"./TOKEN.txt";
		const string ANSWER_WORD_LIST_PATH = @"./data/AnswerWordList.txt";
		const string DATA_PATH = @"./data/data.bytes";
		const string CONFIG_PATH = @"./config.txt";
		static bool SHOW_TWEETS = false;
		const int MAX_TWEET_PER_REQUEST = 100;
		static void Main(string[] args)
		{
			string WORDLE_ID = "277";
			int MAX_REQUEST = 150;
			{
				var lines = System.IO.File.ReadAllText(CONFIG_PATH).Split("\r\n");
				WORDLE_ID = lines[0];
				MAX_REQUEST = int.Parse(lines[1]);
			}

			Console.WriteLine($"setting:WORDLE_ID={WORDLE_ID},MAX_REQUEST={MAX_REQUEST}");

			var answer_word_list = Word.ImportWords(ANSWER_WORD_LIST_PATH);
			var middle_data = Import_data(DATA_PATH,answer_word_list.Count);

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
				var temp = twitter_client.Search.TweetsAsync(count => MAX_TWEET_PER_REQUEST, q => $"\"Wordle {WORDLE_ID}\"", until => u).Result.ToList();
				result.AddRange(temp);
				Console.WriteLine($"Request:key=\"Wordle {WORDLE_ID}\",until={u},result={temp.Count}");
				if (temp.Count < MAX_TWEET_PER_REQUEST) break;
			}
			
			Console.WriteLine("===========================");
			Dictionary<int, int> possible_words = new();
			for (int i = 0; i < answer_word_list.Count; i++)
			{
				possible_words[i] = 0;
			}
			int vailed_tweet_counter = 0;
			bool[] tweet_vailed = new bool[result.Count];
			bool[,] tweet_possible_word = new bool[result.Count,answer_word_list.Count];
			for (int i = 0; i < result.Count; i++)
			{
				tweet_vailed[i] = false;
				for (int j = 0; j < answer_word_list.Count; j++)
				{
					tweet_possible_word[i, j] = false;
				}
			}

			for(int tweet_number=0; tweet_number < result.Count; tweet_number++)
			//foreach (var item in result)
			{
				var item = result[tweet_number];
				if (!item.Text.Contains($"Wordle {WORDLE_ID}")) continue;
				var temp = item.Text.Split("\n");
				var hints = new byte[Hint.COMBINATIONS];
				string formatted_text = "";
				for (int i = 0; i < hints.Length; i++)
				{
					hints[i] = 0;
				}
				int temp_pos = item.Text.IndexOf($"Wordle {WORDLE_ID}") + $"Wordle {WORDLE_ID}".Length + 1;
				if (!(temp_pos < item.Text.Length)) continue;
				bool temp_flag = true;
				int temp_char = item.Text[temp_pos];
				int counter;
				if(temp_char == 'X')
				{
					counter = 6;
				}
				else if(temp_char is > '0' and <= '6')
				{
					counter = temp_char - '0';
				}
				else
				{
					continue;
				}
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

				tweet_vailed[tweet_number] = true;
				vailed_tweet_counter++;
				for (int i = 0; i < answer_word_list.Count; i++)
				{
					for (int j = 0; ; j++)
					{
						if (!(j < Hint.COMBINATIONS))
						{
							possible_words[i]++;
							tweet_possible_word[tweet_number, i] = true;
							break;
						}
						if (middle_data[i, j] < hints[j]) break;
					}
				}
				if (SHOW_TWEETS)
				{
					Console.WriteLine($"tweet_id:{item.Id}");
					Console.WriteLine(item.CreatedAt);
					Console.WriteLine(formatted_text);
					Console.WriteLine("===========================");
				}
			}

			//output
			Console.WriteLine($"Result:total {result.Count} tweets found. {vailed_tweet_counter} tweets are valid.\n");
			var temp_list = possible_words.ToList();
			temp_list.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));
			temp_list.Reverse();
			for (int i = 0; i < 5; i++)
			{
				Console.WriteLine(answer_word_list[temp_list[i].Key] + " " + temp_list[i].Value);
			}

			Console.WriteLine("\nenter answer to debug(empty to exit)");
			string answer_word = Console.ReadLine();
			if (answer_word == "") return;

			int answer_word_pos = answer_word_list.IndexOf(new Word(answer_word));
			for (int i = 0; i < result.Count; i++)
			{
				if (tweet_vailed[i] && !tweet_possible_word[i, answer_word_pos])
				{
					Console.WriteLine(result[i].Id);
				}
			}

			Console.WriteLine("\npress enter to exit");
			Console.ReadLine();

		}

		public static byte[,] Import_data(string path, int answer_word_count)
		{
			byte[,] vs = new byte[answer_word_count, Hint.COMBINATIONS];
			var data_file = System.IO.File.OpenRead(path);
			for (int i = 0; i < answer_word_count; i++)
			{
				for (int j = 0; j < Hint.COMBINATIONS; j++)
				{
					vs[i, j] = (byte)data_file.ReadByte();
				}
			}
			data_file.Close();
			return vs;
		}
	}
}
