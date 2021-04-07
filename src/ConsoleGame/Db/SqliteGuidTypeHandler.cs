using Dapper;
using System;
using System.Data;

namespace ConsoleGame.Db
{
	public class SqliteGuidTypeHandler : SqlMapper.TypeHandler<Guid>
	{
		public override void SetValue(IDbDataParameter parameter, Guid value)
		{
			parameter.Value = value.ToString();
		}

		public override Guid Parse(object value)
		{
			// Dapper may pass a Guid instead of a string
			return value is Guid g ? g : new Guid((string)value);
		}
	}
}