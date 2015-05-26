using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Common;
using FiresecService.Report;
using FiresecService.Service;
using FiresecService.ViewModels;
using Infrastructure.Common;
using Infrastructure.Common.BalloonTrayTip;
using Infrastructure.Common.Windows;

namespace FiresecService
{
	public static class Bootstrapper
	{
		static Thread WindowThread = null;
		static MainViewModel MainViewModel;
		static AutoResetEvent MainViewStartedEvent = new AutoResetEvent(false);

		public static void Run()
		{
			try
			{
				//NpssqlTest();
				Environment.CurrentDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
				Logger.Trace(SystemInfo.GetString());
				var resourceService = new ResourceService();
				resourceService.AddResource(new ResourceDescription(typeof(Bootstrapper).Assembly, "DataTemplates/Dictionary.xaml"));
				resourceService.AddResource(new ResourceDescription(typeof(ApplicationService).Assembly, "Windows/DataTemplates/Dictionary.xaml"));
				WindowThread = new Thread(new ThreadStart(OnWorkThread));
				WindowThread.Name = "Main window";
				WindowThread.Priority = ThreadPriority.Highest;
				WindowThread.SetApartmentState(ApartmentState.STA);
				WindowThread.IsBackground = true;
				WindowThread.Start();
				MainViewStartedEvent.WaitOne();

				UILogger.Log("Загрузка конфигурации");
				ConfigurationCashHelper.Update();
				UILogger.Log("Создание конфигурации ГК");
				GKProcessor.Create();
				PatchManager.Patch();
				UILogger.Log("Открытие хоста");
				FiresecServiceManager.Open();
				ServerLoadHelper.SetStatus(FSServerState.Opened);
				UILogger.Log("Запуск ГК");
				GKProcessor.Start();
				UILogger.Log("Создание конфигурации СКД");
				SKDProcessor.Start();

				UILogger.Log("Запуск сервиса отчетов");
				ReportServiceManager.Run();
				UILogger.Log("Сервис отчетов запущен: " + ConnectionSettingsManager.ReportServerAddress);

				UILogger.Log("Запуск автоматизации");
				ScheduleRunner.Start();

				UILogger.Log("Готово");
				ProcedureRunner.RunOnServerRun();

				
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове Bootstrapper.Run");
				UILogger.Log("Ошибка при запуске сервера");
				BalloonHelper.ShowFromServer("Ошибка во время загрузки");
				Close();
			}
		}

		private static void OnWorkThread()
		{
			try
			{
				MainViewModel = new MainViewModel();
				ApplicationService.Run(MainViewModel, false, false);
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове Bootstrapper.OnWorkThread");

				BalloonHelper.ShowFromServer("Ошибка во время загрузки");
			}
			MainViewStartedEvent.Set();
			System.Windows.Threading.Dispatcher.Run();
		}

		public static void Close()
		{
			ServerLoadHelper.SetStatus(FSServerState.Closed);
			if (WindowThread != null)
			{
				WindowThread.Interrupt();
				WindowThread = null;
			}
			System.Environment.Exit(1);

#if DEBUG
			return;
#endif
			Process.GetCurrentProcess().Kill();
		}

		static void NpssqlTest()
		{
			using (var db = new ChinookContext())
			{
				//db.Database.ExecuteSqlCommand(string.Format("select * from {0}", "Artist"));
				var artists = from a in db.Artists
							  where a.Name.StartsWith("A")
							  orderby a.Name
							  select a;

				foreach (var artist in artists)
				{
					Console.WriteLine(artist.Name);
				}
			}
		}

		public class Artist
		{
			public Artist()
			{
				Albums = new List<Album>();
			}

			public int ArtistId { get; set; }
			public string Name { get; set; }

			public virtual ICollection<Album> Albums { get; set; }
		}

		public class Album
		{
			public int AlbumId { get; set; }
			public string Title { get; set; }

			public int ArtistId { get; set; }
			public virtual Artist Artist { get; set; }
		}

		class ChinookContext : DbContext
		{
			public DbSet<Artist> Artists { get; set; }
			public DbSet<Album> Albums { get; set; }

			protected override void OnModelCreating(DbModelBuilder modelBuilder)
			{
				// Map to the correct Chinook Database tables
				modelBuilder.Entity<Artist>().ToTable("Artist", "public");
				modelBuilder.Entity<Album>().ToTable("Album", "public");

				// Chinook Database for PostgreSQL doesn't auto-increment Ids
				modelBuilder.Conventions
					.Remove<StoreGeneratedIdentityKeyConvention>();
			}
		}
	}
}