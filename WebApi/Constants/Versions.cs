using WebApi.Structs;

namespace WebApi.Constants
{
    public class Versions
    {
        private readonly static string _URL_DOWNLOAD = "https://servicenow.app.br/downloads/";
        private readonly static string _NOTE = "Esta versão corrige alguns erros e adicionar melhorias de performance do sistema e cria um filtro por status do serviço na relatório de contas à receber!";

        private readonly static VersionView[] numbers = [ 
                                                            new VersionView(6837, "v6.8.37", "SGS Install", 160.61, DateTime.Parse("03/04/2025"), _NOTE, _URL_DOWNLOAD)
                                                        ];

        public async static Task<VersionView?> GetInfo(int number)
        {
            var versionNumber = numbers.FirstOrDefault(n => n.Number > number);
            return await Task.FromResult(versionNumber);
        }
    }
}