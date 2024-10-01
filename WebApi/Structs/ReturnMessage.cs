namespace WebApi.Structs
{
    public record struct ReturnMessage
    {
        public string Id { get; set; }
        public string Message { get; set; }
    }
}