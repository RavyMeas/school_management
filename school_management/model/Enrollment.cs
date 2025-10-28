using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace school_management.model
{
    // Relationship: Student ↔ Class
    public class Enrollment
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int ClassId { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public string Status { get; set; } // Active, Dropped, Completed
    }

    // Relationship: Teacher ↔ Class
    public class ClassAssignment
    {
        public int Id { get; set; }
        public int TeacherId { get; set; }
        public int ClassId { get; set; }
        public DateTime AssignmentDate { get; set; }
        public string Role { get; set; } // Primary, Assistant
    }

    // Student Grades in a Class/Subject
    public class Grade
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int ClassId { get; set; }
        public string Subject { get; set; }
        public double Score { get; set; }
        public string LetterGrade { get; set; } // A, B, C, D, F
        public string Term { get; set; }
        public DateTime GradeDate { get; set; }
    }
}
