using school_management.Commands;
using school_management.model;
using school_management.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace school_management.view_model
{
    public class StudentsViewModel : INotifyPropertyChanged
    {
        private readonly StudentService _studentService;
        private ObservableCollection<Student> _students;
        private string _searchText;
        private bool _isAddDialogOpen;
        private bool _isEditDialogOpen;
        private Student _newStudent;
        private Student _editStudent;

        public ObservableCollection<Student> Students
        {
            get => _students;
            set
            {
                _students = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(StudentCount));
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                SearchStudents();
            }
        }

        public bool IsAddDialogOpen
        {
            get => _isAddDialogOpen;
            set
            {
                _isAddDialogOpen = value;
                OnPropertyChanged();
            }
        }

        public bool IsEditDialogOpen
        {
            get => _isEditDialogOpen;
            set
            {
                _isEditDialogOpen = value;
                OnPropertyChanged();
            }
        }

        public Student NewStudent
        {
            get => _newStudent;
            set
            {
                _newStudent = value;
                OnPropertyChanged();
            }
        }

        public Student EditStudent
        {
            get => _editStudent;
            set
            {
                _editStudent = value;
                OnPropertyChanged();
            }
        }

        public int StudentCount => Students?.Count ?? 0;

        public ObservableCollection<string> Grades { get; set; }
        public ObservableCollection<string> Classes { get; set; }

        public ICommand OpenAddDialogCommand { get; }
        public ICommand CloseAddDialogCommand { get; }
        public ICommand AddStudentCommand { get; }
        public ICommand OpenEditDialogCommand { get; }
        public ICommand CloseEditDialogCommand { get; }
        public ICommand EditStudentCommand { get; }
        public ICommand DeleteStudentCommand { get; }

        public StudentsViewModel()
        {
            _studentService = StudentService.Instance;

            // Initialize collections
            Grades = new ObservableCollection<string>
            {
                "Grade 9", "Grade 10", "Grade 11", "Grade 12"
            };

            Classes = new ObservableCollection<string>
            {
                "9A", "9B", "10A", "10B", "11A", "11B", "12A", "12B"
            };

            // Initialize commands
            OpenAddDialogCommand = new RelayCommand(ExecuteOpenAddDialog);
            CloseAddDialogCommand = new RelayCommand(ExecuteCloseAddDialog);
            AddStudentCommand = new RelayCommand(ExecuteAddStudent, CanExecuteAddStudent);
            OpenEditDialogCommand = new RelayCommand(ExecuteOpenEditDialog);
            CloseEditDialogCommand = new RelayCommand(ExecuteCloseEditDialog);
            EditStudentCommand = new RelayCommand(ExecuteEditStudent, CanExecuteEditStudent);
            DeleteStudentCommand = new RelayCommand(ExecuteDeleteStudent);

            // Load students
            LoadStudents();
        }

        private void LoadStudents()
        {
            var students = _studentService.GetAllStudents();
            Students = new ObservableCollection<Student>(students);
        }

        private void SearchStudents()
        {
            var students = _studentService.SearchStudents(SearchText);
            Students = new ObservableCollection<Student>(students);
        }

        private void ExecuteOpenAddDialog(object parameter)
        {
            NewStudent = new Student
            {
                Status = "Active",
                EnrollmentDate = DateTime.Now
            };
            IsAddDialogOpen = true;
        }

        private void ExecuteCloseAddDialog(object parameter)
        {
            IsAddDialogOpen = false;
            NewStudent = null;
        }

        private bool CanExecuteAddStudent(object parameter)
        {
            return NewStudent != null &&
                   !string.IsNullOrWhiteSpace(NewStudent.FirstName) &&
                   !string.IsNullOrWhiteSpace(NewStudent.LastName) &&
                   !string.IsNullOrWhiteSpace(NewStudent.Email) &&
                   !string.IsNullOrWhiteSpace(NewStudent.Grade) &&
                   !string.IsNullOrWhiteSpace(NewStudent.Class);
        }

        private void ExecuteAddStudent(object parameter)
        {
            try
            {
                _studentService.AddStudent(NewStudent);
                LoadStudents();
                IsAddDialogOpen = false;
                //MessageBox.Show("Student added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding student: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteOpenEditDialog(object parameter)
        {
            if (parameter is Student student)
            {
             
                EditStudent = new Student
                {
                    Id = student.Id,
                    FirstName = student.FirstName,
                    LastName = student.LastName,
                    Email = student.Email,
                    Grade = student.Grade,
                    Class = student.Class,
                    Status = student.Status,
                    EnrollmentDate = student.EnrollmentDate
                };
                IsEditDialogOpen = true;
            }
        }

        private void ExecuteCloseEditDialog(object parameter)
        {
            IsEditDialogOpen = false;
            EditStudent = null;
        }

        private bool CanExecuteEditStudent(object parameter)
        {
            return EditStudent != null &&
                   !string.IsNullOrWhiteSpace(EditStudent.FirstName) &&
                   !string.IsNullOrWhiteSpace(EditStudent.LastName) &&
                   !string.IsNullOrWhiteSpace(EditStudent.Email) &&
                   !string.IsNullOrWhiteSpace(EditStudent.Grade) &&
                   !string.IsNullOrWhiteSpace(EditStudent.Class);
        }

        private void ExecuteEditStudent(object parameter)
        {
            try
            {
                _studentService.UpdateStudent(EditStudent);
                LoadStudents();
                IsEditDialogOpen = false;
               // MessageBox.Show("Student updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating student: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteDeleteStudent(object parameter)
        {
            if (parameter is Student student)
            {
                var result = MessageBox.Show(
                    $"Are you sure you want to delete {student.FullName}?",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _studentService.DeleteStudent(student.Id);
                    LoadStudents();
                    //MessageBox.Show("Student deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}