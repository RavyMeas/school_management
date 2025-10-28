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
    public class TeachersViewModel : INotifyPropertyChanged
    {
        private readonly TeacherService _teacherService;
        private ObservableCollection<Teacher> _teachers;
        private string _searchText;
        private bool _isAddDialogOpen;
        private bool _isEditDialogOpen;
        private Teacher _newTeacher;
        private Teacher _selectedTeacher;

        public ObservableCollection<Teacher> Teachers
        {
            get => _teachers;
            set
            {
                _teachers = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TeacherCount));
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                SearchTeachers();
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

        public Teacher NewTeacher
        {
            get => _newTeacher;
            set
            {
                _newTeacher = value;
                OnPropertyChanged();
            }
        }

        public Teacher SelectedTeacher
        {
            get => _selectedTeacher;
            set
            {
                _selectedTeacher = value;
                OnPropertyChanged();
            }
        }

        public int TeacherCount => Teachers?.Count ?? 0;

        public ObservableCollection<string> Subjects { get; set; }
        public ObservableCollection<string> Departments { get; set; }

        public ICommand OpenAddDialogCommand { get; }
        public ICommand CloseAddDialogCommand { get; }
        public ICommand OpenEditDialogCommand { get; }
        public ICommand CloseEditDialogCommand { get; }
        public ICommand AddTeacherCommand { get; }
        public ICommand UpdateTeacherCommand { get; }
        public ICommand DeleteTeacherCommand { get; }

        public TeachersViewModel()
        {
            _teacherService = TeacherService.Instance;

            // Initialize collections
            Subjects = new ObservableCollection<string>
            {
                "Mathematics",
                "English Literature",
                "Physics",
                "Chemistry",
                "Biology",
                "History",
                "Geography",
                "Computer Science",
                "Physical Education",
                "Art",
                "Music"
            };

            Departments = new ObservableCollection<string>
            {
                "Science",
                "Humanities",
                "Mathematics",
                "Languages",
                "Arts",
                "Physical Education"
            };

            // Initialize commands
            OpenAddDialogCommand = new RelayCommand(ExecuteOpenAddDialog);
            CloseAddDialogCommand = new RelayCommand(ExecuteCloseAddDialog);
            OpenEditDialogCommand = new RelayCommand(ExecuteOpenEditDialog);
            CloseEditDialogCommand = new RelayCommand(ExecuteCloseEditDialog);
            AddTeacherCommand = new RelayCommand(ExecuteAddTeacher, CanExecuteAddTeacher);
            UpdateTeacherCommand = new RelayCommand(ExecuteUpdateTeacher, CanExecuteUpdateTeacher);
            DeleteTeacherCommand = new RelayCommand(ExecuteDeleteTeacher);

            // Load teachers
            LoadTeachers();
        }

        private void LoadTeachers()
        {
            var teachers = _teacherService.GetAllTeachers();
            Teachers = new ObservableCollection<Teacher>(teachers);
        }

        private void SearchTeachers()
        {
            var teachers = _teacherService.SearchTeachers(SearchText);
            Teachers = new ObservableCollection<Teacher>(teachers);
        }

        private void ExecuteOpenAddDialog(object parameter)
        {
            NewTeacher = new Teacher
            {
                Status = "Active",
                HireDate = DateTime.Now
            };
            IsAddDialogOpen = true;
        }

        private void ExecuteCloseAddDialog(object parameter)
        {
            IsAddDialogOpen = false;
            NewTeacher = null;
        }

        private void ExecuteOpenEditDialog(object parameter)
        {
            if (parameter is Teacher teacher)
            {
                // Create a copy of the teacher for editing
                SelectedTeacher = new Teacher
                {
                    Id = teacher.Id,
                    FirstName = teacher.FirstName,
                    LastName = teacher.LastName,
                    Email = teacher.Email,
                    Phone = teacher.Phone,
                    Department = teacher.Department,
                    Subject = teacher.Subject,
                    Status = teacher.Status,
                    HireDate = teacher.HireDate
                };
                IsEditDialogOpen = true;
            }
        }

        private void ExecuteCloseEditDialog(object parameter)
        {
            IsEditDialogOpen = false;
            SelectedTeacher = null;
        }

        private bool CanExecuteAddTeacher(object parameter)
        {
            return NewTeacher != null &&
                   !string.IsNullOrWhiteSpace(NewTeacher.FirstName) &&
                   !string.IsNullOrWhiteSpace(NewTeacher.LastName) &&
                   !string.IsNullOrWhiteSpace(NewTeacher.Email) &&
                   !string.IsNullOrWhiteSpace(NewTeacher.Subject) &&
                   !string.IsNullOrWhiteSpace(NewTeacher.Department);
        }

        private void ExecuteAddTeacher(object parameter)
        {
            try
            {
                _teacherService.AddTeacher(NewTeacher);
                LoadTeachers();
                IsAddDialogOpen = false;
                //MessageBox.Show("Teacher added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding teacher: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanExecuteUpdateTeacher(object parameter)
        {
            return SelectedTeacher != null &&
                   !string.IsNullOrWhiteSpace(SelectedTeacher.FirstName) &&
                   !string.IsNullOrWhiteSpace(SelectedTeacher.LastName) &&
                   !string.IsNullOrWhiteSpace(SelectedTeacher.Email) &&
                   !string.IsNullOrWhiteSpace(SelectedTeacher.Subject) &&
                   !string.IsNullOrWhiteSpace(SelectedTeacher.Department);
        }

        private void ExecuteUpdateTeacher(object parameter)
        {
            try
            {
                _teacherService.UpdateTeacher(SelectedTeacher);
                LoadTeachers();
                IsEditDialogOpen = false;
                //MessageBox.Show("Teacher updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating teacher: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteDeleteTeacher(object parameter)
        {
            if (parameter is Teacher teacher)
            {
                var result = MessageBox.Show(
                    $"Are you sure you want to delete {teacher.FullName}?",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _teacherService.DeleteTeacher(teacher.Id);
                    LoadTeachers();
                   // MessageBox.Show("Teacher deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
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
