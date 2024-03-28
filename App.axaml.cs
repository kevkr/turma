using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using turma.ViewModels;
using turma.Views;

namespace turma
{
    public partial class App : Application
    {
        public static MainWindow MainWindow;
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                   
                };

                App.MainWindow = desktop.MainWindow as MainWindow;
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
