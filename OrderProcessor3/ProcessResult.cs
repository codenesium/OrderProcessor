using System.Collections.Generic;

namespace OrderProcessor
{
    public class ProcessResult
    {
        public Dictionary<int, string> Errors { get; private set; } = new Dictionary<int, string>();

        public bool Success
        {
            get
            {
                return this.Errors.Keys.Count == 0;
            }
        }

        public void AddError(int orderId, string error)
        {
            this.Errors[orderId] = error;
        }
    }
}