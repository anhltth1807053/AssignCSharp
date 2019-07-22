using System;
using HelloCSharp.entity;
using MySql.Data.MySqlClient;
using Spring_Hero_Bank_on_CSharp.entity;
using Spring_Hero_Bank_on_CSharp.model;

namespace HelloCSharp.model
{
    public class SHBAccountModel
    {  // Bình thường không làm theo cách này,
        // phải mã hoá mật khẩu, kiểm tra tài khoản theo username sau đó so sánh mật khẩu sau khi mã hoá.
        public SHBAccount FindByUsernameAndPassword(string username, string password)
        {
            // Tạo connection đến db, lấy ra trong bảng shb account những tài khoản có username, password trùng.            
            var cmd = new MySqlCommand(
                "select * from shbaccount where username = @username and password = @password",
                ConnectionHelper.GetConnection());
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@password", password);
            // Tạo ra một đối tượng của lớp shbAccount.
            SHBAccount shbAccount = null;
            // Đóng Connection và trả về đối tượng này.  
            var dataReader = cmd.ExecuteReader();

            if (dataReader.Read())
            {
                shbAccount = new SHBAccount
                {
                    AccountNumber = dataReader.GetString("accountNumber"),
                    Username = dataReader.GetString("username"),
                    Password = dataReader.GetString("password"),
                    Balance = dataReader.GetDouble("balance")
                };

            }
            ConnectionHelper.CloseConnection();
            // Trong trường hợp không tìm thấy tài khoản thì trả về null.
            return shbAccount;
        }
        
        public SHBAccount GetAccountByUsername(string username)
        {
            ConnectionHelper.GetConnection();
            var queryString = "select * from `shbaccount` where `username` = @username";
            var cmd = new MySqlCommand(queryString,ConnectionHelper.GetConnection());
            cmd.Parameters.AddWithValue("@username", username);
            var dataReader = cmd.ExecuteReader();
            SHBAccount shbAccount = null;
            if (dataReader.Read())
            {
                shbAccount = new SHBAccount();
                shbAccount.AccountNumber = dataReader.GetString("accountNumber");
                shbAccount.Username = dataReader.GetString("username");
                shbAccount.Password = dataReader.GetString("password");
                shbAccount.Balance = dataReader.GetDouble("balance");
                
            }
            dataReader.Close();
            ConnectionHelper.CloseConnection();
            return shbAccount;
        }
        public SHBAccount GetAccountByAccountNumber(string accountNumber)
        {
            ConnectionHelper.GetConnection();
            var queryString = "select * from `shbaccount` where `accountNumber` = @accountNumber";
            var cmd = new MySqlCommand(queryString, ConnectionHelper.GetConnection());
            cmd.Parameters.AddWithValue("@accountNumber", accountNumber);
            var dataReader = cmd.ExecuteReader();
            SHBAccount shbAccount = null;
            if (dataReader.Read())
            {
                shbAccount = new SHBAccount()
                {
                    AccountNumber = dataReader.GetString("accountNumber"),
                    Username = dataReader.GetString("username"),
                    Password = dataReader.GetString("password"),
                    Balance = dataReader.GetDouble("balance"),
                  
                };
            }
            dataReader.Close();
            ConnectionHelper.CloseConnection();
            return shbAccount;




        }
        public bool UpdateBalance(SHBAccount currentLoggedInAccount, SHBTransaction transaction)
        {
            
            // 1. Kiểm tra số dư tài khoản hiện tại.
            // 2. Update số dư tài khoản hiện tại.
            // 3. Lưu thông tin giao dịch.
            // 4. Commit transaction.
            ConnectionHelper.GetConnection();
            var transaction1 = ConnectionHelper.GetConnection().BeginTransaction(); // mở giao dịch.
            try
            {
                // Kiểm tra số dư tài khoản.
                var cmd = new MySqlCommand("select balance from shbaccount1 where accountNumber = @accountNumber",
                    ConnectionHelper.GetConnection());
                cmd.Parameters.AddWithValue("@accountNumber", currentLoggedInAccount.AccountNumber);
//                SHBAccount shbAccount = null;
                var dataReader = cmd.ExecuteReader();
                double currentAccountBalance = 0;
                if (dataReader.Read())
                {
                    currentAccountBalance = dataReader.GetDouble("balance");
                }

                dataReader.Close();
                if (currentAccountBalance < transaction.Amount)
                {
                    throw new Exception("Không đủ tiền trong tài khoản.");
                }
                
                if (transaction.Type == SHBTransaction.TransactionType.WITHDRAW &&
                    currentAccountBalance < transaction.Amount)
                {
                    throw new Exception("Không đủ tiền trong tài khoản.");

                }

                if (transaction.Type == SHBTransaction.TransactionType.WITHDRAW)
                {
                    currentAccountBalance -= transaction.Amount;
                }
                
                else if (transaction.Type == SHBTransaction.TransactionType.DEPOSIT)
                {
                    currentAccountBalance += transaction.Amount;
                }
               
                

                    var updateQuery =
                    "update `shbaccount` set `balance` = @balance where accountNumber = @accountNumber";
                var sqlCmd = new MySqlCommand(updateQuery, ConnectionHelper.GetConnection());
                sqlCmd.Parameters.AddWithValue("@balance", currentAccountBalance);
                sqlCmd.Parameters.AddWithValue("@accountNumber", currentLoggedInAccount.AccountNumber);
                var updateResult = sqlCmd.ExecuteNonQuery();
                
                var historyTransactionQuery =
                    "insert into `shbtransaction` (transactionId, type, senderAccountNumber, receiverAccountNumber, amount, message) " +
                    "values (@transactionId, @type, @senderAccountNumber, @receiverAccountNumber, @amount, @message)";
                var historyTransactionCmd =
                    new MySqlCommand(historyTransactionQuery, ConnectionHelper.GetConnection());
                historyTransactionCmd.Parameters.AddWithValue("@transactionId", transaction.TransactionId);
                historyTransactionCmd.Parameters.AddWithValue("@amount", transaction.Amount);
                historyTransactionCmd.Parameters.AddWithValue("@type", transaction.Type);
                historyTransactionCmd.Parameters.AddWithValue("@message", transaction.Message);
                historyTransactionCmd.Parameters.AddWithValue("@senderAccountNumber",
                    transaction.SenderAccountNumber);
                historyTransactionCmd.Parameters.AddWithValue("@receiverAccountNumber",
                    transaction.ReceiverAccountNumber);
                var historyResult = historyTransactionCmd.ExecuteNonQuery();

                if (updateResult != 1 || historyResult != 1)
                {
                    throw new Exception("Không thể thêm giao dịch hoặc update tài khoản.");
                }

                transaction1.Commit();
            }
            catch (Exception e)
            {
                 Console.WriteLine(e.Message);
                               transaction1.Rollback(); // lưu giao dịch vào.                
                               return false;
            }
            ConnectionHelper.CloseConnection();
            return true;
        }

