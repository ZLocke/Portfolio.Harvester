using System.Data.SqlClient;
using System.Xml.Linq;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Portfolio.ETL;

/// <summary>
/// Executes stored procedures on a SQL Server
/// </summary>
public class SQLExecutionService
{
    private readonly TelemetryClient _logger;
    private readonly string _connectionString;

    // Consider making this and the class abstract if and having a service for each sql database. Or split that into another service.
    protected const string _settingId = "ZENDESK";

    public SQLExecutionService(TelemetryConfiguration telemetryConfiguration, IConfiguration configuration)
    {
        _logger = new TelemetryClient(telemetryConfiguration);
        _connectionString = configuration[$"{_settingId}"];
    }

    /// <summary>
    /// Executes a given stored procedure.
    /// </summary>
    /// <param name="sproc"></param>
    /// <returns>Query results of passed type</returns>
    protected SqlConnection CreateConnection => new SqlConnection(_connectionString);
    public List<T> ReadFromStoredProcedure<T>(StoredProcedure sproc)
    {
        _logger.TrackTrace($"Executing stored procedure {sproc.Name}");

        using (SqlConnection connection = CreateConnection)
        using (SqlCommand command = new SqlCommand(sproc.Name, connection))
            try
            {
                List<T> results = new();
                sproc.ConfigureCommand(command);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                DataMapper<T> mapper = new DataMapper<T>(reader);
                while (reader.Read())
                {
                    T mappedRow = mapper.GetModelFromRow();
                    results.Add(mappedRow);
                }
                _logger.TrackTrace($"Completed procedure {sproc.Name}, returning {results.Count} elements of type {typeof(T)}.");
                return results;
            }
            catch (Exception ex)
            {
                _logger.TrackException(ex);
                throw;
            }
            finally
            {
                connection.Close();
            }
    }

    /// <summary>
    /// Executes a given stored procedure.
    /// </summary>
    /// <param name="sproc"></param>
    /// <returns>Whether the execution was successful.</returns>
    public void ExecuteStoredProcedure(StoredProcedure sproc)
    {
        _logger.TrackTrace($"Executing stored procedure {sproc.Name}");
        using (SqlConnection connection = CreateConnection)
        using (SqlCommand command = new SqlCommand(sproc.Name, connection))
            try
            {
                sproc.ConfigureCommand(command);
                connection.Open();
                command.ExecuteNonQuery();
                _logger.TrackTrace($"Completed stored procedure {sproc.Name}");
            }
            catch (Exception ex)
            {
                _logger.TrackException(ex);
            }
            finally
            {
                connection?.Close();
            }
    }
}
