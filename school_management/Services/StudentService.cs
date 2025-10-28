using school_management.model;
using System;
using System.Collections.Generic;
using System.Linq;


namespace school_management.Services
{
    public class StudentService
    {
        private static StudentService _instance;
        private List<Student> _students;
        private int _nextId;

        public static StudentService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new StudentService();
                }
                return _instance;
            }
        }

        private StudentService()
        {
            _students = new List<Student>();
            _nextId = 1;
            InitializeSampleData();
        }

        private void InitializeSampleData()
        {
            AddStudent(new Student
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@email.com",
                Grade = "Grade 10",
                Class = "10A",
                EnrollmentDate = new DateTime(2024, 1, 15),
                Status = "Active"
            });

            AddStudent(new Student
            {
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane.smith@email.com",
                Grade = "Grade 9",
                Class = "9B",
                EnrollmentDate = new DateTime(2024, 1, 20),
                Status = "Active"
            });

            AddStudent(new Student
            {
                FirstName = "Mike",
                LastName = "Johnson",
                Email = "mike.johnson@email.com",
                Grade = "Grade 11",
                Class = "11A",
                EnrollmentDate = new DateTime(2024, 2, 1),
                Status = "Active"
            });
        }

        public List<Student> GetAllStudents()
        {
            return new List<Student>(_students);
        }

        public Student GetStudentById(int id)
        {
            return _students.FirstOrDefault(s => s.Id == id);
        }

        public void AddStudent(Student student)
        {
            if (student == null)
                throw new ArgumentNullException(nameof(student));

            student.Id = _nextId++;
            _students.Add(student);
        }

        public void UpdateStudent(Student student)
        {
            if (student == null)
                throw new ArgumentNullException(nameof(student));

            var existingStudent = GetStudentById(student.Id);
            if (existingStudent == null)
                throw new InvalidOperationException($"student ID ${student.Id} not found");
            {
                existingStudent.FirstName = student.FirstName;
                existingStudent.LastName = student.LastName;
                existingStudent.Email = student.Email;
                existingStudent.Grade = student.Grade;
                existingStudent.Class = student.Class;
                existingStudent.EnrollmentDate = student.EnrollmentDate;
                existingStudent.Status = student.Status;
            }
        }

        public void DeleteStudent(int id)
        {
            var student = GetStudentById(id);
            if (student != null)
            {
                _students.Remove(student);
            }
        }

        public List<Student> SearchStudents(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                return GetAllStudents();
            }

            searchText = searchText.ToLower();
            return _students.Where(s =>
                (s.FullName?.ToLower().Contains(searchText) ?? false) ||
                (s.Email?.ToLower().Contains(searchText) ?? false) ||
                (s.Grade?.ToLower().Contains(searchText) ?? false) ||
                (s.Class?.ToLower().Contains(searchText) ?? false)
            ).ToList();
        }

        public int GetStudentCount()
        {
            return _students.Count;
        }
    }
}
