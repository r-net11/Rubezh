using FiresecAPI.Enums;
using GenerateKeyApplication.Common;
using Infrastructure.Common;
using KeyGenerator.Utils;
using License.Model.Entities;
using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;

namespace KeyGenerator
{
	public sealed class LicenseManager : ILicenseManager
	{
		private const string Key = "Wr6CZRJix4jsxIUAVfyf9UVYUaNhv0IjhaeheDlV5FI=";
		private const string IV = "X8WjoAYe/R4KwMDpEZtqjw==";
		private const string CertificateName = "ACTechCert.cer";
		private const string LicenseFileName = "LicenseDat.lic";
		private readonly LicenseFileManager _licFileManager;
		private readonly UserKeyGenerator _userKeyGenerator;
		private readonly string _pathToLicense;

		public Entities.LicenseEntity CurrentLicense { get; private set; }
		public LicenseStatus LicenseStatus { get; private set; }

		public LicenseManager()
		{
			_pathToLicense = AppDataFolderHelper.GetFile(LicenseFileName);
			_licFileManager = new LicenseFileManager();
			_userKeyGenerator = new UserKeyGenerator();
		}

		public string GetUserKey()
		{
			return _userKeyGenerator.GenerateUID();
		}

		public bool CanConnect() //TODO: Implement this method if need it
		{
			return IsValidLicense();
		}

		public bool CanLoadModule(ModuleType type)
		{
			if (!IsValidLicense()) return false;

			switch (type) //TODO: Implement Photoverification module
			{
				case ModuleType.SKD:
					return CurrentLicense.IsEnabledURV;
				case ModuleType.Video:
					return CurrentLicense.IsEnabledRVI;
				case ModuleType.Automation:
					return CurrentLicense.IsEnabledAutomation;
				default:
					return true;
			}
		}

		public bool CanAddCard(int currentCount)
		{
			return currentCount < CurrentLicense.TotalUsers || CurrentLicense.IsUnlimitedUsers;
		}

		public bool LoadLicenseFromFile(string pathToLicense)
		{
			var lic = LoadFile(pathToLicense);
			if (VerifyProductKey(lic))
			{
				_licFileManager.SaveToFile(lic, _pathToLicense, Key, IV);
				RaiseLicenseChangedEvent();
				return true;
			}

			return false;
		}

		public event Action LicenseChanged;

		private void RaiseLicenseChangedEvent()
		{
			var temp = LicenseChanged;
			if (temp != null)
				temp();
		}

		private bool VerifyProductKey(string key)
		{
			ParseLicense(key, GetCertificationContent());

			if (CurrentLicense == null || CurrentLicense.UID != GetUserKey()) return false;

			return IsValidLicense();
		}

		private static byte[] GetCertificationContent()
		{
			return Utilites.GetBytesFrom(Assembly.GetExecutingAssembly().GetManifestResourceStream(string.Format("{0}.{1}", "KeyGenerator", CertificateName)));
		}

		public bool IsValidExistingKey()
		{
			if (!File.Exists(_pathToLicense)) return false;

			var prodKey = LoadFile(_pathToLicense);
			return VerifyProductKey(prodKey);
		}

		private string LoadFile(string pathToLicense)
		{
			return _licFileManager.Load(pathToLicense, Key, IV);
		}

		private bool IsValidLicense()
		{
			return LicenseStatus == LicenseStatus.Valid;
		}

		private void ParseLicense(string key, byte[] certificationContent)
		{
			if (certificationContent == null) return;

			if (string.IsNullOrWhiteSpace(key))
			{
				LicenseStatus = LicenseStatus.Cracked;
				return;
			}

			try
			{
				//Get RSA key from certificate
				var cert = new X509Certificate2(certificationContent);
				var rsaKey = (RSACryptoServiceProvider)cert.PublicKey.Key;

				var xmlDoc = new XmlDocument {PreserveWhitespace = true};

				// Load an XML file into the XmlDocument object.
				xmlDoc.LoadXml(Encoding.UTF8.GetString(Convert.FromBase64String(key)));

				// Verify the signature of the signed XML.
				if (VerifyXml(xmlDoc, rsaKey))
				{
					var nodeList = xmlDoc.GetElementsByTagName("Signature");
					if (xmlDoc.DocumentElement != null)
						xmlDoc.DocumentElement.RemoveChild(nodeList[0]);

					var licXML = xmlDoc.OuterXml;

					//Deserialize license
					CurrentLicense = new Entities.LicenseEntity(Serializer.Deserialize<LicenseEntity>(licXML)); //TODO: can replace LicenseEntity to factory method if need it
					LicenseStatus = LicenseStatus.Valid;
				}
				else
					LicenseStatus = LicenseStatus.Invalid;
			}
			catch
			{
				LicenseStatus = LicenseStatus.Cracked;
			}
		}

		private static bool VerifyXml(XmlDocument doc, AsymmetricAlgorithm key)
		{
			if (doc == null)
				throw new ArgumentException("Document empty");
			if (key == null)
				throw new ArgumentException("Key empty");

			// Create a new SignedXml object and pass it
			// the XML document class.
			var signedXml = new SignedXml(doc);

			// Find the "Signature" node and create a new
			// XmlNodeList object.
			var nodeList = doc.GetElementsByTagName("Signature");

			// Throw an exception if no signature was found.
			if (nodeList.Count <= 0)
			{
				throw new CryptographicException("Verification failed: No Signature was found in the document.");
			}

			// This example only supports one signature for
			// the entire XML document.  Throw an exception
			// if more than one signature was found.
			if (nodeList.Count >= 2)
			{
				throw new CryptographicException("Verification failed: More that one signature was found for the document.");
			}

			// Load the first <signature> node.
			signedXml.LoadXml((XmlElement)nodeList[0]);

			// Check the signature and return the result.
			return signedXml.CheckSignature(key);
		}
	}
}
