using ConsoleGame.Db;
using ConsoleGame.Entities;
using ConsoleTables;
using ConsoleTools;
using GameAnalyticsSDK.Net;
using System;
using System.Security.Cryptography;

namespace ConsoleGame
{
	public class Gamer : IGamer
	{
		private readonly IDbManager _dbManager;
		private readonly GameConfig _gameConfig;
		private string[] _args;

		public Gamer(IDbManager dbManager)
		{
			_dbManager = dbManager;
			_gameConfig = _dbManager.GetGameConfig();
		}

		public void Start(string[] args)
		{
			_args = args;
			CheckSetup();
			SetupGameAnalytics();
			WriteScore();
			WriteMenu();
		}

		private void SetupGameAnalytics()
		{
			// GameAnalytics.SetEnabledInfoLog(true)
			// GameAnalytics.SetEnabledVerboseLog(true)

			GameAnalytics.ConfigureBuild("0.10");
			GameAnalytics.ConfigureUserId($"ConsoleGame_{_gameConfig.UserId}");

			GameAnalytics.ConfigureAvailableResourceCurrencies("gems", "gold");
			GameAnalytics.ConfigureAvailableResourceItemTypes("boost", "lives");

			GameAnalytics.ConfigureAvailableCustomDimensions01("ninja", "samurai");
			GameAnalytics.ConfigureAvailableCustomDimensions02("whale", "dolpin");
			GameAnalytics.ConfigureAvailableCustomDimensions03("horde", "alliance");

			GameAnalytics.Initialize("0561632006fdb5b79e1d0c05a271ce64", "49c2dd90ef213b9ce78dcb9311f97bd9b7c51bd5");
		}

		private void WriteMenu()
		{
			var menu = new ConsoleMenu(_args, level: 0)
				.Add("Play Game", PlayGame)
				.Add("Game History", WriteHistory)
				//.Add("Two", () => SomeAction("Two"))
				//.Add("Three", () => SomeAction("Three"))
				//.Add("Change me", (thisMenu) => thisMenu.CurrentItem.Name = "I am changed!")
				//.Add("Close", ConsoleMenu.Close)
				//.Add("Action then Close", (thisMenu) => { SomeAction("Close"); thisMenu.CloseMenu(); })
				.Add("Exit", () => Environment.Exit(0))
				.Configure(config =>
				{
					config.Selector = "--> ";
					config.EnableFilter = true;
					config.Title = "The Console Game";
					config.EnableWriteTitle = true;
					config.EnableBreadcrumb = false;
					config.WriteHeaderAction = WriteScore;
				});

			menu.Show();
		}

		private static int Next(int min, int max)
		{
			return RandomNumberGenerator.GetInt32(min, max);
		}

		private void PlayGame()
		{
			GameAnalytics.AddProgressionEvent(EGAProgressionStatus.Start, "ConsoleGame");

			PlayHistory history = new PlayHistory()
			{
				UserId = _gameConfig.UserId,
				StartTime = DateTimeOffset.UtcNow.DateTime,
			};

			Console.Clear();
			Console.WriteLine("The Console Game");
			Console.WriteLine(new string('-', 30));

			var number = Next(1, 1001);
			int shoot = 0;

			int i;
			for (i = 1; i <= 10; i++)
			{
				do
				{
					Console.Write($"{i}. Shoot : ");
					var a = Console.ReadLine();
					if (int.TryParse(a, out shoot))
					{
						break;
					}
					shoot = 0;
				} while (shoot == 0);

				if (shoot == number)
				{
					Console.WriteLine("You win");
					break;
				}
				else if (shoot < number)
				{
					Console.WriteLine("Make the number bigger.");
				}
				else
				{
					Console.WriteLine("Make the number smaller.");
				}
			}
			history.FinishTime = DateTimeOffset.UtcNow.DateTime;
			_gameConfig.NumberOfPlays++;
			if (i == 11)
			{
				Console.WriteLine("You Lost!...");
				history.IsTheWinner = false;
				GameAnalytics.AddProgressionEvent(EGAProgressionStatus.Complete, "ConsoleGame", 0);
			}
			else
			{
				history.IsTheWinner = true;
				_gameConfig.NumberOfWins++;
				GameAnalytics.AddProgressionEvent(EGAProgressionStatus.Complete, "ConsoleGame", (11 - i) * 100);
			}
			_dbManager.SetGameConfig(_gameConfig);
			_dbManager.SetPlayHistory(history);

			Console.WriteLine("Please enter to the continue.");
			Console.ReadLine();
		}

		private void WriteScore()
		{
			Console.WriteLine($"Score : {_gameConfig.NumberOfWins} / {_gameConfig.NumberOfPlays}");
		}

		private void WriteHistory()
		{
			Console.Clear();
			Console.WriteLine("Game History");
			Console.WriteLine(new string('-', 30));

			var history = _dbManager.ListOfLastNItem(_gameConfig.UserId, 10);
			var table = new ConsoleTable("Date / Time", "Duration", "Status");
			table.Options.EnableCount = false;

			foreach (var item in history)
			{
				table.AddRow(item.FinishTime, item.FinishTime - item.StartTime, item.IsTheWinner ? "Won" : "Lost");
			}

			table.Write(Format.MarkDown);

			Console.WriteLine("Please enter to the continue.");
			Console.ReadLine();
		}

		private void CheckSetup()
		{
			if (string.IsNullOrWhiteSpace(_gameConfig.UserName))
			{
				GetUserName();
			}
		}

		private void GetUserName()
		{
			Console.WriteLine("Please Enter User Name : ");
			var userName = Console.ReadLine();
			_gameConfig.UserName = userName;
			_dbManager.SetGameConfig(_gameConfig);
			Console.Clear();
		}
	}
}