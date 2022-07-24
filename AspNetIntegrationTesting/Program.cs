using AspNetIntegrationTesting.Services;

namespace AspNetIntegrationTesting
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddSingleton<IPdfService, PuppeteerPdfService>();

            builder.Services.Configure<ContentOptions>(builder.Configuration.GetSection(ContentOptions.Content));

            builder.Services.AddControllers();

            builder.WebHost.ConfigureKestrel(options => options.ListenAnyIP(5000));

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}