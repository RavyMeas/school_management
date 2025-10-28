using school_management.Commands;
using school_management.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace school_management.view_model
{
    public class DashboardViewModel : INotifyPropertyChanged
    {
        private readonly StudentService _studentService;
        private readonly ClassService _classService;
        private readonly TeacherService _teacherService;
        private int _totalStudents;
        private int _totalTeachers;
        private int _totalClasses;
        private string _academicYear;

        public int TotalStudents
        {
            get => _totalStudents;
            set
            {
                _totalStudents = value;
                OnPropertyChanged();
            }
        }

        public int TotalTeachers
        {
            get => _totalTeachers;
            set
            {
                _totalTeachers = value;
                OnPropertyChanged();
            }
        }

        public int TotalClasses
        {
            get => _totalClasses;
            set
            {
                _totalClasses = value;
                OnPropertyChanged();
            }
        }

        public string AcademicYear
        {
            get => _academicYear;
            set
            {
                _academicYear = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ActivityItem> RecentActivities { get; set; }

        public ICommand AddStudentCommand { get; }
        public ICommand AddTeacherCommand { get; }
        public ICommand CreateClassCommand { get; }

        // Event to notify MainWindowViewModel to navigate
        public event Action<string> NavigateRequested;

        public DashboardViewModel()
        {
            _studentService = StudentService.Instance;
            _classService = ClassService.Instance;
            _teacherService = TeacherService.Instance;

            // Load real counts
            LoadStudentCount();
            LoadTeacherCount();
            LoadClassCount();

            // Initialize other properties
            AcademicYear = "2024-2025";

            // Initialize recent activities
            RecentActivities = new ObservableCollection<ActivityItem>
            {
                new ActivityItem
                {
                    Description = "New student John Doe enrolled in Grade 10A",
                    TimeAgo = "2 hours ago",
                    ActivityType = ActivityType.Student
                },
                new ActivityItem
                {
                    Description = "Teacher Sarah Johnson assigned to Math class",
                    TimeAgo = "4 hours ago",
                    ActivityType = ActivityType.Teacher
                },
                new ActivityItem
                {
                    Description = "Grade 9B schedule updated",
                    TimeAgo = "1 day ago",
                    ActivityType = ActivityType.Schedule
                }
            };

            // Initialize commands
            AddStudentCommand = new RelayCommand(ExecuteAddStudent);
            AddTeacherCommand = new RelayCommand(ExecuteAddTeacher);
            CreateClassCommand = new RelayCommand(ExecuteCreateClass);
        }

        public void LoadStudentCount()
        {
            TotalStudents = _studentService.GetStudentCount();
        }

        public void LoadTeacherCount()
        {
            TotalTeachers = _teacherService.GetTeacherCount();
        }

        public void LoadClassCount()
        {
            TotalClasses = _classService.GetClassCount();
        }

        private void ExecuteAddStudent(object parameter)
        {
            System.Windows.MessageBox.Show("Navigation to Add Student page will be impment next time", "Info",
               System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }

        private void ExecuteAddTeacher(object parameter)
        {
            System.Windows.MessageBox.Show("Navigation to Add Teacher page will be impment next time", "Info",
               System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }

        private void ExecuteCreateClass(object parameter)
        {
            System.Windows.MessageBox.Show("Navigation to Create class page will be impment next time", "Info",
               System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ActivityItem
    {
        public string Description { get; set; }
        public string TimeAgo { get; set; }
        public ActivityType ActivityType { get; set; }
    }

    public enum ActivityType
    {
        Student,
        Teacher,
        Schedule
    }
}
