using System.Threading.Tasks.Dataflow;
using TestGeneratorLibrary;
using TestGeneratorLibrary.Services;
using FileInfo = TestGeneratorLibrary.Models.FileInfo;

namespace TestGeneratorConsole.Dataflow
{
    public class DataflowService
    {
        private readonly TestGenerator _testsGenerator;
        private TransformBlock<string, string> _readerBlock;
        private TransformManyBlock<string, FileInfo> _generatorBlock;
        private ActionBlock<FileInfo> _writerBlock;

        private readonly DataflowLinkOptions _linkOptions = new()
        {
            PropagateCompletion = true
        };

        public DataflowService(int readTaskAmount, 
            int generateTaskAmount, 
            int writeTaskAmount,
            string savePath)
        {
            _testsGenerator = new TestGenerator(new NamespaceService(new ClassService()));

            ConfigureBlocks(readTaskAmount, generateTaskAmount, writeTaskAmount, savePath);
        }

        private void ConfigureBlocks(int readTaskRestriction, int generateTaskRestriction, int writeTaskRestriction, string savePath)
        {
            _readerBlock = new TransformBlock<string, string>(
                async fileName => await CodeReader.Read(fileName),
                new ExecutionDataflowBlockOptions {MaxDegreeOfParallelism = readTaskRestriction}
            );
            
            _generatorBlock = new TransformManyBlock<string, FileInfo>(
                source => _testsGenerator.Generate(source),
                new ExecutionDataflowBlockOptions {MaxDegreeOfParallelism = generateTaskRestriction}
            );
            
            _writerBlock = new ActionBlock<FileInfo>(
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