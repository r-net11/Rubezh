using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using SKDDriver.DataAccess;

namespace SKDDriver.Translators
{
	public class LicenseInfoTranslator
	{
		private readonly SKDDataContext _context;

		public LicenseInfoTranslator(SKDDatabaseService databaseService)
		{
			_context = databaseService.Context;
		}

		//public bool SetKey(byte[] key, byte[] iv)
		//{
		//	var licenseInfo = new LicenseInfo
		//	{
		//		UID = Guid.NewGuid(),
		//		Key = Convert.ToBase64String(key),
		//		Vector = Convert.ToBase64String(iv)
		//	};

		//	if (_context.LicenseInfos.Any())
		//		DeleteAll();

		//	_context.LicenseInfos.InsertOnSubmit(licenseInfo);
		//	_context.SubmitChanges();

		//	return true;
		//}

		//private void DeleteAll()
		//{
		//	foreach (var licInfo in _context.LicenseInfos)
		//		_context.LicenseInfos.DeleteOnSubmit(licInfo);
		//}

		//public KeyValuePair<byte[], byte[]> GetKey()
		//{
		//	var record = _context.LicenseInfos.FirstOrDefault();

		//	return record == null
		//		? new KeyValuePair<byte[], byte[]>()
		//		: new KeyValuePair<byte[], byte[]>(Convert.FromBase64String(record.Key), Convert.FromBase64String(record.Vector));
		//}


	}
}
