namespace DomainShared.Dto.DingTalk;

public class DingTalkResult<T>
{
    public int Code { get; set; }
    public string Msg { get; set; }
    [JsonPropertyName("response")]
    public T Result { get; set; }
}
