using ConsoleGame.Db;
using ConsoleGame.Entities;
using ConsoleTools;
using System;

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
			WriteScore();
			WriteMenu();
		}

		private void WriteMenu()
		{
			var menu = new ConsoleMenu(_args, level: 0)
				.Add("Play Game", PlayGame)
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

		private void PlayGame()
		{
			PlayHistory history = new PlayHistory()
			{
				UserId = _gameConfig.UserId,
				StartTime = DateTimeOffset.UtcNow.DateTime,
			};

			Console.Clear();
			Console.WriteLine("The Console Game");
			Console.WriteLine(new string('-', 30));

			var number = new Random().Next(1, 1001);
			int shoot = 0;

			for (var i = 1; i <= 10; i++)
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
			if (shoot == 0)
			{
				Console.WriteLine("You Lost!...");
				history.IsTheWinner = false;
			}
			else
			{
				history.IsTheWinner = true;
				_gameConfig.NumberOfWins++;
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