        public bool Transfer(SHBAccount currentLoggedInAccount, SHBTransaction transaction)
        {
            ConnectionHelper.GetConnection();
            var transaction1 = ConnectionHelper.GetConnection().BeginTransaction(); // mở giao dịch.
            try
            {
                // Kiểm tra số dư tài khoản.
                var selectBalance =
                    "select balance from shbaccount where accountNumber = @accountNumber";
                var cmdSelect = new MySqlCommand(selectBalance, ConnectionHelper.GetConnection());
                cmdSelect.Parameters.AddWithValue("@accountNumber", currentLoggedInAccount.AccountNumber);
                var dataReader = cmdSelect.ExecuteReader();
                double currentAccountBalance = 0;
                if (dataReader.Read())
                {
                    currentAccountBalance = dataReader.GetDouble("balance");

                }

                dataReader.Close();

                if (currentAccountBalance < transaction.Amount)
                {
                    throw new Exception("Không đủ tiền trong tài khoản.");

                }

                currentAccountBalance -= transaction.Amount;
                //Tiến hành trừ tiền tài khoản gửi.





                // Update tài khoản.

                var updateQuery =
                    "update `shbaccount` set `balance` = @balance where accountNumber = @accountNumber";
                var sqlCmd = new MySqlCommand(updateQuery, ConnectionHelper.GetConnection());
                sqlCmd.Parameters.AddWithValue("@balance", currentAccountBalance);
                sqlCmd.Parameters.AddWithValue("@accountNumber", currentLoggedInAccount.AccountNumber);
                var updateResult = sqlCmd.ExecuteNonQuery();

                // Kiểm tra số dư tài khoản.
                var selectBalanceReceiver =
                    "select balance from `shbaccount` where accountNumber = @accountNumber";
                var cmdSelectReceiver = new MySqlCommand(selectBalanceReceiver, ConnectionHelper.GetConnection());
                cmdSelectReceiver.Parameters.AddWithValue("@accountNumber", transaction.ReceiverAccountNumber);
                var readerReceiver = cmdSelectReceiver.ExecuteReader();
                double receiverBalance = 0;
                if (readerReceiver.Read())
                {
                    receiverBalance = readerReceiver.GetDouble("balance");
                }

                readerReceiver.Close(); // important. 
                //Tiến hành cộng tiền tài khoản nhận.
                receiverBalance += transaction.Amount;

                // Update tài khoản.
                var updateQueryReceiver =
                    "update `shbaccount` set `balance` = @balance where accountNumber = @accountNumber";
                var sqlCmdReceiver = new MySqlCommand(updateQueryReceiver, ConnectionHelper.GetConnection());
                sqlCmdReceiver.Parameters.AddWithValue("@balance", receiverBalance);
                sqlCmdReceiver.Parameters.AddWithValue("@accountNumber", transaction.ReceiverAccountNumber);
                var updateResultReceiver = sqlCmdReceiver.ExecuteNonQuery();

                // Lưu lịch sử giao dịch.
                var historyTransactionQuery =
                    "insert into `shbtransaction` (transactionId, amount, type, message, senderAccountNumber, receiverAccountNumber) " +
                    "values (@transactionId, @amount, @type, @message, @senderAccountNumber, @receiverAccountNumber)";
                var historyTransactionCmd =
                    new MySqlCommand(historyTransactionQuery, ConnectionHelper.GetConnection());
                historyTransactionCmd.Parameters.AddWithValue("@transactionId", transaction.TransactionId);
                historyTransactionCmd.Parameters.AddWithValue("@amount", transaction.Amount);
                historyTransactionCmd.Parameters.AddWithValue("@type", transaction.Type);
                historyTransactionCmd.Parameters.AddWithValue("@message", transaction.Message);
                historyTransactionCmd.Parameters.AddWithValue("@senderAccountNumber",
                    transaction.SenderAccountNumber);
                historyTransactionCmd.Parameters.AddWithValue("@receiverAccountNumber",
                    transaction.ReceiverAccountNumber);
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
    }
    }
