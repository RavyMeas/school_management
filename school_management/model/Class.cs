using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace school_management.model
{
    public class ClassRoom : INotifyPropertyChanged  // Changed from "Class" to "ClassRoom"
    {
        private int _id;
        private string _className;
        private string _grade;
        private string _homeroom;
        private int _capacity;
        private int _teacherId;
        private string _teacherName;
        private int _currentEnrollment;
        private string _academicYear;
        private string _term;

        public int Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged();
            }
        }

        public string ClassName
        {
            get => _className;
            set
            {
                _className = value;
                OnPropertyChanged();
            }
        }

        public string Grade
        {
            get => _grade;
            set
            {
                _grade = value;
                OnPropertyChanged();
            }
        }

        public string Homeroom
        {
            get => _homeroom;
            set
            {
                _homeroom = value;
                OnPropertyChanged();
            }
        }

        public int Capacity
        {
            get => _capacity;
            set
            {
                _capacity = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(EnrollmentDisplay));
                OnPropertyChanged(nameof(EnrollmentPercentage));
            }
        }

        public int TeacherId
        {
            get => _teacherId;
            set
            {
                _teacherId = value;
                OnPropertyChanged();
            }
        }

        public string TeacherName
        {
            get => _teacherName;
            set
            {
                _teacherName = value;
                OnPropertyChanged();
            }
        }

        public int CurrentEnrollment
        {
            get => _currentEnrollment;
            set
            {
                _currentEnrollment = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(EnrollmentDisplay));
                OnPropertyChanged(nameof(EnrollmentPercentage));
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

        public string Term
        {
            get => _term;
            set
            {
                _term = value;
                OnPropertyChanged();
            }
        }

        // Computed Properties - Add setters for WPF binding
        public string EnrollmentDisplay => $"{CurrentEnrollment}/{Capacity}";

        public double EnrollmentPercentage => Capacity > 0 ? (double)CurrentEnrollment / Capacity * 100 : 0;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}