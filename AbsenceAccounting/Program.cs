namespace AbsenceAccounting
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

            string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddSingleton(connectionString);

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            app.UseRouting();
            app.MapControllers();

            app.MapGet("/", async (HttpContext context) => {
                string htmlContent = await File.ReadAllTextAsync("wwwroot/html/index.html");

                return Results.Content(htmlContent, "text/html");
            });

            app.Run();
        }
    }
}
