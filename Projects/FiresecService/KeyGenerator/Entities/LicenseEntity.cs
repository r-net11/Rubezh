using License.Model.Entities;
using System;
using System.ComponentModel;

namespace KeyGenerator.Entities
{
	public class LicenseEntity
	{
		public string UID { get; private set; }
		public DateTime CreateDateTime { get; private set; }

		[Description("Оперативная задача (подключение)")]
		public int OperatorConnectionsNumber { get; private set; }

		[Description("Всего пользователей")]
		public int TotalUsers { get; private set; }

		[Description("Лицензия неограниченное количество пользователей")]
		public bool IsUnlimitedUsers { get; private set; }

		[Description("Модуль \"Учет рабочего времени\"")]
		public bool IsEnabledURV { get; private set; }
		[Description("Модуль \"Фотоверификация\"")]
		public bool IsEnabledPhotoVerification { get; private set; }
		[Description("Модуль \"Интеграция видео RVi\"")]
		public bool IsEnabledRVI { get; private set; }
		[Description("Модуль \"Автоматизация\"")]
		public bool IsEnabledAutomation { get; private set; }

		[Description("Сервер A.C. Tech")]
		public bool IsEnabledServer { get; private set; }

		public LicenseEntity(ILicenseEntity entity)
		{
			if (entity == null) return;

			UID = entity.UID;
			CreateDateTime = entity.CreateDateTime;
			OperatorConnectionsNumber = entity.OperatorConnectionsNumber;
			TotalUsers = entity.TotalUsers;
			IsUnlimitedUsers = entity.IsUnlimitedUsers;
			IsEnabledURV = entity.IsEnabledURV;
			IsEnabledPhotoVerification = entity.IsEnabledPhotoVerification;
			IsEnabledRVI = entity.IsEnabledRVI;
			IsEnabledAutomation = entity.IsEnabledAutomation;
			IsEnabledServer = entity.IsEnabledServer;
		}
	}
}
