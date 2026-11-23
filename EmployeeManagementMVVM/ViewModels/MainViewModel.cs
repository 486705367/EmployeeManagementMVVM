using EmployeeManagementMVVM.Commands;
using EmployeeManagementMVVM.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace EmployeeManagementMVVM.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private string _name = "";
        private string _age = "";
        private string _department = "";

        private string _salary = "";

        private string _searchText = "";
        private string _filterDepartment = "All";
        private Employee? _selectedEmployee;
        private int _nextId = 1;

        public ObservableCollection<Employee> Employees { get; } = new ObservableCollection<Employee>();

        public ICollectionView EmployeesView { get; }

        public List<string> Departments { get; } = new List<string> { "HR", "IT", "Finance", "Testing", "Sales" };

        public List<string> FilterDepartments { get; } = new List<string> { "All", "HR", "IT", "Finance", "Testing", "Sales" };

        public string Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged(); }
        }

        public string Age
        {
            get { return _age; }
            set { _age = value; OnPropertyChanged(); }
        }

        public string Department
        {
            get { return _department; }
            set { _department = value; OnPropertyChanged(); }
        }

        public string Salary
        {
            get { return _salary; }
            set { _salary = value ; OnPropertyChanged(); }
        }

        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                OnPropertyChanged();
                RefreshList();
            }
        }

        public string FilterDepartment
        {
            get { return _filterDepartment; }
            set
            {
                _filterDepartment = value;
                OnPropertyChanged();
                RefreshList();
            }
        }

        public Employee? SelectedEmployee
        {
            get { return _selectedEmployee; }
            set { _selectedEmployee = value; OnPropertyChanged(); }
        }

        public int TotalCount
        {
            get { return EmployeesView.Cast<object>().Count(); }
        }

        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }

        public MainViewModel()
        {
            EmployeesView = CollectionViewSource.GetDefaultView(Employees);
            EmployeesView.Filter = FilterEmployees;

            AddCommand = new RelayCommand(AddEmployee);
            DeleteCommand = new RelayCommand(DeleteEmployee, CanDeleteEmployee);
        }

        private bool FilterEmployees(object item)
        {
            Employee employee = (Employee)item;

            bool nameMatches = string.IsNullOrWhiteSpace(SearchText)
                || employee.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase);

            bool departmentMatches = FilterDepartment == "All"
                || employee.Department == FilterDepartment;

            return nameMatches && departmentMatches;
        }

        private void RefreshList()
        {
            EmployeesView.Refresh();
            OnPropertyChanged(nameof(TotalCount));
        }

        private void AddEmployee(object? parameter)
        {
            if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Department))
            {
                MessageBox.Show("Please enter name and select department,");
                return;
            }

            if (!int.TryParse(Age, out int ageNumber) || ageNumber <= 0)
            {
                MessageBox.Show("Please enter a valid age.");
                return;
            }

            if (!decimal.TryParse(Salary, out decimal salaryNumber) || salaryNumber < 0)
            {
                MessageBox.Show("Please enter valid salary. ");
            }

            Employees.Add(new Employee
            {
                Id = _nextId++,
                Name = Name,
                Age = ageNumber,
                Department = Department,
                Salary = salaryNumber
            });

            Name = "";
            Age = "";
            Department = "";
            Salary = "";
            RefreshList();
        }

        private bool CanDeleteEmployee(object? parameter)
        {
            return SelectedEmployee != null;
        }

        private void DeleteEmployee(object? parameter)
        {
            if (SelectedEmployee == null)
            {
                return;
            }

            Employees.Remove(SelectedEmployee);
            SelectedEmployee = null;
            RefreshList();
        }
    }
}