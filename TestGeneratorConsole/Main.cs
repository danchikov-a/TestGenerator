using TestGeneratorConsole.Dataflow;

namespace TestGeneratorConsole
{
    public static class TestGeneratorMain
    {
        const string OUTPUT_DIRECTORY = "E:\\tests";
        const int AMOUNT_OF_READING_FILES = 20;
        const int AMOUNT_OF_GENERATED_FILES = 20;
        const int AMOUNT_OF_WRITING_FILES = 20;

        public static async Task Main()
        {
            var inputFiles = new List<string>
            {
                "C:\\Users\\USER\\RiderProjects\\XmlHandler\\XmlHandler\\Class1.cs"
            };

            var generator = new DataflowService(
                AMOUNT_OF_READING_FILES,
                AMOUNT_OF_GENERATED_FILES,
                AMOUNT_OF_WRITING_FILES,
                OUTPUT_DIRECTORY
            );
            
            await generator.Generate(inputFiles);
        }
    }
}