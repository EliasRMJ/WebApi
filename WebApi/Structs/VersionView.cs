namespace WebApi.Structs
{
    public record struct VersionView(int Number, string Name, double Size, DateTime Date, string Note = "");
}