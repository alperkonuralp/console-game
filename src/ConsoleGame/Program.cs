using AutoMapper;
using ConsoleGame.Db;
using ConsoleGame.Entities;
using ConsoleGame.Events;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rebus.Config;
using Rebus.Routing.TypeBased;
using Rebus.ServiceProvider;
using Rebus.Transport.InMem;
using Serilog;
using Serilog.Formatting.Compact;
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
			SetupLogging();

			var configuration = new ConfigurationBuilder()
																					.AddEnvironmentVariables()
																					.AddJsonFile("appsettings.json", true)
																					.Build();

			ServiceProvider serviceProvider = CreateServiceProvider(configuration);

			var logger = serviceProvider.GetService<ILogger<Program>>();

			logger.LogDebug("Console Game is started.");

			var gamer = serviceProvider.GetService<IGamer>();

			gamer.Start(args);

			logger.LogDebug("Console Game is finishing.");

			Log.CloseAndFlush();
		}

		private static void SetupLogging()
		{
			Log.Logger = new LoggerConfiguration()
									.MinimumLevel.Debug()
									.Enrich.WithAssemblyInformationalVersion()
									.Enrich.WithAssemblyName()
									.Enrich.WithAssemblyVersion()
									.Enrich.WithEnvironmentUserName()
									.Enrich.WithMachineName()
									.Enrich.WithMemoryUsage()
									//.Enrich.WithRebusCorrelationId()
									.Enrich.WithThreadId()
									.Enrich.WithThreadName()
									.WriteTo.File("logs/consoleapp.log", rollingInterval: RollingInterval.Day)
									.WriteTo.File(new CompactJsonFormatter(), "logs/consoleapp.pjson", rollingInterval: RollingInterval.Day)
									.WriteTo.Debug()
									.CreateLogger();
		}

		private static ServiceProvider CreateServiceProvider(IConfigurationRoot configuration)
		{
			IServiceCollection serviceCollection = new ServiceCollection();
			serviceCollection.AddSingleton<IConfiguration>(configuration);

			serviceCollection.AddLogging(configure => configure.AddSerilog(Log.Logger));

			serviceCollection.AddAutoMapper(typeof(Program).Assembly);

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

			serviceCollection.AddSingleton<SqlMapper.TypeHandler<GameStatus>, SqliteGameStatusTypeHandler>();
			serviceCollection.AddSingleton<SqlMapper.ITypeHandler, SqliteGameStatusTypeHandler>();

			serviceCollection.AutoRegisterHandlersFromAssemblyOf<Program>();

			var memoryNetwork = new InMemNetwork();

			serviceCollection.AddRebus(configure => configure
			 .Logging(l => l.Serilog())
			 .Transport(t => t.UseInMemoryTransport(memoryNetwork, "Messages"))
			 .Routing(r => r.TypeBased().MapAssemblyNamespaceOfDerivedFrom<GameStarting, IEvent>("Messages"))
			 .Options(o => o.SetBusName("ConsoleGame")));

			var serviceProvider = serviceCollection.BuildServiceProvider();
			serviceProvider.UseRebus();
			var auconf = serviceProvider.GetService<IMapper>();
			auconf.ConfigurationProvider.AssertConfigurationIsValid();
			return serviceProvider;
		}
	}
}