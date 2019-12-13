using CrossoutMarketHelp.ApplicationCore.Interfaces;
using CrossoutMarketHelp.Infrastructure.Services;
using System.Windows;
using Unity;

namespace CrossoutMarketHelp.Wpf
{
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			IUnityContainer container = new UnityContainer();
			container.RegisterType<ICrossoutDbService, CrossoutDbService>();

			var mainWindow = container.Resolve<MainWindow>();
			Application.Current.MainWindow = mainWindow;
			Application.Current.MainWindow.Show();
		}
	}
}
