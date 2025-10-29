using school_management.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace school_management.Services
{
    public class ClassService
    {
        private static ClassService _instance;
        private List<ClassRoom> _classes;
        private List<Enrollment> _enrollments;
        private List<ClassAssignment> _classAssignments;
        private int _nextClassId;
        private int _nextEnrollmentId;
        private int _nextAssignmentId;

        public static ClassService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ClassService();
                }
                return _instance;
            }
        }

        private ClassService()
        {
            _classes = new List<ClassRoom>();
            _enrollments = new List<Enrollment>();
            _classAssignments = new List<ClassAssignment>();
            _nextClassId = 1;
            _nextEnrollmentId = 1;
            _nextAssignmentId = 1;
            InitializeSampleData();
        }

        private void InitializeSampleData()
        {
            var teachers = TeacherService.Instance.GetAllTeachers();

            AddClass(new ClassRoom
            {
                ClassName = "10A",
                Grade = "Grade 10",
                Homeroom = "Room 101",
                Capacity = 30,
                CurrentEnrollment = 0,
                TeacherId = teachers.Count > 0 ? teachers[0].Id : 0,
                TeacherName = teachers.Count > 0 ? teachers[0].FullName : "",
                AcademicYear = "2024-2025",
                Term = "Semester 1"
            });

            AddClass(new ClassRoom
            {
                ClassName = "9B",
                Grade = "Grade 9",
                Homeroom = "Room 205",
                Capacity = 25,
                CurrentEnrollment = 0,
                TeacherId = teachers.Count > 1 ? teachers[1].Id : 0,
                TeacherName = teachers.Count > 1 ? teachers[1].FullName : "",
                AcademicYear = "2024-2025",
                Term = "Semester 1"
            });

            AddClass(new ClassRoom
            {
                ClassName = "11A",
                Grade = "Grade 11",
                Homeroom = "Room 304",
                Capacity = 28,
                CurrentEnrollment = 0,
                TeacherId = teachers.Count > 2 ? teachers[2].Id : 0,
                TeacherName = teachers.Count > 2 ? teachers[2].FullName : "",
                AcademicYear = "2024-2025",
                Term = "Semester 1"
            });
        }

        // Class CRUD Operations
        public List<ClassRoom> GetAllClasses()
        {
            return new List<ClassRoom>(_classes);
        }

        public ClassRoom GetClassById(int id)
        {
            return _classes.FirstOrDefault(c => c.Id == id);
        }

        public bool AddClass(ClassRoom classItem)
        {
            try
            {
                if (classItem == null)
                    return false;

                // Validate required fields
                if (string.IsNullOrWhiteSpace(classItem.ClassName) ||
                    string.IsNullOrWhiteSpace(classItem.Grade) ||
                    string.IsNullOrWhiteSpace(classItem.Homeroom))
                {
                    return false;
                }

                // Assign new ID
                classItem.Id = _nextClassId++;

                // Ensure CurrentEnrollment is 0 for new classes
                classItem.CurrentEnrollment = 0;

                // Add to list
                _classes.Add(classItem);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateClass(ClassRoom classItem)
        {
            try
            {
                var existingClass = GetClassById(classItem.Id);
                if (existingClass != null)
                {
                    existingClass.ClassName = classItem.ClassName;
                    existingClass.Grade = classItem.Grade;
                    existingClass.Homeroom = classItem.Homeroom;
                    existingClass.Capacity = classItem.Capacity;
                    existingClass.TeacherId = classItem.TeacherId;
                    existingClass.TeacherName = classItem.TeacherName;
                    existingClass.AcademicYear = classItem.AcademicYear;
                    existingClass.Term = classItem.Term;
                    // Note: Don't update CurrentEnrollment manually, it's calculated
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteClass(int id)
        {
            try
            {
                var classItem = GetClassById(id);
                if (classItem != null)
                {
                    _classes.Remove(classItem);
                    // Remove related enrollments
                    _enrollments.RemoveAll(e => e.ClassId == id);
                    _classAssignments.RemoveAll(a => a.ClassId == id);
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public List<ClassRoom> SearchClasses(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                return GetAllClasses();
            }

            searchText = searchText.ToLower();
            return _classes.Where(c =>
                (c.ClassName?.ToLower().Contains(searchText) ?? false) ||
                (c.Grade?.ToLower().Contains(searchText) ?? false) ||
                (c.Homeroom?.ToLower().Contains(searchText) ?? false) ||
                (c.TeacherName?.ToLower().Contains(searchText) ?? false)
            ).ToList();
        }

        public int GetClassCount()
        {
            return _classes.Count;
        }

        // Enrollment Operations
        public bool EnrollStudent(int studentId, int classId)
        {
            try
            {
                var classItem = GetClassById(classId);
                if (classItem == null)
                {
                    throw new Exception("Class not found");
                }

                // Check if already enrolled
                if (IsStudentEnrolled(studentId, classId))
                {
                    throw new Exception("Student is already enrolled in this class");
                }

                // Check capacity
                if (classItem.CurrentEnrollment >= classItem.Capacity)
                {
                    throw new Exception($"Class is full (Capacity: {classItem.Capacity})");
                }

                var enrollment = new Enrollment
                {
                    Id = _nextEnrollmentId++,
                    StudentId = studentId,
                    ClassId = classId,
                    EnrollmentDate = DateTime.Now,
                    Status = "Active"
                };
                _enrollments.Add(enrollment);

                // Update class enrollment count
                UpdateClassEnrollmentCount(classId);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool EnrollStudentInMultipleClasses(int studentId, List<int> classIds)
        {
            try
            {
                foreach (var classId in classIds)
                {
                    EnrollStudent(studentId, classId);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool UnenrollStudent(int studentId, int classId)
        {
            try
            {
                var enrollment = _enrollments.FirstOrDefault(e =>
                    e.StudentId == studentId &&
                    e.ClassId == classId &&
                    e.Status == "Active");

                if (enrollment != null)
                {
                    enrollment.Status = "Dropped";

                    // Update class enrollment count
                    UpdateClassEnrollmentCount(classId);

                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public void UnenrollStudentFromAllClasses(int studentId)
        {
            var studentEnrollments = _enrollments
                .Where(e => e.StudentId == studentId && e.Status == "Active")
                .ToList();

            foreach (var enrollment in studentEnrollments)
            {
                enrollment.Status = "Dropped";
                UpdateClassEnrollmentCount(enrollment.ClassId);
            }
        }

        public bool IsStudentEnrolled(int studentId, int classId)
        {
            return _enrollments.Any(e =>
                e.StudentId == studentId &&
                e.ClassId == classId &&
                e.Status == "Active");
        }

        public List<ClassRoom> GetClassesForStudent(int studentId)
        {
            var enrolledClassIds = _enrollments
                .Where(e => e.StudentId == studentId && e.Status == "Active")
                .Select(e => e.ClassId)
                .ToList();

            return _classes.Where(c => enrolledClassIds.Contains(c.Id)).ToList();
        }

        public List<Student> GetStudentsInClass(int classId)
        {
            var studentIds = _enrollments
                .Where(e => e.ClassId == classId && e.Status == "Active")
                .Select(e => e.StudentId)
                .ToList();

            var studentService = StudentService.Instance;
            return studentService.GetAllStudents().Where(s => studentIds.Contains(s.Id)).ToList();
        }

        public int GetEnrollmentCount(int classId)
        {
            return _enrollments.Count(e => e.ClassId == classId && e.Status == "Active");
        }

        public List<ClassRoom> GetAvailableClassesForStudent(int studentId, string grade = null)
        {
            var allClasses = string.IsNullOrWhiteSpace(grade)
                ? GetAllClasses()
                : GetAllClasses().Where(c => c.Grade == grade).ToList();

            // Filter out classes that are full or student is already enrolled in
            return allClasses.Where(c =>
                !IsStudentEnrolled(studentId, c.Id) &&
                c.CurrentEnrollment < c.Capacity
            ).ToList();
        }

        // Helper method to update class enrollment count
        private void UpdateClassEnrollmentCount(int classId)
        {
            var classItem = GetClassById(classId);
            if (classItem != null)
            {
                classItem.CurrentEnrollment = _enrollments.Count(e =>
                    e.ClassId == classId &&
                    e.Status == "Active");
            }
        }

        // Refresh all class enrollment counts (useful after bulk operations)
        public void RefreshAllEnrollmentCounts()
        {
            foreach (var classItem in _classes)
            {
                classItem.CurrentEnrollment = _enrollments.Count(e =>
                    e.ClassId == classItem.Id &&
                    e.Status == "Active");
            }
        }

        // Teacher Assignment Operations
        public bool AssignTeacherToClass(int teacherId, int classId)
        {
            try
            {
                // Remove existing assignment for this class if any
                _classAssignments.RemoveAll(a => a.ClassId == classId);

                var assignment = new ClassAssignment
                {
                    Id = _nextAssignmentId++,
                    TeacherId = teacherId,
                    ClassId = classId,
                    AssignmentDate = DateTime.Now,
                    Role = "Primary"
                };
                _classAssignments.Add(assignment);

                // Update class teacher info
                var classItem = GetClassById(classId);
                var teacher = TeacherService.Instance.GetTeacherById(teacherId);
                if (classItem != null && teacher != null)
                {
                    classItem.TeacherId = teacherId;
                    classItem.TeacherName = teacher.FullName;
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public List<ClassRoom> GetClassesForTeacher(int teacherId)
        {
            var classIds = _classAssignments
                .Where(a => a.TeacherId == teacherId)
                .Select(a => a.ClassId)
                .Distinct()
                .ToList();

            return _classes.Where(c => classIds.Contains(c.Id)).ToList();
        }

        // Statistics Methods
        public Dictionary<string, int> GetClassStatistics()
        {
            return new Dictionary<string, int>
            {
                { "TotalClasses", _classes.Count },
                { "TotalStudents", _enrollments.Count(e => e.Status == "Active") },
                { "AvailableSeats", _classes.Sum(c => c.Capacity - c.CurrentEnrollment) },
                { "FullClasses", _classes.Count(c => c.CurrentEnrollment >= c.Capacity) }
            };
        }
    }
}