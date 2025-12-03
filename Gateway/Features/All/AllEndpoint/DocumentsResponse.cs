using Common.Dto;

namespace Gateway.Features.All.AllEndpoint;

public class DocumentsResponse
{
    public IEnumerable<DocumentDto> Documents { get; set; } = [];
}