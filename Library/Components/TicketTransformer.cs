using Microsoft.Extensions.Logging;

namespace Portfolio.ETL;

public class TicketTransformer
{
    private readonly TelemetryClient _logger;

    public TicketTransformer(TelemetryConfiguration telemetryConfiguration)
    {
        _logger = new TelemetryClient(telemetryConfiguration);
    }

    public List<SqlTicketModel> Transform(ZendeskExtraction extract)
    {
        try
        {
            _logger.TrackTrace("Transform Started");
            List<ZendeskTicketModel> tickets = extract.Tickets;
            Dictionary<ZendeskRecordType, IReadOnlyList<ZendeskCommonModel>> records = extract.Records;
            List<SqlTicketModel> convertedTickets = new();
            for (int i = 0; i < tickets.Count; i++) convertedTickets.Add(ConvertTicket(tickets[i]));

            SqlTicketModel ConvertTicket(ZendeskTicketModel t)
            {
                return new SqlTicketModel
                (
                    id: t.id,
                    url: t.url,
                    createdAt: t.created_at,
                    updatedAt: t.updated_at,
                    ticketType: t.type,
                    subject: t.subject,
                    priority: t.priority,
                    status: t.status,
                    recipient: t.recipient,
                    requesterId: FindRecordWithId(t.requester_id, ZendeskRecordType.User),
                    submitterId: FindRecordWithId(t.submitter_id, ZendeskRecordType.User),
                    assigneeId: FindRecordWithId(t.assignee_id, ZendeskRecordType.User),
                    organizationId: FindRecordWithId(t.organization_id, ZendeskRecordType.Org),
                    groupId: FindRecordWithId(t.group_id, ZendeskRecordType.Group),
                    hasIncidents: t.has_incidents,
                    isPublic: t.is_public,
                    tags: t.tags?.ToList(),
                    customFields: FieldsToDict(t.custom_fields),
                    fields: FieldsToDict(t.fields),
                    ticketFormId: FindRecordWithId(t.ticket_form_id, ZendeskRecordType.TicketForm),
                    brandId: FindRecordWithId(t.brand_id, ZendeskRecordType.Brand)
                );
            }

            string FindRecordWithId(long? id, ZendeskRecordType type)
            {
                if (id == null) return null;
                IReadOnlyList<ZendeskCommonModel> recordSet = records[type];
                return recordSet.Where(record => record.Id == id)?.FirstOrDefault()?.Name;
            }

            Dictionary<string, string> FieldsToDict(ZendeskTicketModel.Field[] fields)
            {

                Dictionary<string, string> dict = new();

                for (int i = 0; i < fields.Length; i++) if (!TryConvertField(i)) continue;
                return dict;

                bool TryConvertField(int i)
                {
                    if (fields[i].value == null) return false;
                    string key = FindRecordWithId(fields[i].id, ZendeskRecordType.TicketField);
                    if (key is null || dict.ContainsKey(key)) return false;
                    dict.Add(key, fields[i].value!.ToString()!);
                    return true;
                }
            }

            _logger.TrackTrace("Transform Finished");
            return convertedTickets;
        }
        catch (Exception ex)
        {
            _logger.TrackException(ex);
            throw;
        }

    }
}