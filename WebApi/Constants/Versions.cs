using WebApi.Structs;

namespace WebApi.Constants
{
    public class Versions
    {
        private readonly static string _URL_DOWNLOAD = "https://servicenow.app.br/downloads/";
        private readonly static string _NOTE = "Novos ajustes e melhorias no processo de baixa e controle de versão do sistema!";

        private readonly static VersionView[] numbers = [ 
                                                            new VersionView(6615, "v6.6.15", "ServiceNow-Install", 157.47, DateTime.Parse("06/12/2024"), _NOTE, _URL_DOWNLOAD)
                                                        ];

        public async static Task<VersionView?> GetInfo(int number)
        {
            var versionNumber = numbers.FirstOrDefault(n => n.Number > number);
            return await Task.FromResult(versionNumber);
        }
    }
}