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
            // Create sample students
            var student1 = new Student
            {
                FirstName = "John",
                LastName = "Doe",
              
                Grade = "Grade 10",
                Class = "10A",
                Email = "john.doe@school.com",
                EnrollmentDate = DateTime.Now.AddMonths(-6),
                Status = "Active"
            };
            AddStudent(student1);

            var student2 = new Student
            {
                FirstName = "Jane",
                LastName = "Smith",
                Grade = "Grade 9",
                Class = "9B",
                Email = "jane.smith@school.com",
                EnrollmentDate = DateTime.Now.AddMonths(-6),
                Status = "Active"
            };
            AddStudent(student2);

            var student3 = new Student
            {
                FirstName = "Mike",
                LastName = "Johnson",
                Grade = "Grade 11",
                Class = "11A",
                Email = "mike.johnson@school.com",
                EnrollmentDate = DateTime.Now.AddMonths(-6),
                Status = "Active"
            };
            AddStudent(student3);
        }

        // Student CRUD Operations
        public List<Student> GetAllStudents()
        {
            return new List<Student>(_students);
        }

        public Student GetStudentById(int id)
        {
            return _students.FirstOrDefault(s => s.Id == id);
        }

        public bool AddStudent(Student student)
        {
            try
            {
                if (student == null)
                    return false;

                // Validate required fields
                if (string.IsNullOrWhiteSpace(student.FirstName) ||
                    string.IsNullOrWhiteSpace(student.LastName))
                {
                    return false;
                }

                // Assign new ID
                student.Id = _nextId++;

                // Set default values
                if (student.EnrollmentDate == default)
                {
                    student.EnrollmentDate = DateTime.Now;
                }

                if (string.IsNullOrWhiteSpace(student.Status))
                {
                    student.Status = "Active";
                }

                // Add to list
                _students.Add(student);

                // Auto-enroll student in matching class
                if (!string.IsNullOrWhiteSpace(student.Class))
                {
                    AutoEnrollStudentInClass(student);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateStudent(Student student)
        {
            try
            {
                var existingStudent = GetStudentById(student.Id);
                if (existingStudent != null)
                {
                    // Store old values
                    var oldClass = existingStudent.Class;
                    var oldGrade = existingStudent.Grade;

                    // Update student properties
                    existingStudent.FirstName = student.FirstName;
                    existingStudent.LastName = student.LastName;
                    existingStudent.Email = student.Email;
                    existingStudent.Status = student.Status;
                    existingStudent.Grade = student.Grade;
                    existingStudent.Class = student.Class;
                    existingStudent.EnrollmentDate = student.EnrollmentDate;

                    // Handle class/grade changes
                    var classService = ClassService.Instance;

                    // If grade changed, unenroll from all classes
                    if (oldGrade != student.Grade)
                    {
                        classService.UnenrollStudentFromAllClasses(student.Id);

                        // Enroll in new class if specified
                        if (!string.IsNullOrWhiteSpace(student.Class))
                        {
                            AutoEnrollStudentInClass(existingStudent);
                        }
                    }
                    // If only class changed (same grade)
                    else if (oldClass != student.Class)
                    {
                        // Unenroll from old class
                        var oldClassObj = classService.GetAllClasses()
                            .FirstOrDefault(c => c.ClassName == oldClass);
                        if (oldClassObj != null)
                        {
                            classService.UnenrollStudent(student.Id, oldClassObj.Id);
                        }

                        // Enroll in new class
                        if (!string.IsNullOrWhiteSpace(student.Class))
                        {
                            AutoEnrollStudentInClass(existingStudent);
                        }
                    }

                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteStudent(int id)
        {
            try
            {
                var student = GetStudentById(id);
                if (student != null)
                {
                    // Unenroll from all classes first
                    var classService = ClassService.Instance;
                    classService.UnenrollStudentFromAllClasses(id);

                    // Remove student
                    _students.Remove(student);
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
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
                (s.FirstName?.ToLower().Contains(searchText) ?? false) ||
                (s.LastName?.ToLower().Contains(searchText) ?? false) ||
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

        public List<Student> GetStudentsByGrade(string grade)
        {
            return _students.Where(s => s.Grade == grade && s.Status == "Active").ToList();
        }

        public List<Student> GetStudentsByClass(string className)
        {
            return _students.Where(s => s.Class == className && s.Status == "Active").ToList();
        }

        public List<Student> GetStudentsByStatus(string status)
        {
            return _students.Where(s => s.Status == status).ToList();
        }

        // Class-related operations
        private void AutoEnrollStudentInClass(Student student)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(student.Class))
                    return;

                var classService = ClassService.Instance;
                var matchingClass = classService.GetAllClasses()
                    .FirstOrDefault(c => c.ClassName == student.Class);

                if (matchingClass != null)
                {
                    // Verify grade matches
                    if (matchingClass.Grade == student.Grade)
                    {
                        // Check if already enrolled
                        if (!classService.IsStudentEnrolled(student.Id, matchingClass.Id))
                        {
                            classService.EnrollStudent(student.Id, matchingClass.Id);
                        }
                    }
                }
            }
            catch
            {
                // Silently fail - class enrollment is optional
            }
        }

        public bool EnrollStudentInClass(int studentId, int classId)
        {
            try
            {
                var student = GetStudentById(studentId);
                if (student == null)
                {
                    throw new Exception("Student not found");
                }

                var classService = ClassService.Instance;
                var classItem = classService.GetClassById(classId);

                if (classItem == null)
                {
                    throw new Exception("Class not found");
                }

                // Verify grade matches
                if (classItem.Grade != student.Grade)
                {
                    throw new Exception($"Student grade ({student.Grade}) does not match class grade ({classItem.Grade})");
                }

                // Update student's class property
                student.Class = classItem.ClassName;

                // Enroll in class
                return classService.EnrollStudent(studentId, classId);
            }
            catch
            {
                return false;
            }
        }

        public bool UnenrollStudentFromClass(int studentId, int classId)
        {
            try
            {
                var student = GetStudentById(studentId);
                if (student != null)
                {
                    var classService = ClassService.Instance;
                    var classItem = classService.GetClassById(classId);

                    if (classItem != null && student.Class == classItem.ClassName)
                    {
                        student.Class = null; // Clear class assignment
                    }

                    return classService.UnenrollStudent(studentId, classId);
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public List<ClassRoom> GetStudentClasses(int studentId)
        {
            var classService = ClassService.Instance;
            return classService.GetClassesForStudent(studentId);
        }

        public List<ClassRoom> GetAvailableClassesForStudent(int studentId)
        {
            var student = GetStudentById(studentId);
            if (student == null)
            {
                return new List<ClassRoom>();
            }

            var classService = ClassService.Instance;
            return classService.GetAvailableClassesForStudent(studentId, student.Grade);
        }

        public List<Student> GetStudentsWithoutClasses()
        {
            var classService = ClassService.Instance;
            return _students.Where(s =>
                s.Status == "Active" &&
                string.IsNullOrWhiteSpace(s.Class)
            ).ToList();
        }

        // Statistics
        public Dictionary<string, int> GetStudentStatistics()
        {
            return new Dictionary<string, int>
            {
                { "TotalStudents", _students.Count },
                { "ActiveStudents", _students.Count(s => s.Status == "Active") },
                { "InactiveStudents", _students.Count(s => s.Status == "Inactive") },
                { "Grade9", GetStudentsByGrade("Grade 9").Count },
                { "Grade10", GetStudentsByGrade("Grade 10").Count },
                { "Grade11", GetStudentsByGrade("Grade 11").Count },
                { "Grade12", GetStudentsByGrade("Grade 12").Count },
                { "WithoutClass", GetStudentsWithoutClasses().Count }
            };
        }

        // Bulk operations
        public bool EnrollMultipleStudentsInClass(List<int> studentIds, int classId)
        {
            try
            {
                foreach (var studentId in studentIds)
                {
                    EnrollStudentInClass(studentId, classId);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void RefreshStudentClassAssignments()
        {
            // Refresh all student class assignments based on enrollments
            var classService = ClassService.Instance;

            foreach (var student in _students)
            {
                var enrolledClasses = classService.GetClassesForStudent(student.Id);
                if (enrolledClasses.Any())
                {
                    // Set the student's class to the first enrolled class
                    student.Class = enrolledClasses[0].ClassName;
                }
                else
                {
                    student.Class = null;
                }
            }
        }
    }
}
