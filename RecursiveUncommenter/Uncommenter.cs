using System.IO;
using System.Text;

namespace RecursiveUncommenter
{
	public class Uncommenter
	{
		private enum State
		{
			Normal,
			SingleQuote,
			SingleQuoteBackslash,
			DoubleQuote,
			DoubleQuoteBackslash,
			At,
			AtDoubleQuote,
			AtDoubleQuoteDoubleQuote,
			Slash,
			SlashSlash,
			SlashStar,
			SlashStarStar,
		}

		public void Uncomment(string file)
		{
			string buffer;

			using (var reader = new StreamReader(file))
			{
				buffer = Uncomment(reader);
			}

			using (var writer = new StreamWriter(file))
			{
				writer.Write(buffer);
			}
		}

		public string Uncomment(StreamReader reader)
		{
			var buffer = new StringBuilder();
			string line;
			State state = State.Normal;

			while ((line = reader.ReadLine()) != null)
			{
				int i = 0;
				char[] buf = new char[line.Length];

				switch (state)
				{
					case State.SingleQuote:
					case State.SingleQuoteBackslash:
					case State.DoubleQuote:
					case State.DoubleQuoteBackslash:
					case State.At:
					case State.Slash:
					case State.SlashSlash:
					{
						state = State.Normal;
						break;
					}
					case State.SlashStarStar:
					{
						state = State.SlashStar;
						break;
					}
				}

				foreach (char c in line)
				{
					bool print = true;

					switch (state)
					{
						case State.Normal:
						{
							switch (c)
							{
								case '\'': { state = State.SingleQuote; break; }
								case '"': { state = State.DoubleQuote; break; }
								case '@': { state = State.At; break; }
								case '/': { state = State.Slash; print = false; break; }
							}
							break;
						}
						case State.SingleQuote:
						{
							switch (c)
							{
								case '\'': { state = State.Normal; break; }
								case '\\': { state = State.SingleQuoteBackslash; break; }
							}
							break;
						}
						case State.SingleQuoteBackslash:
						{
							state = State.SingleQuote;
							break;
						}
						case State.DoubleQuote:
						{
							switch (c)
							{
								case '"': { state = State.Normal; break; }
								case '\\': { state = State.DoubleQuoteBackslash; break; }
							}
							break;
						}
						case State.DoubleQuoteBackslash:
						{
							state = State.DoubleQuote;
							break;
						}
						case State.At:
						{
							switch (c)
							{
								case '"': { state = State.AtDoubleQuote; break; }
								default: { state = State.Normal; break; }
							}
							break;
						}
						case State.AtDoubleQuote:
						{
							switch (c)
							{
								case '"': { state = State.AtDoubleQuoteDoubleQuote; break; }
							}
							break;
						}
						case State.AtDoubleQuoteDoubleQuote:
						{
							switch (c)
							{
								case '"': { state = State.AtDoubleQuote; break; }
								default: { state = State.Normal; break; }
							}
							break;
						}
						case State.Slash:
						{
							switch (c)
							{
								case '/': { state = State.SlashSlash; print = false; break; }
								case '*': { state = State.SlashStar; print = false; break; }
								default: { state = State.Normal; buf[i++] = '/'; break; }
							}
							break;
						}
						case State.SlashSlash:
						{
							print = false;
							break;
						}
						case State.SlashStar:
						{
							switch (c)
							{
								case '*': { state = State.SlashStarStar; break; }
							}
							print = false;
							break;
						}
						case State.SlashStarStar:
						{
							switch (c)
							{
								case '*': { break; }
								case '/': { state = State.Normal; break; }
								default: { state = State.SlashStar; break; }
							}
							print = false;
							break;
						}
					}

					if (print)
						buf[i++] = c;
				}

				if (i > 0)
				{
					buffer.AppendLine(new string(buf, 0, i));
				}
				else if (line.Length == 0)
				{
					buffer.AppendLine();
				}
			}

			return buffer.ToString();
		}
	}
}