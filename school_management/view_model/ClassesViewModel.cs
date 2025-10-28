using school_management.Commands;
using school_management.model;
using school_management.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace school_management.view_model
{
    public class ClassesViewModel : INotifyPropertyChanged
    {
        private readonly ClassService _classService;
        private readonly TeacherService _teacherService;
        private ObservableCollection<ClassRoom> _classes;
        private string _searchText;
        private bool _isAddDialogOpen;
        private bool _isEditDialogOpen;
        private ClassRoom _newClass;
        private ClassRoom _selectedClass;

        public ObservableCollection<ClassRoom> Classes
        {
            get => _classes;
            set
            {
                _classes = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ClassCount));
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                SearchClasses();
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

        public ClassRoom NewClass
        {
            get => _newClass;
            set
            {
                _newClass = value;
                OnPropertyChanged();
            }
        }

        public ClassRoom SelectedClass
        {
            get => _selectedClass;
            set
            {
                _selectedClass = value;
                OnPropertyChanged();
            }
        }

        public int ClassCount => Classes?.Count ?? 0;

        public ObservableCollection<string> Grades { get; set; }
        public ObservableCollection<Teacher> Teachers { get; set; }
        public ObservableCollection<string> AcademicYears { get; set; }
        public ObservableCollection<string> Terms { get; set; }

        public ICommand OpenAddDialogCommand { get; }
        public ICommand CloseAddDialogCommand { get; }
        public ICommand OpenEditDialogCommand { get; }
        public ICommand CloseEditDialogCommand { get; }
        public ICommand AddClassCommand { get; }
        public ICommand UpdateClassCommand { get; }
        public ICommand DeleteClassCommand { get; }

        public ClassesViewModel()
        {
            _classService = ClassService.Instance;
            _teacherService = TeacherService.Instance;

            // Initialize collections
            Grades = new ObservableCollection<string>
            {
                "Grade 9", "Grade 10", "Grade 11", "Grade 12"
            };

            Teachers = new ObservableCollection<Teacher>(_teacherService.GetAllTeachers());

            AcademicYears = new ObservableCollection<string>
            {
                "2024-2025", "2025-2026", "2026-2027"
            };

            Terms = new ObservableCollection<string>
            {
                "Semester 1", "Semester 2", "Summer"
            };

            // Initialize commands
            OpenAddDialogCommand = new RelayCommand(ExecuteOpenAddDialog);
            CloseAddDialogCommand = new RelayCommand(ExecuteCloseAddDialog);
            OpenEditDialogCommand = new RelayCommand(ExecuteOpenEditDialog);
            CloseEditDialogCommand = new RelayCommand(ExecuteCloseEditDialog);
            AddClassCommand = new RelayCommand(ExecuteAddClass, CanExecuteAddClass);
            UpdateClassCommand = new RelayCommand(ExecuteUpdateClass, CanExecuteUpdateClass);
            DeleteClassCommand = new RelayCommand(ExecuteDeleteClass);

            // Load classes
            LoadClasses();
        }

        private void LoadClasses()
        {
            var classes = _classService.GetAllClasses();
            Classes = new ObservableCollection<ClassRoom>(classes);
        }

        private void SearchClasses()
        {
            var classes = _classService.SearchClasses(SearchText);
            Classes = new ObservableCollection<ClassRoom>(classes);
        }

        private void ExecuteOpenAddDialog(object parameter)
        {
            NewClass = new ClassRoom
            {
                CurrentEnrollment = 0,
                AcademicYear = "2024-2025",
                Term = "Semester 1"
            };
            IsAddDialogOpen = true;
        }

        private void ExecuteCloseAddDialog(object parameter)
        {
            IsAddDialogOpen = false;
            NewClass = null;
        }

        private void ExecuteOpenEditDialog(object parameter)
        {
            if (parameter is ClassRoom classItem)
            {
                // Create a copy of the class for editing
                SelectedClass = new ClassRoom
                {
                    Id = classItem.Id,
                    ClassName = classItem.ClassName,
                    Grade = classItem.Grade,
                    Homeroom = classItem.Homeroom,
                    Capacity = classItem.Capacity,
                    CurrentEnrollment = classItem.CurrentEnrollment,
                    TeacherId = classItem.TeacherId,
                    TeacherName = classItem.TeacherName,
                    AcademicYear = classItem.AcademicYear,
                    Term = classItem.Term
                };
                IsEditDialogOpen = true;
            }
        }

        private void ExecuteCloseEditDialog(object parameter)
        {
            IsEditDialogOpen = false;
            SelectedClass = null;
        }

        private bool CanExecuteAddClass(object parameter)
        {
            return NewClass != null &&
                   !string.IsNullOrWhiteSpace(NewClass.ClassName) &&
                   !string.IsNullOrWhiteSpace(NewClass.Grade) &&
                   !string.IsNullOrWhiteSpace(NewClass.Homeroom) &&
                   NewClass.Capacity > 0 &&
                   NewClass.TeacherId > 0;
        }

        private void ExecuteAddClass(object parameter)
        {
            try
            {
                // Set teacher name
                var teacher = _teacherService.GetTeacherById(NewClass.TeacherId);
                if (teacher != null)
                {
                    NewClass.TeacherName = teacher.FullName;
                }

                _classService.AddClass(NewClass);
                _classService.AssignTeacherToClass(NewClass.TeacherId, NewClass.Id);

                LoadClasses();
                IsAddDialogOpen = false;
               // MessageBox.Show("Class added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding class: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanExecuteUpdateClass(object parameter)
        {
            return SelectedClass != null &&
                   !string.IsNullOrWhiteSpace(SelectedClass.ClassName) &&
                   !string.IsNullOrWhiteSpace(SelectedClass.Grade) &&
                   !string.IsNullOrWhiteSpace(SelectedClass.Homeroom) &&
                   SelectedClass.Capacity > 0 &&
                   SelectedClass.TeacherId > 0;
        }

        private void ExecuteUpdateClass(object parameter)
        {
            try
            {
                // Set teacher name
                var teacher = _teacherService.GetTeacherById(SelectedClass.TeacherId);
                if (teacher != null)
                {
                    SelectedClass.TeacherName = teacher.FullName;
                }

                _classService.UpdateClass(SelectedClass);

                LoadClasses();
                IsEditDialogOpen = false;
                //MessageBox.Show("Class updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating class: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteDeleteClass(object parameter)
        {
            if (parameter is ClassRoom classItem)
            {
                var result = MessageBox.Show(
                    $"Are you sure you want to delete class {classItem.ClassName}?",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _classService.DeleteClass(classItem.Id);
                    LoadClasses();
                    //MessageBox.Show("Class deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
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
