using WebApi.Structs;

namespace WebApi.Constants
{
    public class Versions
    {
        private readonly static string _URL_DOWNLOAD = "https://servicenow.app.br/downloads/";
        private readonly static string _NOTE = "Esta versão cria uma validação no cadastro de cliente para alteração de empresa financeira caso o cliente tenha uma cobrança em aberto e com boleto ou NFS-e gerados e não permite excluir um contato com e-mail configurado nos documentos!";

        private readonly static VersionView[] numbers = [ 
                                                            new VersionView(6921, "v6.9.21", "SGS Install", 171.148, DateTime.Parse("07/10/2025"), _NOTE, _URL_DOWNLOAD)
                                                        ];

        public async static Task<VersionView?> GetInfo(int number)
        {
            var versionNumber = numbers.FirstOrDefault(n => n.Number > number);
            return await Task.FromResult(versionNumber);
        }
    }
}