namespace Portfolio.ETL;

public record ZendeskExtraction(List<ZendeskTicketModel> Tickets, Dictionary<ZendeskRecordType, IReadOnlyList<ZendeskCommonModel>> Records);

