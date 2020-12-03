namespace OrderProcessor
{
    public class ProcessResult
    {
        public bool Success { get; private set; }

        public ProcessResult(bool success)
        {
            this.Success = success;
        }
    }
}