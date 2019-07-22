using System;
using System.Collections.Generic;
using System.Xml.Linq;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Crypto.Tls;

namespace ConsoleApp1.model
{
    
    public class StudentModel
    
    {
       

        public  void Save(Student obj)
        {
        
//            

            var cmd = new MySqlCommand("insert into students (rollNumber, fullName, email, address)" +
                                                "values(@rollNumber, @fullName, @email, @address)",ConnectionHelper.GetConnection());
            cmd.Parameters.AddWithValue("@rollNumber",obj.RollNumber);
            cmd.Parameters.AddWithValue("@fullName",obj.FullName);
            cmd.Parameters.AddWithValue("@email",obj.Email);
            cmd.Parameters.AddWithValue("@address",obj.Address);
            cmd.ExecuteNonQuery();
            ConnectionHelper.CloseConnection();
            Console.WriteLine("Add Object Success!");
        }
        
        public static List<Student> FindAll()
        {
            
            var list = new List<Student>();
           
            var cmd = new MySqlCommand("select * from students",ConnectionHelper.GetConnection());
            var dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                var obj = new Student
                {
                    RollNumber = dataReader.GetString("rollNumber"),
                    FullName = dataReader.GetString("fullName"),
                    Email = dataReader.GetString("email"),
                    Address = dataReader.GetString("address")
                };
                


                list.Add(obj);
          
            }
            ConnectionHelper.CloseConnection();
            return list;
            
        }

        public Student FindById(string rollNumber)
        {
            var cmd = new MySqlCommand("select * from students where rollNumber = @rollNumber",
                ConnectionHelper.GetConnection());
            cmd.Parameters.AddWithValue("@rollNumber", rollNumber);
            var dataReader = cmd.ExecuteReader();
            if (!dataReader.Read())

            {
                return null;
            }
            var obj = new Student
            {
                RollNumber = dataReader.GetString("rollNumber"),
                FullName = dataReader.GetString("fullName"),
                Email = dataReader.GetString("email"),
                Address = dataReader.GetString("address"),
            };
            ConnectionHelper.CloseConnection();
            return obj;
        }

        public static void Update(Student obj)
        {
           
            {
                var cmd = new MySqlCommand ("update students set  fullName = @fullName, email = @email, address = @address where rollNumber = @rollNumber ",
                ConnectionHelper.GetConnection());
                cmd.Parameters.AddWithValue("@rollNumber", obj.RollNumber);
                cmd.Parameters.AddWithValue("@fullName", obj.FullName);
                cmd.Parameters.AddWithValue("@email", obj.Email);
                cmd.Parameters.AddWithValue("@address", obj.Address);
                cmd.ExecuteNonQuery();
                ConnectionHelper.CloseConnection();
            }
            
        }

        public static void Delete(string id)
        {
            var cmd = new MySqlCommand("delete from students where rollNumber=@rollNumber",
                ConnectionHelper.GetConnection());
                cmd.Parameters.AddWithValue("rollNumber", id);
                    cmd.ExecuteNonQuery();
                        ConnectionHelper.CloseConnection();
                        Console.WriteLine("Delete Success!");
                
        }
    }
}