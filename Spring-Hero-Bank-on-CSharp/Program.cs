using System;
using ConsoleApp1;
using HelloCSharp.entity;
using Spring_Hero_Bank_on_CSharp.entity;
using Spring_Hero_Bank_on_CSharp.@interface;

namespace HelloCSharp
{
    class Program
    {
        public static SHBAccount currentLoggedInAccount;
        public static BlockchainAddress currentLoggedInAddress;

        static void Main(string[] args)
        {

            while (true)
            {
                Console.Clear();
                GiaoDich giaoDich = null;
                Console.WriteLine("Vui lòng lựa chọn 1 trong những kiểu giao dịch dưới đây.");
                Console.WriteLine("========================================================");
                Console.WriteLine("1. Giao dịch bằng ngân hàng Spring Hero Bank.");
                Console.WriteLine("2. Giao dịch bằng Blockchain.");
                Console.WriteLine("3. Thoát.");
                Console.WriteLine("========================================================");
                Console.WriteLine("Vui lòng nhập lựa chọn của bạn: ");
                var choice = int.Parse(Console.ReadLine());

                switch (choice)
                {
                    case 1:
                        giaoDich = new GiaoDichSHB();
                        break;
                    case 2:
                        giaoDich = new GiaoDichBlockchain();
                        break;
                    case 3:
                      
                        break;
                    default:
                        Console.WriteLine("Lựa chọn sai, vui lòng chọn lại.");
                        break;
                }

                if (choice == 3)
                {
                    Console.WriteLine("Hẹn gặp lại");
                    break;
                }

                giaoDich.Login();
                if (currentLoggedInAccount != null)
                {
                    
                    Console.WriteLine("Đăng nhập thành công với tài khoản.");
                    Console.WriteLine($"Tài khoản: {currentLoggedInAccount.Username}");
                    Console.WriteLine($"Số dư: {currentLoggedInAccount.Balance}");
                    Console.WriteLine("Ấn phím bất kỳ để tiếp tục giao dịch.");
                    Console.ReadLine();
                    GenerateTransactionMenu(giaoDich);
                }

                if (currentLoggedInAddress != null)
                {
                    
                     Console.WriteLine("Đăng nhập thành công với tài khoản.");
                     Console.WriteLine($"Địa chỉ: {currentLoggedInAddress.Address}");
                     Console.WriteLine($"Số dư: {currentLoggedInAddress.Balance}");
                     Console.WriteLine("Ấn phím bất kỳ để tiếp tục giao dịch.");
                     Console.ReadLine();
                     GenerateTransactionMenu(giaoDich);
                }
            }
        }

        private static void GenerateTransactionMenu(GiaoDich giaoDich)
        {
            while (true)
            {
                Console.WriteLine("Vui lòng lựa chọn kiểu giao dịch.");
                Console.WriteLine("==================================");
                Console.WriteLine("1. Rút tiền.");
                Console.WriteLine("2. Gửi tiền.");
                Console.WriteLine("3. Chuyển tiền.");
                Console.WriteLine("4. Thoát.");
                Console.WriteLine("===================================");
                Console.WriteLine("Nhập lựa chọn của bạn: ");
                var choice1 = int.Parse(Console.ReadLine());
                switch (choice1)
                {
                    case 1:
                        giaoDich.RutTien();
                        break;
                    case 2:
                        giaoDich.GuiTien();
                        break;
                    case 3:
                        giaoDich.ChuyenTien();
                        break;
                    case 4:
                        break;
                    default:
                        Console.WriteLine("Lựa chọn sai, vui lòng chọn lại.");
                        break;

                }

                if (choice1 == 4)
                {
                    Console.WriteLine("Hẹn gặp lại!");
                    break;
                }
            }
        }
    }
   
}