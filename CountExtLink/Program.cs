using HtmlAgilityPack;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CountExtLink
{
	class Program
	{
		static ILog _logger = LogManager.GetLogger("CountExtLink");
		private static IEnumerable<string> GetURLs(string fileName)
		{
			//return File.ReadAllLines(fileName);

			return new string[]
			{
				"https://codeby.net/kak-najti-ssylki-na-sajte/",
				"http://mail.ru",
				"https://onesignal.com",
				"http://html-agility-pa1ck.net/",
				"https://metanit.com/sharp/tutorial/6.2.php",
				"https://docs.microsoft.com/en-us/azure/",
				"https://docs.microsoft.com/en-us/aspnet/core/",
				"https://www.3pillarglobal.com/insights/"
			}.Distinct();
		}

		private static async Task RetriveExtLincCount(string url)
		{
			try
			{
				var htmlDoc = await _web.LoadFromWebAsync(url);
				var nodes = htmlDoc.DocumentNode.SelectNodes("//a");

                int count = nodes.Count(n => n.Attributes["href"] != null &&
					                         n.Attributes["href"].Value.IndexOf(url) < 0);

                _result.Add(new Tuple<string, int>(url, count));
				Interlocked.Increment(ref _urlsCompleteCount);
				Console.SetCursorPosition(0, 0);
				Console.Write($"Обработано {_urlsCompleteCount} из {_urlsCount}");
				
			}
			catch (Exception ex)
			{
				_logger.Error($"{url} - {ex.Message} - {(ex.InnerException?.Message)}");
			}

		}

		static HtmlWeb _web = new HtmlWeb();
		static IList<Tuple<string, int>> _result = new List<Tuple<string, int>>();
		static int _urlsCount = 0;
		static int _urlsCompleteCount = 0;

		static void Main(string[] args)
		{
			File.Delete("out.txt"); 

			var urls = GetURLs(null);
			_urlsCount = urls.Count();

            var tasks = urls.Select(url => Task.Run(() => RetriveExtLincCount(url)));
			Task.WhenAll(tasks).Wait();

			Console.WriteLine();

			foreach (var url in urls)
			{
				var r = _result.FirstOrDefault(p => p.Item1 == url);
				if (r != null)
				{
					string text = $"{r.Item1} - {r.Item2}";
					File.AppendAllText("out.txt",text + Environment.NewLine); 
					Console.WriteLine(text);
				}
			}

			Console.ReadKey();

		}
	}
}