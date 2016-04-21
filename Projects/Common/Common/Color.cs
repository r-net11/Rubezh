using System.Text;

namespace Common
{
	/// <summary>Describes a color in terms of alpha, red, green, and blue channels. </summary>
	public struct Color
	{
		public byte A { get; set; }
		public byte R { get; set; }
		public byte G { get; set; }
		public byte B { get; set; }

		internal static Color FromUInt32(uint argb)
		{
			Color color = default(Color);
			color.A = (byte)((argb & 4278190080u) >> 24);
			color.R = (byte)((argb & 16711680u) >> 16);
			color.G = (byte)((argb & 65280u) >> 8);
			color.B = (byte)(argb & 255u);

			return color;
		}

		public static Color FromArgb(byte a, byte r, byte g, byte b)
		{
			Color color = default(Color);
			color.A = a;
			color.R = r;
			color.G = g;
			color.B = b;
			return color;
		}

		public static Color FromRgb(byte r, byte g, byte b)
		{
			return Color.FromArgb(255, r, g, b);
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("#{0:X2}", this.A);
			stringBuilder.AppendFormat("{0:X2}", this.R);
			stringBuilder.AppendFormat("{0:X2}", this.G);
			stringBuilder.AppendFormat("{0:X2}", this.B);
			return stringBuilder.ToString();
		}

		public static bool operator ==(Color color1, Color color2)
		{
			return color1.A == color2.A && color1.R == color2.R && color1.G == color2.G && color1.B == color2.B;
		}

		public static bool operator !=(Color color1, Color color2)
		{
			return !(color1 == color2);
		}

		public override int GetHashCode()
		{
			return A * 256 ^ 3 + R * 256 ^ 2 + G * 256 + B;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			if (obj.GetType() != typeof(Color))
				return false;
			Color tmp = (Color)obj;
			return tmp.A == A && tmp.R == R && tmp.G == G && tmp.B == B;
		}
	}
}