using Application.Queries;

namespace Application.DependencyInjection;

public class QueriesModule : Module
{
    private readonly IServiceProvider _serviceProvider;

    public QueriesModule(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override void Load(ContainerBuilder builder)
    {
        builder.Register(c => new FolderQueries(_serviceProvider)).As<IFolderQueries>().InstancePerLifetimeScope();
        
        
    }
}
