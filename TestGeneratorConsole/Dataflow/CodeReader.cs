namespace TestGeneratorConsole.Dataflow
{
    public static class CodeReader
    {
        public static async Task<string> Read(string fileName)
        {
            using var reader = File.OpenText(fileName);

            return await reader.ReadToEndAsync();
        }
    }
}