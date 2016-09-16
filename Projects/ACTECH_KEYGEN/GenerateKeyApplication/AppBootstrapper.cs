using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;
using Caliburn.Micro;
using GenerateKeyApplication.Controls.Interfaces;
using GenerateKeyApplication.Controls.ViewModels;

namespace GenerateKeyApplication
{

    public class AppBootstrapper : BootstrapperBase
	{
		CompositionContainer _container;

        public AppBootstrapper()
		{
            Initialize();
        }

        protected override void Configure()
		{
			_container = new CompositionContainer();

			var catalog = new AggregateCatalog(AssemblySource.Instance.Select(x => new AssemblyCatalog(x)));
			catalog.Catalogs.Add(new AssemblyCatalog((typeof(ShellViewModel)).Assembly));
			_container = new CompositionContainer(catalog);

			var batch = new CompositionBatch();

			batch.AddExportedValue<IWindowManager>(new WindowManager());
			batch.AddExportedValue<IEventAggregator>(new EventAggregator());
			batch.AddExportedValue(catalog);
			batch.AddExportedValue(_container);

			_container.Compose(batch);
        }

        protected override object GetInstance(Type service, string key)
		{
			var contract = string.IsNullOrEmpty(key) ? AttributedModelServices.GetContractName(service) : key;
			var exports = _container.GetExportedValues<object>(contract);

			if (exports.Any())
				return exports.First();

			throw new Exception(string.Format("Could not locate any instances of contract {0}.", contract));
        }

        protected override void BuildUp(object instance)
		{
			_container.SatisfyImportsOnce(instance);
        }

        protected override void OnStartup(object sender, System.Windows.StartupEventArgs e)
		{
			DisplayRootViewFor<IShell>();
        }

		protected override IEnumerable<Assembly> SelectAssemblies()
		{
			var assemblies = base.SelectAssemblies().ToList();

			assemblies.Add(typeof(IShell).Assembly);

			return assemblies;
		}
    }
}