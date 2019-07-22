using System;
using HelloCSharp;
using HelloCSharp.model;
using Spring_Hero_Bank_on_CSharp.entity;

namespace Spring_Hero_Bank_on_CSharp.@interface
{
    public class GiaoDichSHB : GiaoDich
    {
        private static SHBAccountModel shbAccountModel;

        public GiaoDichSHB()
        {
            shbAccountModel = new SHBAccountModel();
        }


        public void RutTien()
        {
            if (Program.currentLoggedInAccount != null)
            {
                Console.Clear();
                Console.WriteLine("Tiến hành rút tiền tại hệ thống SHB.");
                Console.WriteLine("Vui lòng nhập số tiền cần rút.");
                var amount = double.Parse(Console.ReadLine());
                
                if (amount > Program.currentLoggedInAccount.Balance)
                {
                    Console.WriteLine("Số lượng không hợp lệ, vui lòng thử lại.");
                    return;
                }

                var transaction = new SHBTransaction
                {
                    TransactionId = Guid.NewGuid().ToString(),
                    SenderAccountNumber = Program.currentLoggedInAccount.AccountNumber,
                    ReceiverAccountNumber = Program.currentLoggedInAccount.AccountNumber,
                    Type = SHBTransaction.TransactionType.WITHDRAW,
                    Amount = amount,
                    Message = "Tiến hành rút tiền tại ATM với số tiền: " + amount,
                    CreateAtMLS = DateTime.Now.Ticks,
                    UpdateAtMLS = DateTime.Now.Ticks,
                    Status = SHBTransaction.TransactionStatus.DONE
                };
                if (shbAccountModel.UpdateBalance(Program.currentLoggedInAccount,transaction))
                {
                   Console.WriteLine("Giao dịch thành công.");  
                }
//                bool result = shbAccountModel.UpdateBalance(Program.currentLoggedInAccount, transaction);
              
            }
            else
            {
                Console.WriteLine("Vui lòng đăng nhập để sử dụng chức năng này.");
            }
        }

        public void GuiTien()
        {
            if (Program.currentLoggedInAccount != null)
            {
                Console.Clear();
                Console.WriteLine("Tiến hành gửi tiền tại hệ thống SHB.");
                Console.WriteLine("Vui lòng nhập số tiền cần gửi.");
                var amount = double.Parse(Console.ReadLine());
                
                if (amount <= 0)
                {
                    Console.WriteLine("Số lượng không hợp lệ, vui lòng thử lại.");
                    return;
                }
                var transaction = new SHBTransaction
                {
                    TransactionId = Guid.NewGuid().ToString(),
                    SenderAccountNumber = Program.currentLoggedInAccount.AccountNumber,
                    ReceiverAccountNumber = Program.currentLoggedInAccount.AccountNumber,
                    Type = SHBTransaction.TransactionType.DEPOSIT,
                    Amount = amount,
                    Message = "Tiến hành gửi tiền tại ngân hàng với số tiền: " + amount,
                    CreateAtMLS = DateTime.Now.Ticks,
                    UpdateAtMLS = DateTime.Now.Ticks,
                    Status = SHBTransaction.TransactionStatus.DONE
                };
                if (shbAccountModel.UpdateBalance(Program.currentLoggedInAccount,transaction))
                {
                    Console.WriteLine("Giao dịch thành công.");  
                }
//                bool result = shbAccountModel.UpdateBalance(Program.currentLoggedInAccount, transaction);
              
            }
            else
            {
                Console.WriteLine("Vui lòng đăng nhập để sử dụng chức năng này.");
            }
        }
        

        public void ChuyenTien()
        {
            if (Program.currentLoggedInAccount != null)
            {
                Console.WriteLine("Tiến hành chuyển tiền tại hệ thống SHB.");
                Console.WriteLine("Vui lòng nhập số tài khoản chuyển tiền: ");
                var accountNumber = Console.ReadLine();
                var receiverAccount = shbAccountModel.GetAccountByAccountNumber(accountNumber);
                if (receiverAccount == null)
                {
                    Console.WriteLine("Tài khoản nhận tiền không tồn tại hoặc đã bị khoá.");
                    return;
                }
                Console.WriteLine("Tài khoản nhận tiền: " + accountNumber);
                Console.WriteLine("Chủ tài khoản: " + receiverAccount.Username);
                Console.WriteLine("Nhập số tiền chuyển khoản: ");
                var amount = double.Parse(Console.ReadLine());
                Program.currentLoggedInAccount = shbAccountModel.GetAccountByUsername(Program.currentLoggedInAccount.Username);
                if (amount > Program.currentLoggedInAccount.Balance)
                {
                    Console.WriteLine("Số dư tài khoản không đủ thực hiện giao dịch.");
                    return;
                }
                Console.WriteLine("Nhập nội dung giao dịch: ");
                var message = Console.ReadLine();
                var shbTransaction  = new SHBTransaction()
                {
                    TransactionId = Guid.NewGuid().ToString(),
                    Type = SHBTransaction.TransactionType.TRANSFER,
                    Amount = amount,    
                    Message = message,
                    CreateAtMLS = DateTime.Now.Ticks,
                    UpdateAtMLS = DateTime.Now.Ticks,
                    Status = SHBTransaction.TransactionStatus.DONE,
                    SenderAccountNumber = Program.currentLoggedInAccount.AccountNumber,
                    ReceiverAccountNumber = accountNumber
                };
                if (shbAccountModel.Transfer(Program.currentLoggedInAccount, shbTransaction))
                {
                    Console.WriteLine("Giao dịch thành công.");
                }
                else
                {
                    Console.WriteLine("Giao dịch thất bại, vui lòng thử lại.");
                }
            }
        }

        public void Login()
        {
            Program.currentLoggedInAccount = null;
            Console.Clear();
            Console.WriteLine("Tiến hành đăng nhập hệ thống SHB.");
            // Yêu cầu nhập username, password.
            Console.WriteLine("Vui lòng nhập usename: ");
            var username = Console.ReadLine();
            Console.WriteLine("Vui lòng nhập mật khẩu: ");
            var password = Console.ReadLine();
            // gọi đến model kiểm, nếu model trả về null thì báo đăng nhập sai.
            var shbAccount = shbAccountModel.FindByUsernameAndPassword(username, password);
            if (shbAccount == null)
            {
                Console.WriteLine("Sai thông tin tài khoản, vui lòng đăng nhập lại.");
                Console.WriteLine("Ấn phím bất kỳ để tiếp tục.");
                Console.ReadLine();
                return;
            }

            // trong trường hợp trả về khác null.
            // set giá trị vào biến currentLoggedInAccount.
            Program.currentLoggedInAccount = shbAccount;
            
        }
    }
}