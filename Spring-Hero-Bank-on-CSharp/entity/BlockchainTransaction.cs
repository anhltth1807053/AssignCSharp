namespace HelloCSharp.entity
{
    public class BlockchainTransaction
    {
        public BlockchainTransaction()
        {
        }
        public enum TransactionType
        {
            WITHDRAW = 1,
            DEPOSIT = 2, 
            TRANSFER = 3
        }
        
        public enum TransactionStatus
        {
            DONE = 1,
            PROTECTED = 2,
            DELETED = 3
        }

        public string TransactionId { get; set; }
        public string SenderAddress { get; set; }
        public string ReceiverAddress { get; set; }
        public double Amount { get; set; }
        public TransactionType Type { get; set; }
        public string Message { get; set; }
        public long CreateAtMlS { get; set; }
        public long UpdateAtMlS { get; set; }
        public TransactionStatus Status { get; set; }
    }
}