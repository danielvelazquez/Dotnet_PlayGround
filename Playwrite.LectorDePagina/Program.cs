using Playwrite.LectorDePagina.Servicios;

namespace Playwrite.LectorDePagina;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.AddServiceDefaults();
        builder.Services.AddHostedService<Worker>();

        builder.Services.AddTransient<IPageReaderService, PageReaderService>();
        var host = builder.Build();
        host.Run();
    }
}