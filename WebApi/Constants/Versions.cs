using WebApi.Structs;

namespace WebApi.Constants
{
    public class Versions
    {
        private readonly static string _URL_DOWNLOAD = "https://servicenow.app.br/downloads/";
        private readonly static string _NOTE = "[CORREÇÃO] Esta versão corrige os relatórios de contas à receber e pagar, além de melhorias de performance do sistema!";

        private readonly static VersionView[] numbers = [ 
                                                            new VersionView(6615, "v6.6.15", "ServiceNow-Install", 157.47, DateTime.Parse("06/12/2024"), _NOTE, _URL_DOWNLOAD),
                                                            new VersionView(6616, "v6.6.16", "ServiceNow-Install", 158.80, DateTime.Parse("15/12/2024"), _NOTE, _URL_DOWNLOAD),
                                                            new VersionView(6617, "v6.6.17", "ServiceNow-Install", 159.87, DateTime.Parse("16/12/2024"), _NOTE, _URL_DOWNLOAD)
                                                        ];

        public async static Task<VersionView?> GetInfo(int number)
        {
            var versionNumber = numbers.FirstOrDefault(n => n.Number > number);
            return await Task.FromResult(versionNumber);
        }
    }
}