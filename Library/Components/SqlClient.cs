using System.Data;
using System.Data.Sql;

using Microsoft.Extensions.Configuration;

namespace Portfolio.ETL;
public class SqlClient
{
    private readonly TelemetryClient _logger;
    private readonly SQLExecutionService _sql;
    private readonly string _getTicketIdsProcedure;
    private readonly string _getStaleTicketsProcedure;
    private readonly string _insertTicketsProcedure;
    private readonly string _updateTicketsProcedure;

    public SqlClient(TelemetryConfiguration telemetryConfiguration, IConfiguration config, SQLExecutionService sql)
    {
        _logger = new TelemetryClient(telemetryConfiguration);
        _sql = sql;

        _getTicketIdsProcedure = config["SP_GET_TICKETS"];
        _getStaleTicketsProcedure = config["SP_GET_STALE_TICKETS"];
        _insertTicketsProcedure = config["SP_INSERT_TICKETS"];
        _updateTicketsProcedure = config["SP_UPDATE_TICKETS"];
    }

    public List<long> GetUpToDateTickets(DateTime updatedAt)
    {
        SqlParameter updatedAtParam = new SqlParameter("@updatedAt", updatedAt.TimeOfDay);
        StoredProcedure sproc = new StoredProcedure(_getTicketIdsProcedure, updatedAtParam);
        return _sql.ReadFromStoredProcedure<long>(sproc) ?? new List<long>();
    }

    public List<long> GetStaleTickets(DateTime updatedAt, List<SqlTicketModel> tickets)
    {
        SqlParameter updatedAtParam = new SqlParameter("@updatedAt", updatedAt.TimeOfDay);
        List<long> ticketIds = tickets.Select(t => t.Id).ToList();
        SqlParameter idListParam = new SqlParameter("@idList", ConvertListToDataTable<long>("id", ticketIds));
        StoredProcedure sproc = new StoredProcedure(_getStaleTicketsProcedure, updatedAtParam, idListParam);
        return _sql.ReadFromStoredProcedure<long>(sproc) ?? new List<long>();
    }

    public void InsertTickets(List<SqlTicketModel> tickets)
    {
        SqlParameter newTicketsParam = new SqlParameter("@newTickets", CreateTicketsDataTable(tickets));
        StoredProcedure sproc = new(_insertTicketsProcedure, newTicketsParam);
        _sql.ExecuteStoredProcedure(sproc);
    }

    public void UpdateTickets(List<SqlTicketModel> tickets)
    {
        SqlParameter updatedTicketsParam = new SqlParameter("@updatedTickets", CreateTicketsDataTable(tickets));
        StoredProcedure sproc = new(_updateTicketsProcedure, updatedTicketsParam);
        _sql.ExecuteStoredProcedure(sproc);
    }

    private DataTable ConvertListToDataTable<T>(string columnName, List<T> list)
    {
        DataTable dt = new DataTable();
        dt.Columns.Add(columnName, typeof(T));
        foreach (T element in list) dt.Rows.Add(element);
        return dt;
    }

    // TODO: Use Expressions to automap this or create a datatable model
    private DataTable CreateTicketsDataTable(List<SqlTicketModel> tickets)
    {
        _logger.TrackTrace("CreateDataTable Started");
        DataTable datatable = new DataTable();
        AddTicketColumns();
        AddRows();
        return datatable;

        void AddTicketColumns()
        {
            _logger.TrackTrace("Adding Columns to Data Table");
            datatable.Columns.Add("id", typeof(long));
            datatable.Columns.Add("url", typeof(string));
            datatable.Columns.Add("created_at", typeof(DateTime));
            datatable.Columns.Add("updated_at", typeof(DateTime));
            datatable.Columns.Add("type", typeof(string));
            datatable.Columns.Add("subject", typeof(string));
            datatable.Columns.Add("priority", typeof(string));
            datatable.Columns.Add("status", typeof(string));
            datatable.Columns.Add("recipient", typeof(string));
            datatable.Columns.Add("requester", typeof(string));
            datatable.Columns.Add("submitter", typeof(string));
            datatable.Columns.Add("assignee", typeof(string));
            datatable.Columns.Add("organization", typeof(string));
            datatable.Columns.Add("group", typeof(string));
            datatable.Columns.Add("has_incidents", typeof(bool));
            datatable.Columns.Add("is_public", typeof(bool));
            datatable.Columns.Add("tags", typeof(string));
            datatable.Columns.Add("custom_fields", typeof(string));
            datatable.Columns.Add("fields", typeof(string));
            datatable.Columns.Add("ticket_form", typeof(string));
            datatable.Columns.Add("brand", typeof(string));
            foreach (DataColumn column in datatable.Columns) column.AllowDBNull = true;
        }

        void AddRows()
        {
            _logger.TrackTrace("Adding Rows to Data Table");
            for (int i = 0; i < tickets.Count; i++) AddRow(i);
        }

        void AddRow(int i)
        {
            DataRow r = datatable.NewRow();
            SqlTicketModel t = tickets[i];
            r["id"] = t.Id;
            r["url"] = t.Url;
            r["created_at"] = t.CreatedAt;
            r["updated_at"] = t.UpdatedAt;
            r["type"] = t.TicketType;
            r["subject"] = t.Subject;
            r["priority"] = t.Priority;
            r["status"] = t.Status;
            r["recipient"] = t.Recipient;
            r["requester"] = t.RequesterId;
            r["submitter"] = t.SubmitterId;
            r["assignee"] = t.AssigneeId;
            r["organization"] = t.OrganizationId;
            r["group"] = t.GroupId;
            r["has_incidents"] = t.HasIncidents;
            r["is_public"] = t.IsPublic;
            r["tags"] = t.Tags?.ToString();
            r["custom_fields"] = t.CustomFields?.ToString();
            r["fields"] = t.Fields;
            r["ticket_form"] = t.TicketFormId;
            r["brand"] = t.BrandId;
            datatable.Rows.Add(r);
        }
    }

}
