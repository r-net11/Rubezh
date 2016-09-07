using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading.Tasks;
using Common;
using Localization.StrazhService.WS.Errors;

namespace StrazhService.WS
{
	public partial class StrazhWindowsService : ServiceBase
	{
		private ServiceStarter _serviceStarter;

		public StrazhWindowsService()
		{
			InitializeComponent();
			CanStop = true;
		}

		protected override void OnStart(string[] args)
		{
			try
			{
				// Подписываемся на все неотловленные исключения, включая те, что были сгенерированы не в основном потоке
				AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

				Logger.Info("Запускаем службу 'StrazhService'");
				Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
				Logger.Info(string.Format("Текущий рабочий каталог службы '{0}'", Environment.CurrentDirectory));
				DoStart();
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове StrazhWindowsService.OnStart");
				DoStop();
				throw;
			}
		}

		/// <summary>
		/// Обработать общую ошибку
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs args)
		{
			Exception exception = null;
			var o = args.ExceptionObject as Exception;
			if (o != null)
				exception = o;

			if (exception == null)
				exception = new Exception(CommonErrors.UnhandledException_Error);

			var errorMessage =
				String.Format(CommonErrors.MainThread_Error
							  + Environment.NewLine + "Exception.Message: {0}"
							  + Environment.NewLine + "Exception.Source: {1}"
							  + Environment.NewLine + "Exception.StackTrace: {2}",
					exception.Message, exception.Source, exception.StackTrace);

			errorMessage += exception.InnerExceptionToString();

			// Пишем сообщение в лог
			try
			{
				Logger.Error(errorMessage);
			}
			catch
			{

			}

			Trace.WriteLine(string.Format("{0}\t{1}", DateTime.Now, errorMessage));
		}

		protected override void OnStop()
		{
			Logger.Info("Останавливаем службу 'StrazhService'");
			DoStop();
		}

		public void DoStart()
		{
			_serviceStarter = new ServiceStarter();
			Task.Factory.StartNew(_serviceStarter.Start);
		}

		public void DoStop()
		{
			_serviceStarter.Stop();
		}
	}
}
