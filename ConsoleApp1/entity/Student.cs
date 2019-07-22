namespace ConsoleApp1
{
    public class Student
    {
        
            public Student()
            {
            }

            public string RollNumber { get; set; }

            public string FullName { get; set; }

            public string Email { get; set; }

            public override string ToString()
            {
                return $"FullName: {FullName}\nRollNumber: {RollNumber}\nEmail: {Email}\nAddress: {Address} ";
            }

            public string Address { get; set; }

            public Student(string rollNumber, string fullName, string email, string address)
            {
                RollNumber = rollNumber;
                FullName = fullName;
                Email = email;
                Address = address;
            }
        }
}