using Common.Dto;

namespace Gateway.Features.All.AllEndpoint;

public class InstitutionsResponse
{
    public IEnumerable<InstitutionDto> Institutions { get; set; } = [];
}