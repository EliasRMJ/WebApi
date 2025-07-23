using WebApi.Structs;

namespace WebApi.Constants
{
    public class Versions
    {
        private readonly static string _URL_DOWNLOAD = "https://servicenow.app.br/downloads/";
        private readonly static string _NOTE = "Esta versão adiciona uma nova coluna (bairro) no relatório de posição diária!";

        private readonly static VersionView[] numbers = [ 
                                                            new VersionView(6916, "v6.9.16", "SGS Install", 167.200, DateTime.Parse("23/07/2025"), _NOTE, _URL_DOWNLOAD)
                                                        ];

        public async static Task<VersionView?> GetInfo(int number)
        {
            var versionNumber = numbers.FirstOrDefault(n => n.Number > number);
            return await Task.FromResult(versionNumber);
        }
    }
}