
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
		static string _address;
		static void Main(string[] args)
		{
			_address = args.Length == 0 ? "localhost:1050" : args[0];
			_testService = new TestService(_address);

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
					case ConsoleKey.NumPad0:
					case ConsoleKey.D0:
						InvokeOperationResult();
						break;
					case ConsoleKey.NumPad1:
					case ConsoleKey.D1:
						InvokeAction(_testService.Void, "Void");
						break;
					case ConsoleKey.NumPad2:
					case ConsoleKey.D2:
						InvokeAction(_testService.VoidOneWay, "VoidOneWay");
						break;
					case ConsoleKey.NumPad3:
					case ConsoleKey.D3:
						InvokeRandomInt();
						break;
					case ConsoleKey.NumPad4:
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
					case ConsoleKey.NumPad5:
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
					case ConsoleKey.NumPad6:
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
				case ConsoleKey.NumPad9:
				case ConsoleKey.D9:
					for (int i = 0; i < 3; i++)
						new Thread(() =>
						{
								InvokeAction(_testService.Void, "Void#" + i);
						}).Start();
					_isCanceled = false;
					break;
				}
			}
		}

		static void UpdateTitle()
		{
			Console.Title = string.Format("Address: {0}, ClientId: {1}, Period: {2}", _address, _testService.ClientId, _period);
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

		static void InvokeOperationResult()
		{
			var stopWatch = new Stopwatch();
			stopWatch.Start();

			var result = _testService.OperationResult();

			stopWatch.Stop();
			Console.WriteLine("OperationResult = {0}: {1} ms", result.Result, stopWatch.ElapsedMilliseconds);
			_isCanceled = false;
		}

		static void Void()
		{

		}

	}
}
