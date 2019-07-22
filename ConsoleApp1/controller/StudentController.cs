using System;
using ConsoleApp1.model;

namespace ConsoleApp1.controller
{
    public class StudentController
    {
        private static StudentModel _studentModel;

        public StudentController()
        {
            if (_studentModel == null)
            {
                _studentModel = new StudentModel();
            }
        }
        public void CreateStudent()
        {
            
            Console.Clear();
            Console.WriteLine("==============CREATE NEW STUDENT===============");
            var student = new Student();
            Console.WriteLine("Please enter information.");
            Console.WriteLine("Please enter your rollNumber: ");
            student.RollNumber = Console.ReadLine();
            Console.WriteLine("Please enter your name: ");
            student.FullName = Console.ReadLine();
            Console.WriteLine("Please enter your email: ");
            student.Email = Console.ReadLine();
            Console.WriteLine("Please enter your address: ");
            student.Address = Console.ReadLine();
            Console.WriteLine(student.ToString());
            _studentModel.Save(student);
        }

        public void ShowList()
        {
            Console.Clear();
            Console.WriteLine("==============LIST STUDENT=================");
            var list = StudentModel.FindAll();
            if (list.Count == 0)
            {
                Console.WriteLine("Have no student in list");
                return;
            }
            
            Console.WriteLine("{0,-20} {1,-20} {2,-30} {3,-20}","RollNumber","FullName","Email","Address");
            foreach (var student in list)
            {
                Console.WriteLine("{0,-20} {1,-20} {2,-30} {3,-20}", student.RollNumber,student.FullName,student.Email,student.Address);
            }
        }

        public void UpdateInforStudent()
        {
         Console.Clear();
         Console.WriteLine("===============UPDATE STUDENT===============");
         Console.WriteLine("Please enter student rollnumber to update: ");
         var rollNumber = Console.ReadLine();
         var student = _studentModel.FindById(rollNumber);
         if (student == null)
         {
             Console.WriteLine("Student is not found. Please try again.");
             return;
         }

         Console.WriteLine("Please enter infomatin student: ");
         Console.WriteLine("RollNumber: ");
         student.RollNumber = Console.ReadLine();
         Console.WriteLine("Name: ");
         student.FullName = Console.ReadLine();
         Console.WriteLine("Email: ");
         student.Email = Console.ReadLine();
         Console.WriteLine("Address: ");
         student.Address = Console.ReadLine();
         StudentModel.Update(student);
        }

        public void DeleteStudent()
        {
            Console.Clear();
            Console.WriteLine("==============DELETE STUDENT================");
            Console.WriteLine("Enter id student you want delete.");
            var rollNumber = Console.ReadLine();
            var student = _studentModel.FindById(rollNumber);
            if (student == null)
            {
                Console.WriteLine("Don't find id student. Please try again!");
                return;
            }

            Console.WriteLine($"Are you sure wanna delete student withh rollnumber{student.RollNumber} (y/n)");
            var choice = Console.ReadLine().ToLower();
            if (choice.Equals("n"))
            {
                return;
                
            }
            StudentModel.Delete(rollNumber);


        }
    }
}