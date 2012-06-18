using System;
using System.IO;
using System.Linq;

namespace RecursiveUncommenter
{
	public class DirectoryRecursor
	{
		private readonly Predicate<string> _predicate;
		private readonly Action<string> _action;

		public DirectoryRecursor(Predicate<string> predicate, Action<string> action)
		{
			_predicate = predicate;
			_action = action;
		}

		public void Recurse(string directory)
		{
			if (Directory.Exists(directory))
			{
				foreach (var file in Directory.GetFiles(directory).Where(file => _predicate(file)))
				{
					_action(file);
				}

				foreach (var dir in Directory.GetDirectories(directory))
				{
					Recurse(dir);
				}
			}
			else
			{
				throw new DirectoryNotFoundException("Directory " + directory + " does not exist.");
			}
		}
	}
}