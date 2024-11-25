namespace EZcore.DAL
{
    public interface IFile
    {
        public string MainFilePath { get; set; }

        public List<string> OtherFilePaths { get; set; }
    }
}
