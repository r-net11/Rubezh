using System.ComponentModel;

namespace XFiresecAPI
{
	public enum FormulaOperationType
	{
		[DescriptionAttribute("Взять бит")]
		GETBIT = 0,

		[DescriptionAttribute("Положить бит")]
		PUTBIT = 1,

		[DescriptionAttribute("Константа")]
		CONST = 2,

		[DescriptionAttribute("Взять байт")]
		GETBYTE = 3,

		[DescriptionAttribute("Положить байт")]
		PUTBYTE = 4,

		[DescriptionAttribute("Взять слово")]
		GETWORD = 5,

		[DescriptionAttribute("Положить слово")]
		PUTWORD = 6,

		[DescriptionAttribute("Сложить")]
		ADD = 8,

		[DescriptionAttribute("Вычесть")]
		SUB = 9,

		[DescriptionAttribute("Умножить")]
		MUL = 10,

		[DescriptionAttribute("Или")]
		OR = 12,

		[DescriptionAttribute("И")]
		AND = 13,

		[DescriptionAttribute("Исключающее или")]
		XOR = 14,

		[DescriptionAttribute("Инверсия")]
		COM = 15,

		[DescriptionAttribute("Отрицание")]
		NEG = 16,

		[DescriptionAttribute("Условие Равно")]
		EQ = 17,

		[DescriptionAttribute("Условие Не равно")]
		NE = 18,

		[DescriptionAttribute("Условие Больше")]
		GT = 19,

		[DescriptionAttribute("Условие Больше или равно")]
		GE = 20,

		[DescriptionAttribute("Условие Меньше")]
		LT = 21,

		[DescriptionAttribute("Условие Меньше или равно")]
		LE = 22,

		[DescriptionAttribute("Дублировать содержимое стека")]
		DUP = 30,

		[DescriptionAttribute("Конец")]
		END = 31
	}
}