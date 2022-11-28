using System.Threading.Tasks.Dataflow;
using TestGeneratorLibrary.Model;
using TestGeneratorLibrary.Service;

namespace TestGeneratorConsole.Dataflow
{
    public class DataflowService
    {
        private readonly TestGeneratorService _testsGenerator;
        private TransformBlock<string, string> _readerBlock;
        private TransformManyBlock<string, FileInformation> _generatorBlock;
        private ActionBlock<FileInformation> _writerBlock;

        private readonly DataflowLinkOptions _linkOptions = new()
        {
            PropagateCompletion = true
        };

        public DataflowService(int readTaskAmount, 
            int generateTaskAmount, 
            int writeTaskAmount,
            string savePath)
        {
            _testsGenerator = new TestGeneratorService(new NamespaceIntegrationService(new ClassGenerationService()));

            ConfigureBlocks(readTaskAmount, generateTaskAmount, writeTaskAmount, savePath);
        }

        private void ConfigureBlocks(int readTaskRestriction, int generateTaskRestriction, int writeTaskRestriction, string savePath)
        {
            _readerBlock = new TransformBlock<string, string>(
                async fileName => await CodeReader.Read(fileName),
                new ExecutionDataflowBlockOptions {MaxDegreeOfParallelism = readTaskRestriction}
            );
            
            _generatorBlock = new TransformManyBlock<string, FileInformation>(
                source => _testsGenerator.Generate(source),
                new ExecutionDataflowBlockOptions {MaxDegreeOfParallelism = generateTaskRestriction}
            );
            
            _writerBlock = new ActionBlock<FileInformation>(
                async fileInfo =>
                {
                    await CodeWriter.Write($"{savePath}\\{fileInfo.FileName}.cs", fileInfo.FileContent);
                },
                new ExecutionDataflowBlockOptions {MaxDegreeOfParallelism = writeTaskRestriction}
            );
            
            _readerBlock.LinkTo(_generatorBlock, _linkOptions);
            _generatorBlock.LinkTo(_writerBlock, _linkOptions);
        }

        public async Task Generate(List<string> fileNames)
        {
            foreach (var fileName in fileNames)
            {
                _readerBlock.Post(fileName);
            }

            _readerBlock.Complete();
            
            await _writerBlock.Completion;
        }
    }
}