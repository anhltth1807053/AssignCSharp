namespace HelloCSharp.entity
{
    public class SHBAccount
    {
        public SHBAccount()
        {
        }

        public string AccountNumber { get; set; }
        public string Password { get; set; }
        public string Username { get; set; }
        public double Balance { get; set; }
        public long CreateAtMLS { get; set; }
        public long UpdateAtMLS { get; set; }
    }
}