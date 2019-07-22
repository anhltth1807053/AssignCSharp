using System;
using HelloCSharp;
using HelloCSharp.entity;
using HelloCSharp.model;
using MySql.Data.MySqlClient;
using Spring_Hero_Bank_on_CSharp.entity;
using Spring_Hero_Bank_on_CSharp.@interface;
using Spring_Hero_Bank_on_CSharp.model;

namespace ConsoleApp1
{
    public class GiaoDichBlockchain : GiaoDich
    {
        private static BlockchainAddressModel blockchainAddressModel;
        private GiaoDich _giaoDichImplementation;

        public GiaoDichBlockchain()
        {
            blockchainAddressModel = new BlockchainAddressModel();
        }
        public void RutTien()
        {
            if (Program.currentLoggedInAddress != null)
            {
                Console.Clear();
                Console.WriteLine("Tiến hành rút tiền tại ví điện tử Blockchain.");
                Console.WriteLine("Vui lòng nhập số tiền cần rút.");
                var amount = double.Parse(Console.ReadLine());
                if (amount > Program.currentLoggedInAddress.Balance)
                {
                    Console.WriteLine("Số lượng không hợp lệ, vui lòng thử lại.");
                    return;
                }

                var blockchainTransaction = new BlockchainTransaction
                {
                    TransactionId = Guid.NewGuid().ToString(),
                    SenderAddress = Program.currentLoggedInAddress.Address,
                    ReceiverAddress = Program.currentLoggedInAddress.Address,
                    Type = BlockchainTransaction.TransactionType.WITHDRAW,
                    Amount = amount,
                    Message = "Tiến hành rút tiền tại ví điện tử Blockchain với số tiền: " + amount,
                    CreateAtMlS = DateTime.Now.Ticks,
                    UpdateAtMlS = DateTime.Now.Ticks,
                    Status = BlockchainTransaction.TransactionStatus.DONE
                };
                if (blockchainAddressModel.UpdateBalanceBlockchain(Program.currentLoggedInAddress,blockchainTransaction))
                {
                    Console.WriteLine("Giao dịch thành công.");  
                }
            }
            else
            {
                Console.WriteLine("Vui lòng đăng nhập để sử dụng chức năng này.");
            }
        }

        public void GuiTien()
        {
            if (Program.currentLoggedInAddress != null)
            {
                Console.Clear();
                Console.WriteLine("Tiến hành gửi tiền tại ví điện tử Blockchain.");
                Console.WriteLine("Vui lòng nhập số tiền cần gửi.");
                var amount = double.Parse(Console.ReadLine());
                if (amount <= 0)
                {
                    Console.WriteLine("Số lượng không hợp lệ, vui lòng thử lại.");
                    return;
                }

                var blockchainTransaction = new BlockchainTransaction
                {
                    TransactionId = Guid.NewGuid().ToString(),
                    SenderAddress = Program.currentLoggedInAddress.Address,
                    ReceiverAddress = Program.currentLoggedInAddress.Address,
                    Type = BlockchainTransaction.TransactionType.DEPOSIT,
                    Amount = amount,
                    Message = "Tiến hành gửi tiền tại ví điện tử Blockchain với số tiền: " + amount,
                    CreateAtMlS = DateTime.Now.Ticks,
                    UpdateAtMlS = DateTime.Now.Ticks,
                    Status = BlockchainTransaction.TransactionStatus.DONE
                };
                if (blockchainAddressModel.UpdateBalanceBlockchain(Program.currentLoggedInAddress,blockchainTransaction))
                {
                    
                    Console.WriteLine("Giao dịch thành công.");  
                }
            }
            else
            {
                Console.WriteLine("Vui lòng đăng nhập để sử dụng chức năng này.");
            }
        }

        public void ChuyenTien()
        {
            throw new NotImplementedException();
        }


