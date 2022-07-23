using AspNetIntegrationTesting.Services;

namespace AspNetIntegrationTesting
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddSingleton<IContentService, WebArchiveContentService>().AddHttpClient();
            builder.Services.AddTransient<IPdfService, CefPdfService>();

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