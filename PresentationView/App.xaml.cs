namespace ReferenceApplicationArchitecture.PresentationView;

using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using ReferenceApplicationArchitecture.BusinessLogic;
using ReferenceApplicationArchitecture.Data;
using ReferenceApplicationArchitecture.PresentationViewModel;

public partial class App : Application
{
    private ServiceProvider? _serviceProvider;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        var services = new ServiceCollection();
        ConfigureServices(services);
        _serviceProvider = services.BuildServiceProvider();

        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _serviceProvider?.Dispose();
        base.OnExit(e);
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<ITaskRepository, InMemoryTaskRepository>();
        services.AddSingleton<ITaskService, TaskService>();
        services.AddSingleton<TaskListViewModel>();
        services.AddSingleton<MainWindow>();
    }
}
