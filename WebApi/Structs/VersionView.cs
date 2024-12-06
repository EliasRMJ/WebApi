namespace WebApi.Structs
{
    public record struct VersionView(int Number, string Name, string FullName, double Size, DateTime Date, string Note, string UrlDownload);
}