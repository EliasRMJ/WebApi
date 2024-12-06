namespace WebApi.Structs
{
    public record struct EmailStruct
    {
        public string From { get; set; }
        public string To { get; set; }
        public string ToName { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public bool UserSsl { get; set; }
    }
}