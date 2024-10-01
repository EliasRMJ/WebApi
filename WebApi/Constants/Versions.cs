using WebApi.Structs;

namespace WebApi.Constants
{
    public class Versions
    {
        private readonly static VersionView[] numbers = [ new VersionView(6411, "v6.4.11", 157.25, DateTime.Parse("01/10/2024"))
                                                        , new VersionView(6410, "v6.4.10", 156.15, DateTime.Parse("14/09/2024"))];

        public async static Task<VersionView?> GetInfo(int number)
        {
            var versionNumber = numbers.FirstOrDefault(n => n.Number > number);
            return await Task.FromResult(versionNumber);
        }
    }
}