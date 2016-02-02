
using System;
using System.Diagnostics;
using System.Threading;
namespace Client
{
	class Program
	{
		static bool _isCanceled;
		static int _period = 1024;
		static TestService _testService;
		static void Main(string[] args)
		{
			var address = args.Length == 0 ? "localhost:1050" : args [0];
			_testService = new TestService (address);

			UpdateTitle();

			while (true)
			{
				var key = Console.ReadKey();
				switch (key.Key)
				{
					case ConsoleKey.UpArrow:
						if (_period < 10000)
						{
							_period *= 2;
							UpdateTitle();
						}
						break;
					case ConsoleKey.DownArrow:
						if (_period > 1)
						{
							_period /= 2;
							UpdateTitle();
						}
						break;
					case ConsoleKey.Escape:
						_isCanceled = true;
						break;
				case ConsoleKey.D1:
						InvokeAction(_testService.Void, "Void");
						break;
				case ConsoleKey.D2:
						InvokeAction(_testService.VoidOneWay, "VoidOneWay");
						break;
				case ConsoleKey.D3:
						InvokeRandomInt();
						break;
				case ConsoleKey.D4:
						new Thread(() =>
							{
								while (!_isCanceled)
								{
									InvokeAction(_testService.Void, "Void");
									Thread.Sleep(_period);
								}
								_isCanceled = false;
							}).Start();
						break;
					case ConsoleKey.D5:
						new Thread(() =>
							{
								while (!_isCanceled)
								{
									InvokeAction(_testService.VoidOneWay, "VoidOneWay");
									Thread.Sleep(_period);
								}
								_isCanceled = false;
							}).Start();
						break;
					case ConsoleKey.D6:
						new Thread(() =>
						{
							while (!_isCanceled)
							{
								InvokeRandomInt();
								Thread.Sleep(_period);
							}
							_isCanceled = false;
						}).Start();
						break;
				}
			}
		}

		static void UpdateTitle()
		{
			Console.Title = string.Format("ClientId: {0}, Period: {1}", _testService.ClientId, _period);
		}

		static void InvokeAction(Action action, string actionName)
		{
			var stopWatch = new Stopwatch();
			stopWatch.Start();

			action();

			stopWatch.Stop();
			Console.WriteLine(" {0}: {1} ms", actionName, stopWatch.ElapsedMilliseconds);
			_isCanceled = false;
		}

		static void InvokeRandomInt()
		{
			var stopWatch = new Stopwatch();
			stopWatch.Start();

			var result = _testService.RandomInt(333);

			stopWatch.Stop();
			Console.WriteLine("RandomInt = {0}: {1} ms", result, stopWatch.ElapsedMilliseconds);
			_isCanceled = false;
		}



		static void Void()
		{

		}

	}
}