        public bool ChuyenTien(BlockchainAddress currentLoggedInAccount, BlockchainTransaction blockchainTransaction)
        {
            
             ConnectionHelper.GetConnection();
            var transaction1 = ConnectionHelper.GetConnection().BeginTransaction(); // mở giao dịch.
            try
            {
                // Kiểm tra số dư tài khoản.
                var selectBalance =
                    "select balance from blockchain where accountNumber = @accountNumber";
                var cmdSelect = new MySqlCommand(selectBalance, ConnectionHelper.GetConnection());
                cmdSelect.Parameters.AddWithValue("@address", currentLoggedInAccount.Address);
                var dataReader = cmdSelect.ExecuteReader();
                double currentAccountBalance = 0;
                if (dataReader.Read())
                {
                    currentAccountBalance = dataReader.GetDouble("balance");

                }

                dataReader.Close();

                if (currentAccountBalance < blockchainTransaction.Amount)
                {
                    throw new Exception("Không đủ tiền trong tài khoản.");

                }

                currentAccountBalance -= blockchainTransaction.Amount;
                //Tiến hành trừ tiền tài khoản gửi.
                // Update tài khoản.

                var updateQuery =
                    "update `shbaccount` set `blockchain` = @balance where privateKey = @privateKey";
                var sqlCmd = new MySqlCommand(updateQuery, ConnectionHelper.GetConnection());
                sqlCmd.Parameters.AddWithValue("@balance", currentAccountBalance);
                sqlCmd.Parameters.AddWithValue("@privateKey", currentLoggedInAccount.PrivateKey);
                var updateResult = sqlCmd.ExecuteNonQuery();

                // Kiểm tra số dư tài khoản.
                var selectBalanceReceiver =
                    "select balance from `blockchain` where privateKey = @privateKey";
                var cmdSelectReceiver = new MySqlCommand(selectBalanceReceiver, ConnectionHelper.GetConnection());
                cmdSelectReceiver.Parameters.AddWithValue("@privateKey", blockchainTransaction.ReceiverAddress);
                var readerReceiver = cmdSelectReceiver.ExecuteReader();
                double receiverBalance = 0;
                if (readerReceiver.Read())
                {
                    receiverBalance = readerReceiver.GetDouble("balance");
                }

                readerReceiver.Close(); // important. 
                //Tiến hành cộng tiền tài khoản nhận.
                receiverBalance += blockchainTransaction.Amount;

                // Update tài khoản.
                var updateQueryReceiver =
                    "update `blockchain` set `balance` = @balance where privateKey = @privateKey";
                var sqlCmdReceiver = new MySqlCommand(updateQueryReceiver, ConnectionHelper.GetConnection());
                sqlCmdReceiver.Parameters.AddWithValue("@balance", receiverBalance);
                sqlCmdReceiver.Parameters.AddWithValue("@privateKey", blockchainTransaction.ReceiverAddress);
                var updateResultReceiver = sqlCmdReceiver.ExecuteNonQuery();

                // Lưu lịch sử giao dịch.
                var historyTransactionQuery =
                    "insert into `blockchainTransaction` (transactionId, amount, type, message, senderAddress, receiverAddress) " +
                    "values (@transactionId, @amount, @type, @message, @senderAccountNumber, @receiverAccountNumber)";
                var historyTransactionCmd =
                    new MySqlCommand(historyTransactionQuery, ConnectionHelper.GetConnection());
                historyTransactionCmd.Parameters.AddWithValue("@transactionId", blockchainTransaction.TransactionId);
                historyTransactionCmd.Parameters.AddWithValue("@amount", blockchainTransaction.Amount);
                historyTransactionCmd.Parameters.AddWithValue("@type", blockchainTransaction.Type);
                historyTransactionCmd.Parameters.AddWithValue("@message", blockchainTransaction.Message);
                historyTransactionCmd.Parameters.AddWithValue("@senderAddress",
                    blockchainTransaction.SenderAddress);
                historyTransactionCmd.Parameters.AddWithValue("@receiverAddress",
                    blockchainTransaction.ReceiverAddress);
                var historyResult = historyTransactionCmd.ExecuteNonQuery();

                if (updateResult != 1 || historyResult != 1 || updateResultReceiver != 1)
                {
                    throw new Exception("Không thể thêm giao dịch hoặc update tài khoản.");
                }

                transaction1.Commit();
                return true;
            }
            catch (Exception e)
            {
                transaction1.Rollback();
                Console.WriteLine(e.StackTrace);
                Console.WriteLine(e.Message);
                Console.WriteLine(e.Source);
                Console.WriteLine(e.ToString());
                return false;
            }
            finally
            {
                ConnectionHelper.CloseConnection();        
            }
        }

        public void Login()
        {
            Program.currentLoggedInAddress = null;
            Console.Clear();
            Console.WriteLine("Tiến hành đăng nhập hệ thống Blockchain.");
            Console.WriteLine("Vui lòng nhập địa chỉ đăng nhập: ");
            var address = Console.ReadLine();
            Console.WriteLine("Vui lòng nhập private key: ");
            var privateKey = Console.ReadLine();
            var blockchainAddress = blockchainAddressModel.FindByAddressAndPrivateKey(address, privateKey);
            if (blockchainAddress == null)
            {
                Console.WriteLine("Sai địa chỉ tài khoản, vui lòng đăng nhập lại.");
                Console.WriteLine("Ấn phím bất kỳ để tiếp tục.");
                Console.ReadLine();
                return;
            }

            // trong trường hợp trả về khác null.
            // set giá trị vào biến CurrentLoggedInAddress.
            Program.currentLoggedInAddress = blockchainAddress;

        }
    }
}