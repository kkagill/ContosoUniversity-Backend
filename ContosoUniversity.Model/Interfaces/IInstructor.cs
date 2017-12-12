using System;

namespace ContosoUniversity.Model.Interfaces
{   
    public interface IInstructor
    {
        int ID { get; set; }
        string LastName { get; set; }
        string FirstMidName { get; set; }
        DateTime HireDate { get; set; }
        string Office { get; set; }
    }
}