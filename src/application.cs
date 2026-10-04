using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace konrevise.Remote;

public class Application
{
	private const string PfxPassword = "tyNeProidesh"; // вставьте сюда пароль

	public WebApplication WebApp { get; private set; }

	public Application(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);
		builder.WebHost.UseUrls("https://0.0.0.0:8080");
		builder.Services.AddCors(options =>
		{
		    options.AddDefaultPolicy(policy =>
		    {
		        policy.AllowAnyOrigin()
		              .AllowAnyHeader()
		              .AllowAnyMethod();
		    });
		});

		builder.Logging.AddFilter("Default", LogLevel.Warning);
		builder.Logging.AddFilter("Microsoft", LogLevel.Warning);

		// это надо, чтобы сайт был на протоколе https
		builder.WebHost.ConfigureKestrel(serverOptions =>
		{
		    serverOptions.Listen(IPAddress.Any, 8080, listenOptions =>
		    {
		        var certificate = X509CertificateLoader.LoadPkcs12FromFile(
		            "localhost.pfx", 
		            PfxPassword,
		            keyStorageFlags: X509KeyStorageFlags.Exportable
		        );
		        
		        listenOptions.UseHttps(certificate);
		    });
		});

		WebApp = builder.Build();
	}

	public void Initialize()
	{
		WebApp.UseCors();
		WebApp.UseDefaultFiles();
		WebApp.UseStaticFiles();
	}

	public void MapHttpMethod(string pattern, string method,
		Func<HttpContext, Task<IResult>> handler)
	{
		WebApp.MapMethods(pattern, [ method ], async (HttpContext ctx) =>
		{
		    Console.WriteLine($"\n\n########## i heard {method} \"{pattern}\"");
		    var result = await handler(ctx);
		    return result;
		});
	}

	public void Run()
	{
		Console.WriteLine("run the app!");
		WebApp.Run();
	}
}
