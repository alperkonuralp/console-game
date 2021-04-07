using ConsoleGame.Entities;
using Dapper;
using System;
using System.Data;

namespace ConsoleGame.Db
{
	public class SqliteGameLevelTypeHandler : SqlMapper.TypeHandler<GameLevel>
	{
		public override void SetValue(IDbDataParameter parameter, GameLevel value)
		{
			parameter.Value = value.ToString();
		}

		public override GameLevel Parse(object value)
		{
			// Dapper may pass a Enum instead of a string
			return value is GameLevel g ? g : Enum.Parse<GameLevel>(value.ToString());
		}
	}
}