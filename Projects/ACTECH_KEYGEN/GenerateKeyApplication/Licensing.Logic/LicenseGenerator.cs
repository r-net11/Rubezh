using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using License.Model.Entities;

namespace Licensing.Generator
{
	public class LicenseGenerator
	{
		public string GenerateLicenseBASE64String(ILicenseEntity lic, byte[] array, string certFilePwd)
		{
			if (lic == null) return string.Empty;

			//Serialize license object into XML
			var licenseObject = new XmlDocument();
			using (var writer = new StringWriter())
			{
				var serializer = new XmlSerializer(lic.GetType(), new[] { lic.GetType() });

				serializer.Serialize(writer, lic);

				licenseObject.LoadXml(writer.ToString());
			}

			//Get RSA key from certificate
			var cert = new X509Certificate2(array, certFilePwd);

			var rsaKey = (RSACryptoServiceProvider)cert.PrivateKey;

			//Sign the XML
			SignXML(licenseObject, rsaKey);

			//Convert the signed XML into BASE64 string
			return Convert.ToBase64String(Encoding.UTF8.GetBytes(licenseObject.OuterXml));
		}

		// Sign an XML file.
		// This document cannot be verified unless the verifying
		// code has the key with which it was signed.
		private static void SignXML(XmlDocument xmlDoc, AsymmetricAlgorithm key)
		{
			if (xmlDoc == null)
				throw new ArgumentException("xmlDoc");
			if (key == null)
				throw new ArgumentException("Key");

			// Create a SignedXml object.
			var signedXml = new SignedXml(xmlDoc) {SigningKey = key};

			// Add the key to the SignedXml document.

			// Create a reference to be signed.
			var reference = new Reference {Uri = string.Empty};

			// Add an enveloped transformation to the reference.
			var env = new XmlDsigEnvelopedSignatureTransform();
			reference.AddTransform(env);

			// Add the reference to the SignedXml object.
			signedXml.AddReference(reference);

			// Compute the signature.
			signedXml.ComputeSignature();

			// Get the XML representation of the signature and save
			// it to an XmlElement object.
			var xmlDigitalSignature = signedXml.GetXml();

			// Append the element to the XML document.
			if (xmlDoc.DocumentElement != null)
				xmlDoc.DocumentElement.AppendChild(xmlDoc.ImportNode(xmlDigitalSignature, true));
		}
	}
}
