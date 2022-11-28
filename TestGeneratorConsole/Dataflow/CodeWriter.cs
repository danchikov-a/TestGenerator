namespace TestGeneratorConsole.Dataflow
{
    public static class CodeWriter
    {
        public static async Task Write(string path, string fileContent)
        {
            await using var writer = new StreamWriter(path);

            await writer.WriteAsync(fileContent);
        }
    }
}