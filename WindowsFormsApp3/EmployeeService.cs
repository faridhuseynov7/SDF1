using System.Collections.Generic;

namespace WindowsFormsApp3
{
    public class EmployeeService : EmployeeServiceBase
    {
        
        public static List<Employee> Employees = new List<Employee>();

        public override bool AddEmployee(Employee employee)
        {
            if (employee == null || string.IsNullOrWhiteSpace(employee.Code))
                return false;

            if (FindEmployee(employee.Code) != null)
                return false;

            Employees.Add(employee);
            return true;
        }

        public override Employee FindEmployee(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return null;

            return Employees.Find(e => e.Code.Trim().ToLower() == code.Trim().ToLower());
        }

        public override bool AddPayment(string employeeCode, string title, double amount)
        {
            Employee emp = FindEmployee(employeeCode);
            if (emp == null || amount <= 0 || string.IsNullOrWhiteSpace(title))
                return false;

            emp.Payments.Add(new Payment(title, amount));
            return true;
        }

        public override List<Payment> GetEmployeePayments(string employeeCode)
        {
            Employee emp = FindEmployee(employeeCode);
            if (emp == null)
                return null;

            return emp.Payments;
        }

        public override List<Employee> GetAllEmployees()
        {
            return Employees;
        }
    }
}