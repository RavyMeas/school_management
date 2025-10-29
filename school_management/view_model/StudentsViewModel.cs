using school_management.Commands;
using school_management.model;
using school_management.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace school_management.view_model
{
    public class StudentsViewModel : INotifyPropertyChanged
    {
        private readonly StudentService _studentService;
        private readonly ClassService _classService;
        private ObservableCollection<Student> _students;
        private string _searchText;
        private bool _isAddDialogOpen;
        private bool _isEditDialogOpen;
        private Student _newStudent;
        private Student _editStudent;
        private string _selectedGradeForNew;
        private string _selectedGradeForEdit;

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

        public string SelectedGradeForNew
        {
            get => _selectedGradeForNew;
            set
            {
                _selectedGradeForNew = value;
                OnPropertyChanged();

                // Update NewStudent.Grade when grade is selected
                if (NewStudent != null)
                {
                    NewStudent.Grade = value;
                }

                LoadAvailableClassesForNewStudent();
            }
        }

        public string SelectedGradeForEdit
        {
            get => _selectedGradeForEdit;
            set
            {
                _selectedGradeForEdit = value;
                OnPropertyChanged();

                // Update EditStudent.Grade when grade is selected
                if (EditStudent != null)
                {
                    EditStudent.Grade = value;
                }

                LoadAvailableClassesForEditStudent();
            }
        }

        public int StudentCount => Students?.Count ?? 0;

        public ObservableCollection<string> Grades { get; set; }
        public ObservableCollection<string> AvailableClasses { get; set; }
        public ObservableCollection<string> AvailableClassesForEdit { get; set; }
        public ObservableCollection<string> GenderOptions { get; set; }
        public ObservableCollection<string> StatusOptions { get; set; }

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
            _classService = ClassService.Instance;

            // Initialize collections
            Grades = new ObservableCollection<string>
            {
                "Grade 9", "Grade 10", "Grade 11", "Grade 12"
            };

            AvailableClasses = new ObservableCollection<string>();
            AvailableClassesForEdit = new ObservableCollection<string>();

            GenderOptions = new ObservableCollection<string>
            {
                "Male", "Female", "Other"
            };

            StatusOptions = new ObservableCollection<string>
            {
                "Active", "Inactive", "Graduated", "Transferred"
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
            try
            {
                var students = _studentService.GetAllStudents();
                Students = new ObservableCollection<Student>(students);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading students: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Students = new ObservableCollection<Student>();
            }
        }

        private void SearchStudents()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(SearchText))
                {
                    LoadStudents();
                }
                else
                {
                    var students = _studentService.SearchStudents(SearchText);
                    Students = new ObservableCollection<Student>(students);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching students: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadAvailableClassesForNewStudent()
        {
            AvailableClasses.Clear();

            if (string.IsNullOrWhiteSpace(SelectedGradeForNew))
                return;

            // Get all classes for the selected grade
            var classes = _classService.GetAllClasses()
                .Where(c => c.Grade == SelectedGradeForNew)
                .OrderBy(c => c.ClassName)
                .ToList();

            foreach (var classItem in classes)
            {
                AvailableClasses.Add(classItem.ClassName);
            }

            // If no class selected yet and there are available classes, don't auto-select
            // Let user choose
        }

        private void LoadAvailableClassesForEditStudent()
        {
            AvailableClassesForEdit.Clear();

            if (string.IsNullOrWhiteSpace(SelectedGradeForEdit))
                return;

            // Get all classes for the selected grade
            var classes = _classService.GetAllClasses()
                .Where(c => c.Grade == SelectedGradeForEdit)
                .OrderBy(c => c.ClassName)
                .ToList();

            foreach (var classItem in classes)
            {
                AvailableClassesForEdit.Add(classItem.ClassName);
            }
        }

        private void ExecuteOpenAddDialog(object parameter)
        {
            NewStudent = new Student
            {
                Status = "Active",
                EnrollmentDate = DateTime.Now,
    
            };

            // Set default grade to trigger class loading
            SelectedGradeForNew = Grades.FirstOrDefault();

            if (NewStudent != null && !string.IsNullOrWhiteSpace(SelectedGradeForNew))
            {
                NewStudent.Grade = SelectedGradeForNew;
            }

            IsAddDialogOpen = true;
        }

        private void ExecuteCloseAddDialog(object parameter)
        {
            IsAddDialogOpen = false;
            NewStudent = null;
            SelectedGradeForNew = null;
            AvailableClasses.Clear();
        }

        private bool CanExecuteAddStudent(object parameter)
        {
            return NewStudent != null &&
                   !string.IsNullOrWhiteSpace(NewStudent.FirstName) &&
                   !string.IsNullOrWhiteSpace(NewStudent.LastName) &&
                   !string.IsNullOrWhiteSpace(NewStudent.Email) &&
                   !string.IsNullOrWhiteSpace(NewStudent.Grade);
        }

        private void ExecuteAddStudent(object parameter)
        {
            try
            {
                // Validate class selection
                if (!string.IsNullOrWhiteSpace(NewStudent.Class))
                {
                    var selectedClass = _classService.GetAllClasses()
                        .FirstOrDefault(c => c.ClassName == NewStudent.Class);

                    if (selectedClass == null)
                    {
                        MessageBox.Show("Selected class does not exist!", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    // Check if class is full
                    if (selectedClass.CurrentEnrollment >= selectedClass.Capacity)
                    {
                        var result = MessageBox.Show(
                            $"Class {selectedClass.ClassName} is full ({selectedClass.CurrentEnrollment}/{selectedClass.Capacity}).\nDo you want to add the student without assigning a class?",
                            "Class Full",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Warning);

                        if (result == MessageBoxResult.Yes)
                        {
                            NewStudent.Class = null;
                        }
                        else
                        {
                            return;
                        }
                    }

                    // Verify grade matches
                    if (selectedClass != null && selectedClass.Grade != NewStudent.Grade)
                    {
                        MessageBox.Show(
                            $"Class grade ({selectedClass.Grade}) does not match student grade ({NewStudent.Grade})!",
                            "Validation Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);
                        return;
                    }
                }

                // Add student (will auto-enroll in class if specified)
                var success = _studentService.AddStudent(NewStudent);

                if (success)
                {
                    LoadStudents();
                    IsAddDialogOpen = false;
                    //MessageBox.Show("Student added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Failed to add student!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
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
                    EnrollmentDate = student.EnrollmentDate,
                   
                };

                // Set selected grade to load available classes
                SelectedGradeForEdit = student.Grade;

                IsEditDialogOpen = true;
            }
        }

        private void ExecuteCloseEditDialog(object parameter)
        {
            IsEditDialogOpen = false;
            EditStudent = null;
            SelectedGradeForEdit = null;
            AvailableClassesForEdit.Clear();
        }

        private bool CanExecuteEditStudent(object parameter)
        {
            return EditStudent != null &&
                   !string.IsNullOrWhiteSpace(EditStudent.FirstName) &&
                   !string.IsNullOrWhiteSpace(EditStudent.LastName) &&
                   !string.IsNullOrWhiteSpace(EditStudent.Email) &&
                   !string.IsNullOrWhiteSpace(EditStudent.Grade);
        }

        private void ExecuteEditStudent(object parameter)
        {
            try
            {
                // Validate class selection if provided
                if (!string.IsNullOrWhiteSpace(EditStudent.Class))
                {
                    var selectedClass = _classService.GetAllClasses()
                        .FirstOrDefault(c => c.ClassName == EditStudent.Class);

                    if (selectedClass == null)
                    {
                        MessageBox.Show("Selected class does not exist!", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    // Verify grade matches
                    if (selectedClass.Grade != EditStudent.Grade)
                    {
                        MessageBox.Show(
                            $"Class grade ({selectedClass.Grade}) does not match student grade ({EditStudent.Grade})!",
                            "Validation Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);
                        return;
                    }

                    // Check capacity only if enrolling in a different class
                    var currentStudent = _studentService.GetStudentById(EditStudent.Id);
                    if (currentStudent?.Class != EditStudent.Class)
                    {
                        if (selectedClass.CurrentEnrollment >= selectedClass.Capacity)
                        {
                            MessageBox.Show(
                                $"Class {selectedClass.ClassName} is full ({selectedClass.CurrentEnrollment}/{selectedClass.Capacity})!",
                                "Class Full",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                            return;
                        }
                    }
                }

                // Update student
                var success = _studentService.UpdateStudent(EditStudent);

                if (success)
                {
                    LoadStudents();
                    IsEditDialogOpen = false;
                    //MessageBox.Show("Student updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Failed to update student!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
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
                try
                {
                    // Check if student is enrolled in any classes
                    var enrolledClasses = _studentService.GetStudentClasses(student.Id);

                    string message = enrolledClasses.Any()
                        ? $"Student {student.FullName} is enrolled in {enrolledClasses.Count} class(es).\nAre you sure you want to delete this student?"
                        : $"Are you sure you want to delete {student.FullName}?";

                    var result = MessageBox.Show(
                        message,
                        "Confirm Delete",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        var success = _studentService.DeleteStudent(student.Id);

                        if (success)
                        {
                            LoadStudents();
                            //MessageBox.Show("Student deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("Failed to delete student!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting student: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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