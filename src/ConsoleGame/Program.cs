using ConsoleGame.Db;
using Microsoft.Extensions.DependencyInjection;

namespace ConsoleGame
{
	class Program
	{
		static void Main(string[] args)
		{
			//var gamer = new Gamer();
			//gamer.Start();

			ServiceProvider serviceProvider = CreateServiceProvider();

			var gamer = serviceProvider.GetService<IGamer>();

			gamer.Start(args);
		}

		private static ServiceProvider CreateServiceProvider()
		{
			IServiceCollection serviceCollection = new ServiceCollection();

			serviceCollection.AddSingleton<IGamer, Gamer>();
			serviceCollection.AddSingleton<IDbManager, DbManager>();

			var serviceProvider = serviceCollection.BuildServiceProvider();
			return serviceProvider;
		}

	}
}
