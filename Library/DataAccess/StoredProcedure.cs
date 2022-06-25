using System.Data;

namespace Portfolio.ETL;
public class StoredProcedure
{
    public readonly string Name;
    public readonly SqlParameter[] Parameters;

    public StoredProcedure(string name, params SqlParameter[] parameters)
    {
        Name = name;
        Parameters = parameters ?? Array.Empty<SqlParameter>();
    }

    public void ConfigureCommand(SqlCommand command)
    {
        command.CommandType = CommandType.StoredProcedure;
        if (Parameters != null && Parameters.Length > 0)
            for (int i = 0; i < Parameters.Length; i++) command.Parameters.Add(Parameters[i]);
    }
}
