namespace DemoSite;

public class Program
{
	public static void Main(string[] args)
		=> CreateHostBuilder(args)
			.Build()
			.Run();

	public static IHostBuilder CreateHostBuilder(string[] args) =>
		Host.CreateDefaultBuilder(args)
			.ConfigureUmbracoDefaults()
			.ConfigureWebHostDefaults(webBuilder =>
			{
				webBuilder.ConfigureAppConfiguration(config =>
				{
					var settings = config.Build();
					string connectionString = settings.GetConnectionString("AppConfig");
					if (!string.IsNullOrWhiteSpace(connectionString))
					{
						config.AddAzureAppConfiguration(o =>
						{
							o.Connect(connectionString);
							o.UseFeatureFlags(o => o.CacheExpirationInterval = TimeSpan.FromSeconds(1)); //Setting this low for demo purposes
						});
					}
				});
				webBuilder.UseStaticWebAssets();
				webBuilder.UseStartup<Startup>();
			});
}
