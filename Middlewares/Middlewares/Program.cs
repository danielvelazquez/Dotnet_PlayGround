
namespace Middlewares;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.AddServiceDefaults();

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        app.MapDefaultEndpoints();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        // Encadenamiento de varios delegados usando use
        app.Use(async (context, next) =>
        {
            // Do work that doesn't write to the Response.
            
            // Nota: Si no se llama a next, se puede ocacionar un cortocircuito en la cadena de delegados
            // ya que next es el siguiente delegado en la cadena.
            await next.Invoke();
            
            // Do logging or other work that doesn't write to the Response.
        });


        app.Run(async context =>
        {
            // A simple delegate-based request handler.
            // Si se agrega otro delegado después de este, no se ejecutará.
            await context.Response.WriteAsync("Hola desde el 2do delegado.");
        });
    }
}
