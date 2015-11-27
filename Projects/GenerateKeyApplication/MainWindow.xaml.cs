// Copyright (c) 2006 Damien Miller <djm@mindrot.org>
// Copyright (c) 2007 Derek Slager
//
// Permission to use, copy, modify, and distribute this software for any
// purpose with or without fee is hereby granted, provided that the above
// copyright notice and this permission notice appear in all copies.
//
// THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES
// WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES OF
// MERCHANTABILITY AND FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR
// ANY SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES
// WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN
// ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF
// OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.

using AppSoftware.LicenceEngine.Common;
using AppSoftware.LicenceEngine.KeyGenerator;
using System.Windows;



namespace GenerateKeyApplication
{
	/// <summary>BCrypt implements OpenBSD-style Blowfish password hashing
	/// using the scheme described in "A Future-Adaptable Password Scheme"
	/// by Niels Provos and David Mazieres.</summary>
	/// <remarks>
	/// <para>This password hashing system tries to thwart offline
	/// password cracking using a computationally-intensive hashing
	/// algorithm, based on Bruce Schneier's Blowfish cipher. The work
	/// factor of the algorithm is parametized, so it can be increased as
	/// computers get faster.</para>
	/// <para>To hash a password for the first time, call the
	/// <c>HashPassword</c> method with a random salt, like this:</para>
	/// <code>
	/// string hashed = BCrypt.HashPassword(plainPassword, BCrypt.GenerateSalt());
	/// </code>
	/// <para>To check whether a plaintext password matches one that has
	/// been hashed previously, use the <c>CheckPassword</c> method:</para>
	/// <code>
	/// if (BCrypt.CheckPassword(candidatePassword, storedHash)) {
	///     Console.WriteLine("It matches");
	/// } else {
	///     Console.WriteLine("It does not match");
	/// }
	/// </code>
	/// <para>The <c>GenerateSalt</c> method takes an optional parameter
	/// (logRounds) that determines the computational complexity of the
	/// hashing:</para>
	/// <code>
	/// string strongSalt = BCrypt.GenerateSalt(10);
	/// string strongerSalt = BCrypt.GenerateSalt(12);
	/// </code>
	/// <para>
	/// The amount of work increases exponentially (2**log_rounds), so
	/// each increment is twice as much work. The default log_rounds is
	/// 10, and the valid range is 4 to 31.
	/// </para>
	/// </remarks>
	///
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			Title = "A.C. Tech генератор ключей продукта";
		}

		private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
		{
			OutputBox.Text = GetProductKey(InputBox.Text);
		}

		private string GetProductKey(string userKey)
		{
			if (string.IsNullOrEmpty(userKey)) return string.Empty;

			var keyGenerator = new PkvLicenceKeyGenerator();

			// The KeyByteSet objects are the secret byte groups that are built in
			// to the licence key. On verification of the licence
			// key, we will test against a subset of these KeyByteSet objects.

			// Important: The full set of KeyByteSet objects should not be available
			// in any publicly distributed code or binaries. Nor should
			// the AppSoftware.LicenceEngine.KeyGenerator.dll binary file.

			var keyBytes = new[] {

                                     new KeyByteSet(1, 254, 122, 96),
                                     new KeyByteSet(2, 54, 124, 222),
                                     new KeyByteSet(3, 119, 142, 132),
                                     new KeyByteSet(4, 128, 122, 10),
                                     new KeyByteSet(5, 165, 15, 132),
                                     new KeyByteSet(6, 128, 175, 213),
                                     new KeyByteSet(7, 7, 244, 132 ),
                                     new KeyByteSet(8, 128, 122, 251)
                                  };

			string inputString = userKey;
			int inputInt = inputString.GetHashCode();
			// Pass a logRounds parameter to GenerateSalt to explicitly specify the
			// amount of resources required to check the password. The work factor
			// increases exponentially, so each increment is twice as much work. If
			// omitted, a default of 10 is used.
		//	string hashed = BCrypt.HashPassword(inputString, BCrypt.GenerateSalt(12));

			// Check the password.
		//	var matches = BCrypt.CheckPassword(inputString, hashed);
			return keyGenerator.MakeKey(inputInt, keyBytes);
		}
	}
}
