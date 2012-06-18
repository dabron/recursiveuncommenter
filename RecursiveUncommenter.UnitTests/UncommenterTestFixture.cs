using System.IO;
using System.Text;
using NUnit.Framework;

namespace RecursiveUncommenter.UnitTests
{
	[TestFixture]
	public class UncommenterTestFixture
	{
		private Uncommenter _uncommenter;

		[SetUp]
		public void Setup()
		{
			_uncommenter = new Uncommenter();
		}

		private static StreamReader CreateReader(string str)
		{
			byte[] buffer = Encoding.ASCII.GetBytes(str);
			var stream = new MemoryStream(buffer);
			return new StreamReader(stream);
		}

		[Test]
		public void ShouldRemoveSingleLineComment()
		{
			const string test = @"Foo// this should all go";

			using (var reader = CreateReader(test))
			{
				string result = _uncommenter.Uncomment(reader);
				Assert.AreEqual("Foo\r\n", result);
			}
		}

		[Test]
		public void ShouldRemoveMultiLineComment()
		{
			const string test =
@"Foo
/*
this should all go
lots of lines
*/
Bar";

			using (var reader = CreateReader(test))
			{
				string result = _uncommenter.Uncomment(reader);
				Assert.AreEqual("Foo\r\nBar\r\n", result);
			}
		}

		[Test]
		public void ShouldRemoveInlineComment()
		{
			const string test = @"Foo/**/Bar";

			using (var reader = CreateReader(test))
			{
				string result = _uncommenter.Uncomment(reader);
				Assert.AreEqual("FooBar\r\n", result);
			}
		}

		[Test]
		public void ShouldRemoveMultipleInlineComments()
		{
			const string test = @"Foo/**//**/Bar";

			using (var reader = CreateReader(test))
			{
				string result = _uncommenter.Uncomment(reader);
				Assert.AreEqual("FooBar\r\n", result);
			}
		}

		[Test]
		public void ShouldRemovePartiallyCommentedLines()
		{
			const string test =
@"Foo/*this
should all
go*/Bar";

			using (var reader = CreateReader(test))
			{
				string result = _uncommenter.Uncomment(reader);
				Assert.AreEqual("Foo\r\nBar\r\n", result);
			}
		}

		[Test]
		public void ShouldNotRemoveQuotedSingleLineComment()
		{
			const string test = @"Foo ""\"" //"" Bar";

			using (var reader = CreateReader(test))
			{
				string result = _uncommenter.Uncomment(reader);
				Assert.AreEqual("Foo \"\\\" //\" Bar\r\n", result);
			}
		}

		[Test]
		public void ShouldNotRemoveQuotedMultiLineBeginComment()
		{
			const string test = @"Foo ""\"" /*"" Bar";

			using (var reader = CreateReader(test))
			{
				string result = _uncommenter.Uncomment(reader);
				Assert.AreEqual("Foo \"\\\" /*\" Bar\r\n", result);
			}
		}

		[Test]
		public void ShouldNotRemoveQuotedMultiLineEndComment()
		{
			const string test = @"Foo ""\"" */"" Bar";

			using (var reader = CreateReader(test))
			{
				string result = _uncommenter.Uncomment(reader);
				Assert.AreEqual("Foo \"\\\" */\" Bar\r\n", result);
			}
		}

		[Test]
		public void ShouldNotRemoveAtQuotedSingleLineComment()
		{
			const string test = @"Foo @"""""" //"" Bar";

			using (var reader = CreateReader(test))
			{
				string result = _uncommenter.Uncomment(reader);
				Assert.AreEqual("Foo @\"\"\" //\" Bar\r\n", result);
			}
		}

		[Test]
		public void ShouldNotRemoveAtQuotedMultiLineBeginComment()
		{
			const string test = @"Foo @"""""" /*"" Bar";

			using (var reader = CreateReader(test))
			{
				string result = _uncommenter.Uncomment(reader);
				Assert.AreEqual("Foo @\"\"\" /*\" Bar\r\n", result);
			}
		}

		[Test]
		public void ShouldNotRemoveAtQuotedMultiLineEndComment()
		{
			const string test = @"Foo @"""""" */"" Bar";

			using (var reader = CreateReader(test))
			{
				string result = _uncommenter.Uncomment(reader);
				Assert.AreEqual("Foo @\"\"\" */\" Bar\r\n", result);
			}
		}

		[Test]
		public void ShouldNotRemoveMultiLineAtQuotedComment()
		{
			const string test =
@"Foo @""Foo
// this should stay
Bar"" Bar";

			using (var reader = CreateReader(test))
			{
				string result = _uncommenter.Uncomment(reader);
				Assert.AreEqual("Foo @\"Foo\r\n// this should stay\r\nBar\" Bar\r\n", result);
			}
		}

		[Test]
		public void ShouldIgnoreCommentedMultiLineComment()
		{
			const string test =
@"Foo// /*
Bar";

			using (var reader = CreateReader(test))
			{
				string result = _uncommenter.Uncomment(reader);
				Assert.AreEqual("Foo\r\nBar\r\n", result);
			}
		}

		[Test]
		public void ShouldRemoveSingleLineCommentWithExtraStar()
		{
			const string test = @"Foo/***/Bar";

			using (var reader = CreateReader(test))
			{
				string result = _uncommenter.Uncomment(reader);
				Assert.AreEqual("FooBar\r\n", result);
			}
		}

		[Test]
		public void ShouldRemoveMultiLineCommentWithExtraStar()
		{
			const string test =
@"Foo/*
*
*/Bar";

			using (var reader = CreateReader(test))
			{
				string result = _uncommenter.Uncomment(reader);
				Assert.AreEqual("Foo\r\nBar\r\n", result);
			}
		}

		[Test]
		public void ShouldRemoveMultiLineCommentWithExtraStarAndSlash()
		{
			const string test =
@"Foo/*
* /
*/Bar";

			using (var reader = CreateReader(test))
			{
				string result = _uncommenter.Uncomment(reader);
				Assert.AreEqual("Foo\r\nBar\r\n", result);
			}
		}

		[Test]
		public void ShouldIgnoreBlankLine()
		{
			const string test =
@"Foo

Bar";

			using (var reader = CreateReader(test))
			{
				string result = _uncommenter.Uncomment(reader);
				Assert.AreEqual("Foo\r\n\r\nBar\r\n", result);
			}
		}
	}
}