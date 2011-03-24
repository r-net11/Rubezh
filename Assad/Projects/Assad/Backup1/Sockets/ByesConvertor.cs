using System;
using System.IO;

namespace Socktes
{
	/// <summary>
	/// 
	/// </summary>
	public sealed class ByesConvertor
	{
		public static string BytesToString(byte[] ByteArr)
		{
			return System.Text.Encoding.ASCII.GetString(ByteArr);
		}

		public static int BytesToInt(byte[] ByteArr)
		{
			return Convert.ToInt32(System.Text.Encoding.ASCII.GetString(ByteArr));
		}

		public static byte[] GetBytes(string s)
		{
			return System.Text.Encoding.ASCII.GetBytes(s);
		}
		public static byte[] GetBytes(int i)
		{
			return System.Text.Encoding.ASCII.GetBytes(i.ToString());
		}
		public static event EventHandler PercantCompleted;
		public static void SaveToFile(string Path, byte[] buff)
		{
			ByesConvertor.PercantCompleted+=new EventHandler(ByesConvertor_PercantCompleted);
			FileStream f = new FileStream(Path,FileMode.OpenOrCreate,FileAccess.ReadWrite);
			f.Seek(0,SeekOrigin.End);
			BinaryWriter bw = new BinaryWriter(f);
			bw.Write(buff);
			ByesConvertor.PercantCompleted(buff.Length,null);
			bw.Close();
			f.Close();

		}

		private static void ByesConvertor_PercantCompleted(object sender, EventArgs e)
		{

		}

		public static char[] BytesToCharArr(byte[] buff)
		{
			return System.Text.Encoding.ASCII.GetChars(buff);
		}
	}
}
