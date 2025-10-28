using school_management.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace school_management.Services
{
    public class TeacherService
    {
        private static TeacherService _instance;
        private List<Teacher> _teachers;
        private int _nextId;

        public static TeacherService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TeacherService();
                }
                return _instance;
            }
        }

        private TeacherService()
        {
            _teachers = new List<Teacher>();
            _nextId = 1;
            InitializeSampleData();
        }

        private void InitializeSampleData()
        {
            AddTeacher(new Teacher
            {
                FirstName = "Sarah",
                LastName = "Johnson",
                Email = "sarah.johnson@school.edu",
                Phone = "555-0101",
                Department = "Science",
                Subject = "Mathematics",
                Status = "Active",
                HireDate = new DateTime(2020, 8, 15)
            });

            AddTeacher(new Teacher
            {
                FirstName = "Michael",
                LastName = "Brown",
                Email = "michael.brown@school.edu",
                Phone = "555-0102",
                Department = "Humanities",
                Subject = "English Literature",
                Status = "Active",
                HireDate = new DateTime(2019, 9, 1)
            });

            AddTeacher(new Teacher
            {
                FirstName = "Emily",
                LastName = "Davis",
                Email = "emily.davis@school.edu",
                Phone = "555-0103",
                Department = "Science",
                Subject = "Physics",
                Status = "Active",
                HireDate = new DateTime(2021, 1, 10)
            });
        }

        public List<Teacher> GetAllTeachers()
        {
            return new List<Teacher>(_teachers);
        }

        public Teacher GetTeacherById(int id)
        {
            return _teachers.FirstOrDefault(t => t.Id == id);
        }

        public void AddTeacher(Teacher teacher)
        {
            teacher.Id = _nextId++;
            _teachers.Add(teacher);
        }

        public void UpdateTeacher(Teacher teacher)
        {
            var existingTeacher = GetTeacherById(teacher.Id);
            if (existingTeacher != null)
            {
                existingTeacher.FirstName = teacher.FirstName;
                existingTeacher.LastName = teacher.LastName;
                existingTeacher.Email = teacher.Email;
                existingTeacher.Phone = teacher.Phone;
                existingTeacher.Department = teacher.Department;
                existingTeacher.Subject = teacher.Subject;
                existingTeacher.Status = teacher.Status;
                existingTeacher.HireDate = teacher.HireDate;
            }
        }

        public void DeleteTeacher(int id)
        {
            var teacher = GetTeacherById(id);
            if (teacher != null)
            {
                _teachers.Remove(teacher);
            }
        }

        public int GetTeacherCount()
        {
            return _teachers.Count;
        }

        public List<Teacher> SearchTeachers(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                return GetAllTeachers();
            }

            searchText = searchText.ToLower();
            return _teachers.Where(t =>
                t.FullName.ToLower().Contains(searchText) ||
                t.Email.ToLower().Contains(searchText) ||
                t.Department.ToLower().Contains(searchText) ||
                t.Subject.ToLower().Contains(searchText)
            ).ToList();
        }
    }
}
