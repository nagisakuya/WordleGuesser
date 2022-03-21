using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordleGuesser
{
	public enum Color
	{
		White = 0,
		Yellow = 1,
		Green = 2,
	}
	static class EnumColorExtention
	{
		public const string WHITE_BOX = "⬛";
		public const string BLACK_BOX = "⬜";
		public const string YELLOW_BOX = "\U0001f7e8";
		public const string GREEN_BOX = "\U0001f7e9";
		public static string ToBox(this Color c)
		{
			return c switch
			{
				Color.White => WHITE_BOX,
				Color.Yellow => YELLOW_BOX,
				Color.Green => GREEN_BOX,
				_ => throw new NotImplementedException(),
			};
		}
		public static List<Color> BoxsToColor(string str)
		{
			List<Color> colors = new();
			int position = 0;
			for (int i = 0; position < str.Length; i++)
			{
				if (position + BLACK_BOX.Length <= str.Length && str.Substring(position, BLACK_BOX.Length) == BLACK_BOX)
				{
					colors.Add(Color.White);
					position += BLACK_BOX.Length;
				}
				else if (position + WHITE_BOX.Length <= str.Length && str.Substring(position, WHITE_BOX.Length) == WHITE_BOX)
				{
					colors.Add(Color.White);
					position += WHITE_BOX.Length;
				}
				else if (position + YELLOW_BOX.Length <= str.Length && str.Substring(position, YELLOW_BOX.Length) == YELLOW_BOX)
				{
					colors.Add(Color.Yellow);
					position += YELLOW_BOX.Length;
				}
				else if (position + GREEN_BOX.Length <= str.Length && str.Substring(position, GREEN_BOX.Length) == GREEN_BOX)
				{
					colors.Add(Color.Green);
					position += GREEN_BOX.Length;
				}
				else position++;
			}
			return colors;
		}
		public static string ToLetter(this Color c)
		{
			return c switch
			{
				Color.White => "W",
				Color.Yellow => "Y",
				Color.Green => "G",
				_ => throw new NotImplementedException(),
			};
		}
	}
}
