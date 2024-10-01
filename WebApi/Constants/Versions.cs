namespace WebApi.Constants
{
    public class Versions
    {
        static int[] numbers = [6411, 6410];

        public async static Task<int> GetNumber(int number)
        {
            var versionNumber = numbers.FirstOrDefault(n => n > number);
            return await Task.FromResult(versionNumber);
        }
    }
}