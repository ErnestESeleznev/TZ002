namespace WebApi.Library.Models
{
    public class BaseModel
    {
        public int StatusCode { get; init; } = 200;
        public string? Message { get; init; }
    }
}
