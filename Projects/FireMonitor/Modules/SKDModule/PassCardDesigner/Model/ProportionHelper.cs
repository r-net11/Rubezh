using System;

namespace SKDModule.PassCardDesigner.Model
{
	/// <summary>
	/// Класс, содержащий вспомогательные методы для вычисления размеров, учитывая пропорции.
	/// Пропорции вычисляются на основе максимально возможных размеров и текущих размеров.
	/// </summary>
	public static class ProportionHelper
	{
		private const double Tolerance = 0.0000001;

		/// <summary>
		/// Расчёт высоты контрола превью, на основе множителя.
		/// </summary>
		/// <example>
		/// <code>
		///		var result = CalculateProportionHeight(10, 10, 5, 5)
		/// </code>
		/// результат в <c>result</c> будет равен 10
		/// </example>
		/// <returns>Пропорциональная высота</returns>
		public static double CalculateProportionHeight(int maxWidth, int maxHeight, double currentWidth, double currentHeight)
		{
			return Math.Abs(currentHeight) < Tolerance
				? default(double)
				: currentHeight * GetRatio(maxWidth, maxHeight, currentWidth, currentHeight);
		}

		/// <summary>
		/// Расчёт ширины контрола превью, на основе множителя.
		/// </summary>
		/// /// <code>
		///		var result = CalculateProportionWidth(10, 10, 5, 5)
		/// </code>
		/// результат в <c>result</c> будет равен 10
		/// <returns>Пропорциональная ширина</returns>
		public static double CalculateProportionWidth(int maxWidth, int maxHeight, double currentWidth, double currentHeight)
		{
			return Math.Abs(currentWidth) < Tolerance
				? default(double)
				: currentWidth * GetRatio(maxWidth, maxHeight, currentWidth, currentHeight);
		}

		/// <summary>
		/// Вычисляет множитель, необходимый для перевода длины и ширины, введенной пользователем
		/// в длину и ширину контрола превью.
		/// </summary>
		/// <example>
		///		var ratio = Getratio(16, 16, 8, 8);
		/// </example>
		/// результат в <c>ratio</c> будет равен 2
		/// <returns>Множитель, используемый при конвертировании.</returns>
		public static double GetRatio(int maxWidth, int maxHeight, double currentWidth, double currentHeight)
		{
			var ratioX = maxWidth / currentWidth;
			var ratioY = maxHeight / currentHeight;

			return ratioX < ratioY ? ratioX : ratioY;
		}
	}
}
