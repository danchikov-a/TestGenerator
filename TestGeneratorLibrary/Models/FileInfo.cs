namespace TestGeneratorLibrary.Models
{
    public class FileInfo
    {
        public string FileName
        {
            get;
        }

        public string FileContent
        {
            get;
        }

        public FileInfo(string fileName, string fileContent)
        {
            FileName = fileName;
            FileContent = fileContent;
        }
    }
}