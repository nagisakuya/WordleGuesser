using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordleGuesser
{

	public record Hint
	{
		readonly Color[] colors;
		static public readonly int COMBINATIONS = (int)Math.Pow(3, 5);
		public Hint(Color c1, Color c2, Color c3, Color c4, Color c5)
		{
			this.colors = new Color[5] { c1, c2, c3, c4, c5 };
		}
		public Hint(Color[] c)
		{
			colors = c;
		}
		private Hint()
		{
			this.colors = new Color[5];
		}
		public Hint(Color c)
		{
			colors = new Color[5];
			for (int i = 0; i < colors.Length; i++)
			{
				colors[i] = c;
			}
		}
		static public explicit operator int(Hint from)
		{
			int to = 0;
			for (int i = 0; i < from.colors.Length; i++)
			{
				to *= 3;
				to += (int)from.colors[i];
			}
			return to;
		}
		static public explicit operator Hint(int from)
		{
			Hint to = new();
			for (int i = to.colors.Length; --i >= 0;)
			{
				to.colors[i] = (Color)(from % 3);
				from /= 3;
			}
			return to;
		}
		public override string ToString()
		{
			String stream = "";
			for (int i = 0; i < colors.Length; i++)
			{
				stream += colors[i].ToLetter();
			}
			return stream;
		}
		public Color this[int i]
		{
			set
			{
				colors[i] = value;
			}
			get
			{
				return colors[i];
			}
		}
	}
}
