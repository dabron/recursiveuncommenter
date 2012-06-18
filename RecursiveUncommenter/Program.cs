using System;
using System.IO;

namespace RecursiveUncommenter
{
	internal class Program
	{
		private readonly Uncommenter _uncommenter = new Uncommenter();

		private static void Main(string[] args)
		{
			new Program().Run(args);
		}

		private void Run(string[] args)
		{
			if (args.Length == 1)
			{
				Run(args[0]);
			}
			else
			{
				Console.WriteLine("\r\nThis program recursively uncomments *.cs files found in the specified path.\r\nCorrect usage: uncomment <path>\r\n");
			}
		}

		private void Run(string directory)
		{
			var recursor = new DirectoryRecursor(ShouldUncommentFile, UncommentFile);

			try
			{
				recursor.Recurse(directory);
			}
			catch (DirectoryNotFoundException ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

		private static bool ShouldUncommentFile(string file)
		{
			return Path.GetExtension(file) == ".cs";
		}

		private void UncommentFile(string file)
		{
			_uncommenter.Uncomment(file);
			Console.WriteLine("Uncommented: " + file);
		}
	}
}