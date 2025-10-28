using school_management.Commands;
using school_management.model;
using school_management.view;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace school_management.view_model
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private UserControl _currentView;
        private string _selectedMenu;

        public UserControl CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        public string SelectedMenu
        {
            get => _selectedMenu;
            set
            {
                _selectedMenu = value;
                OnPropertyChanged();
            }
        }

        public ICommand NavigateCommand { get; }

        public MainWindowViewModel()
        {
            NavigateCommand = new RelayCommand(ExecuteNavigate);

            // Set default view to Dashboard
            SelectedMenu = "Dashboard";
            CurrentView = new dasboard();
        }

        private void ExecuteNavigate(object parameter)
        {
            if (parameter is string menuName)
            {
                SelectedMenu = menuName;

                switch (menuName)
                {
                    case "Dashboard":
                        CurrentView = new dasboard();
                        break;
                    case "Students":
                        CurrentView = new student_view();
                        break;
                    case "Teachers":
                        CurrentView = new TeacherView();
                        break;
                    case "Parents":
                        CurrentView = new NotImplementedView("Parents Managemnt");
                        break;
                    case "Classes":
                        CurrentView = new ClassView();
                        break;
                    case "Subjects":
                        CurrentView = new NotImplementedView("Subjects Managemnt");
                        break;
                    case "Courses":
                        CurrentView = new NotImplementedView("Course Managemnt");
                        break;
                    case "Enrollment":
                        CurrentView = new NotImplementedView("Enrollment Managemnt");
                        break;
                    case "Timetable":
                        CurrentView = new NotImplementedView("Timetable Managemnt");
                        break;
                    case "Academic Year":
                        CurrentView = new NotImplementedView("Academic Year Managemnt");
                        break;
                    case "Attendance":
                        CurrentView = new NotImplementedView("Attendance Managemnt");
                        break;
                    case "Grades":
                        CurrentView = new NotImplementedView("Grades Managemnt");
                        break;
                    case "Fee Management":
                        CurrentView = new NotImplementedView("Fee Management Managemnt");
                        break;
                    case "Payments":
                        CurrentView = new NotImplementedView("Payments Managemnt");
                        break;
                    default:
                        CurrentView = new dasboard();
                        break;
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
