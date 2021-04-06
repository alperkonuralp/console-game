using ConsoleGame.Db;
using ConsoleGame.Entities;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rebus.Config;
using Rebus.Routing.TypeBased;
using Rebus.ServiceProvider;
using Rebus.Transport.InMem;
using Serilog;
using System;
using System.Collections.Generic;

namespace ConsoleGame
{
	public class Program
	{
		protected Program()
		{

		}


		private static readonly IReadOnlyDictionary<GameLevel, int> _levelStandartPoints =
			new Dictionary<GameLevel, int> {
				{ GameLevel.Easy, 75 },
				{ GameLevel.Normal, 100 },
				{ GameLevel.Hard, 150 },
			};

		private static void Main(string[] args)
		{
			Log.Logger = new LoggerConfiguration()
			 .WriteTo.File("consoleapp.log")
			 .WriteTo.Debug()
			 .CreateLogger();


			var configuration = new ConfigurationBuilder()
																					.AddEnvironmentVariables()
																					.AddJsonFile("appsettings.json", true)
																					.Build();

			ServiceProvider serviceProvider = CreateServiceProvider(configuration);


			var logger = serviceProvider.GetService<ILogger<Program>>();

			logger.LogDebug("Console Game is started.");

			var gamer = serviceProvider.GetService<IGamer>();

			gamer.Start(args);
		}

		private static ServiceProvider CreateServiceProvider(IConfigurationRoot configuration)
		{
			IServiceCollection serviceCollection = new ServiceCollection();
			serviceCollection.AddSingleton<IConfiguration>(configuration);

			serviceCollection.AddLogging(configure => configure.AddSerilog());

			serviceCollection.AddSingleton<IReadOnlyDictionary<GameLevel, int>>(_levelStandartPoints);

			serviceCollection.AddSingleton<IGamer, Gamer>();
			serviceCollection.AddSingleton<IDbManager, DbManager>();
			serviceCollection.AddSingleton<IPlayGame, PlayGame>();
			serviceCollection.AddSingleton<ICommonFunctions, CommonFunctions>();
			serviceCollection.AddSingleton<GameConfig>(sp => sp.GetService<IDbManager>().GetGameConfig());

			serviceCollection.AddSingleton<SqlMapper.TypeHandler<GameLevel>, SqliteGameLevelTypeHandler>();
			serviceCollection.AddSingleton<SqlMapper.ITypeHandler, SqliteGameLevelTypeHandler>();
			
			serviceCollection.AddSingleton<SqlMapper.TypeHandler<Guid>, SqliteGuidTypeHandler>();
			serviceCollection.AddSingleton<SqlMapper.ITypeHandler, SqliteGuidTypeHandler>();

			serviceCollection.AddSingleton<SqlMapper.TypeHandler<int[]>, SqliteIntegerArrayTypeHandler>();
			serviceCollection.AddSingleton<SqlMapper.ITypeHandler, SqliteIntegerArrayTypeHandler>();


			serviceCollection.AutoRegisterHandlersFromAssemblyOf<Program>();

			var memoryNetwork = new InMemNetwork();

			serviceCollection.AddRebus(configure => configure
			 .Logging(l => l.Serilog())
			 .Transport(t => t.UseInMemoryTransport(memoryNetwork, "Messages"))
			 .Routing(r => r.TypeBased().MapAssemblyOf<Gamer>("Messages")));

			var serviceProvider = serviceCollection.BuildServiceProvider();
			serviceProvider.UseRebus();
			return serviceProvider;
		}
	}
}