using WebApi.Structs;

namespace WebApi.Constants
{
    public class Versions
    {
        private readonly static string _URL_DOWNLOAD = "https://servicenow.app.br/downloads/";
        private readonly static string _NOTE = "Esta versão corrige alguns erros e inclui um novo relatório de numeros de atendimentos, além de melhorias de performance do sistema!";

        private readonly static VersionView[] numbers = [ 
                                                            new VersionView(6717, "v6.7.17", "SGS Install", 159.25, DateTime.Parse("26/01/2025"), _NOTE, _URL_DOWNLOAD)
                                                        ];

        public async static Task<VersionView?> GetInfo(int number)
        {
            var versionNumber = numbers.FirstOrDefault(n => n.Number > number);
            return await Task.FromResult(versionNumber);
        }
    }
}