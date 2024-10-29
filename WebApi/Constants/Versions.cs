using WebApi.Structs;

namespace WebApi.Constants
{
    public class Versions
    {
        private readonly static VersionView[] numbers = [ 
                                                              new VersionView(6413, "v6.4.13", 157.25, DateTime.Parse("02/10/2024"))
                                                            , new VersionView(6510, "v6.5.10", 159.69, DateTime.Parse("29/10/2024"))
                                                        ];

        public async static Task<VersionView?> GetInfo(int number)
        {
            var versionNumber = numbers.FirstOrDefault(n => n.Number > number);
            return await Task.FromResult(versionNumber);
        }
    }
}