namespace Quantify.Core.Data;

public class ModuleDTO
{
    public required string ModuleName {get;set;}
    public string? Description {get;set;}

    public List<TopicDTO> Topics {get; set;} = new List<TopicDTO>();
}