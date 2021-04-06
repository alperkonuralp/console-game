using ConsoleGame.Entities;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;

namespace ConsoleGame.Db
{
	public interface IDbManager
	{
		void CheckDbFile();
		SqliteConnection GetConnection();
		GameConfig GetGameConfig();
		List<PlayHistory> ListOfLastNItem(Guid userId, int n);
		void SetGameConfig(GameConfig gameConfig);
		void SetPlayHistory(PlayHistory history);
	}
}