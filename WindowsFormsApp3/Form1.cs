using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace WindowsFormsApp3
{
    public partial class Form1 : Form
    {
        private EmployeeServiceBase service = new EmployeeService();

        public Form1()
        {
            InitializeComponent();

            this.Load += Form1_Load;

            if (btnAddEmployee != null) btnAddEmployee.Click += btnAddEmployee_Click;
            if (btnAddPayment != null) btnAddPayment.Click += btnAddPayment_Click;
            if (btnSearch != null) btnSearch.Click += btnSearch_Click;
            if (btnExit != null) btnExit.Click += btnExit_Click;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            RefreshEmployeeViews();
        }

        private void btnAddEmployee_Click(object sender, EventArgs e)
        {
            string code = txtCode.Text.Trim();
            string firstName = txtFirstName.Text.Trim();
            string lastName = txtLastName.Text.Trim();

            if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
            {
                MessageBox.Show("Zəhmət olmasa bütün xanaları doldurun!", "Xəbərdarlıq", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txtAge.Text.Trim(), out int age) || age <= 0)
            {
                MessageBox.Show("Düzgün yaş daxil edin!", "Xəta", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Employee newEmp = new Employee(code, firstName, lastName, age);
            bool isAdded = service.AddEmployee(newEmp);

            if (isAdded)
            {
                MessageBox.Show("İşçi uğurla əlavə edildi!", "Uğur", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearEmployeeInputs();
                RefreshEmployeeViews();
            }
            else
            {
                MessageBox.Show("Bu koda malik işçi artıq mövcuddur!", "Xəbərdarlıq", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnAddPayment_Click(object sender, EventArgs e)
        {
            if (cmbEmployees.SelectedItem == null)
            {
                MessageBox.Show("Zəhmət olmasa işçi seçin!", "Xəbərdarlıq", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string selectedCode = cmbEmployees.SelectedValue.ToString();
            string title = txtPaymentTitle.Text.Trim();

            if (string.IsNullOrWhiteSpace(title))
            {
                MessageBox.Show("Ödənişin adını daxil edin!", "Xəbərdarlıq", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!double.TryParse(txtPaymentAmount.Text.Trim(), out double amount) || amount <= 0)
            {
                MessageBox.Show("Ödəniş məbləği 0-dan böyük ədəd olmalıdır!", "Xəta", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            bool isPaymentAdded = service.AddPayment(selectedCode, title, amount);

            if (isPaymentAdded)
            {
                MessageBox.Show("Ödəniş uğurla qeydə alındı!", "Uğur", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtPaymentTitle.Clear();
                txtPaymentAmount.Clear();
            }
            else
            {
                MessageBox.Show("Ödəniş əlavə edilə bilmədi!", "Xəta", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string searchCode = txtSearchCode.Text.Trim();

            if (string.IsNullOrWhiteSpace(searchCode))
            {
                MessageBox.Show("Axtarış üçün işçi kodunu daxil edin!", "Xəbərdarlıq", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Employee emp = service.FindEmployee(searchCode);

            if (emp == null)
            {
                lblEmployeeDetails.Text = "İşçi məlumatı: Tapılmadı";
                lstPayments.Items.Clear();
                MessageBox.Show("Belə koda malik işçi tapılmadı!", "Xəbərdarlıq", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            lblEmployeeDetails.Text = $"Kod: {emp.Code} | Ad: {emp.FirstName} | Soyad: {emp.LastName} | Yaş: {emp.Age}";

            lstPayments.Items.Clear();
            List<Payment> payments = service.GetEmployeePayments(searchCode);

            if (payments != null && payments.Count > 0)
            {
                foreach (var p in payments)
                {
                    lstPayments.Items.Add($"{p.Title} — {p.Amount} AZN");
                }
            }
            else
            {
                lstPayments.Items.Add("Bu işçinin hələ heç bir ödənişi yoxdur.");
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Proqramdan çıxmaq istədiyinizə əminsiniz?", "Çıxış Təsdiqi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void RefreshEmployeeViews()
        {
            List<Employee> allEmployees = service.GetAllEmployees();

            dgvEmployees.DataSource = null;
            if (allEmployees != null && allEmployees.Count > 0)
            {
                dgvEmployees.DataSource = new List<Employee>(allEmployees);
            }

            cmbEmployees.DataSource = null;
            if (allEmployees != null && allEmployees.Count > 0)
            {
                cmbEmployees.DataSource = new List<Employee>(allEmployees);
                cmbEmployees.DisplayMember = "FirstName";
                cmbEmployees.ValueMember = "Code";
            }
        }

        private void ClearEmployeeInputs()
        {
            txtCode.Clear();
            txtFirstName.Clear();
            txtLastName.Clear();
            txtAge.Clear();
        }
    }
}