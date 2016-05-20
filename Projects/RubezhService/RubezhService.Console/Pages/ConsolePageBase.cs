using System;
namespace RubezhService
{
	abstract class ConsolePageBase
	{
		public abstract string Name { get; }
		public abstract void Draw(int left, int top, int width, int height);
		public virtual void OnKeyPressed(ConsoleKey key) { }

		protected void EndDraw()
		{
			Console.SetCursorPosition(0, 0);
			Console.SetCursorPosition(0, Console.WindowHeight - 1);
		}
	}
}
