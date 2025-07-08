using WebApi.Structs;

namespace WebApi.Constants
{
    public class Versions
    {
        private readonly static string _URL_DOWNLOAD = "https://servicenow.app.br/downloads/";
        private readonly static string _NOTE = "Esta versão realiza uma ajustes na integração de NFS-e para emissão de valor iss retido e correção na criação de nova NFS-e!";

        private readonly static VersionView[] numbers = [ 
                                                            new VersionView(6915, "v6.9.15", "SGS Install", 167.200, DateTime.Parse("08/07/2025"), _NOTE, _URL_DOWNLOAD)
                                                        ];

        public async static Task<VersionView?> GetInfo(int number)
        {
            var versionNumber = numbers.FirstOrDefault(n => n.Number > number);
            return await Task.FromResult(versionNumber);
        }
    }
}