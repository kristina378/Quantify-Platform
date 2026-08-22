namespace Quantify.Core.Data;

public class TopicDTO
{
    public required string TopicName {get;set;}
    public required string PathToContent {get;set;}

    public List<TaskDTO> Tasks {get;set;} = new List<TaskDTO>();
}