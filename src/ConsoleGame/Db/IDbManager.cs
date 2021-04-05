using ConsoleGame.Entities;
using Microsoft.Data.Sqlite;

namespace ConsoleGame.Db
{
	public interface IDbManager
	{
		void CheckDbFile();
		SqliteConnection GetConnection();
		GameConfig GetGameConfig();
		void SetGameConfig(GameConfig gameConfig);
		void SetPlayHistory(PlayHistory history);
	}
}