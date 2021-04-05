using Dapper;
using System;
using System.Data;

namespace ConsoleGame.Db
{
	public class SqliteGuidTypeHandler : SqlMapper.TypeHandler<Guid>
  {
    public override void SetValue(IDbDataParameter parameter, Guid guid)
    {
      parameter.Value = guid.ToString();
    }

    public override Guid Parse(object value)
    {
      // Dapper may pass a Guid instead of a string
      if (value is Guid)
        return (Guid)value;

      return new Guid((string)value);
    }
  }
}