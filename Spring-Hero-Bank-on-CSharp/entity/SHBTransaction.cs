namespace HelloCSharp.entity
{
    public class SHBTransaction
    {
        public SHBTransaction()
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
        public TransactionType Type { get; set; } // 1. withdraw | 2. deposit | 3. transfer
        public string SenderAccountNumber { get; set; }
        public string ReceiverAccountNumber { get; set; }
        public double Amount { get; set; }
        public string Message { get; set; }
        public TransactionStatus Status { get; set; }
        public long CreateAtMLS { get; set; }
        public long UpdateAtMLS { get; set; }

        public SHBTransaction(string senderAccountNumber, string receiverAccountNumber, double amount, string message)
        {
            SenderAccountNumber = senderAccountNumber;
            ReceiverAccountNumber = receiverAccountNumber;
            Amount = amount;
            Message = message;
        }
    }
}