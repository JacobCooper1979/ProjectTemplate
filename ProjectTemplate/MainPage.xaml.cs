using CsvHelper;
using ProjectTemplate.ViewModel;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using CsvHelper.Configuration.Attributes;
using System.Dynamic;

namespace ProjectTemplate
{
    public partial class MainPage : ContentPage
    {
        public CsvMap CurrentdgPerson = new();
        public bool sFlag = false;

        /// <summary>
        /// Initializes the application
        /// </summary>
        /// <param name="viewModel"></param>
        public MainPage(MainPageViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;

            viewModel.ImportSomeRecords();
            ReadTaxRatesFromCSV();
        }

        /// <summary>
        /// Event handler for selecting a data grid item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PersonView_ItemSelected(object sender, SelectionChangedEventArgs e)
        {
            // Get the selected item from the data grid
            CsvMap selection = PersonView.SelectedItem as CsvMap;

            // Update the CurrentdgPerson with the selected item's details
            CurrentdgPerson.employeeID = selection.employeeID;
            CurrentdgPerson.firstName = selection.firstName;
            CurrentdgPerson.lastName = selection.lastName;
            CurrentdgPerson.typeEmployee = selection.typeEmployee;
            CurrentdgPerson.hourlyRate = selection.hourlyRate;
            CurrentdgPerson.taxthreshold = selection.taxthreshold;

            // Set the flag to indicate that an item is selected
            sFlag = true;
        }

        /// <summary>
        /// Method for the calculator button when clicked 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void CalcButton_Clicked(object sender, EventArgs e)
        {
            if (sFlag)
            {
                // Get user input
                var hoursWorked = Convert.ToDecimal(hrsEntered.Text);

                // Perform calculations
                decimal grossPayment = PayCalculator.CalculateGross(CurrentdgPerson.hourlyRate, hoursWorked);
                decimal superAmount = PayCalculator.CalculateSuperannuation(grossPayment);

                // Calculate the tax amount
                decimal taxAmount = CalculateTaxAmount(grossPayment, CurrentdgPerson.taxthreshold);

                // Calculate the net pay
                decimal netPay = PayCalculator.CalculateNet(grossPayment, taxAmount);

                // Update the Payslip Summary labels with the calculated values
                Name.Text = "Employee Name: " + CurrentdgPerson.firstName + " " + CurrentdgPerson.lastName;
                bool taxThreshold = IsWithinTaxThreshold(grossPayment, CurrentdgPerson.taxthreshold);
                TaxThreshold.Text = taxThreshold ? "Tax Threshold: Yes" : "Tax Threshold: No";
                HoursWorked.Text = "Hours Worked: " + hoursWorked.ToString();
                HourlyRate.Text = "Hourly Rate: " + CurrentdgPerson.hourlyRate.ToString();
                GrossPay.Text = "Gross Pay: " + grossPayment.ToString();
                NetPay.Text = "Net Pay: " + netPay.ToString();
                SuperAnnuation.Text = "Super Annuation: " + superAmount.ToString();
                Date.Text = "Date: " + DateTime.Now.ToString("dd/MM/yyyy");
                Time.Text = "Time: " + DateTime.Now.ToString("HH:mm:ss");

                
            }
        }

        /// <summary>
        /// Calculate the tax amount based on the gross payment and tax threshold
        /// </summary>
        /// <param name="grossPayment"></param>
        /// <param name="taxThreshold"></param>
        /// <returns></returns>
        private static decimal CalculateTaxAmount(decimal grossPayment, string taxThreshold)
        {
            decimal taxAmount = 0m;

            if (IsWithinTaxThreshold(grossPayment, taxThreshold))
            {
                // Calculate the tax amount using the tax rate for within the threshold
                taxAmount = grossPayment * 0.2m;
            }

            return taxAmount;
        }

        /// <summary>
        /// Check if the gross payment is within the tax threshold
        /// </summary>
        /// <param name="grossPayment"></param>
        /// <param name="taxThreshold"></param>
        /// <returns></returns>
        private static bool IsWithinTaxThreshold(decimal grossPayment, string taxThreshold)
        {
            return taxThreshold.Equals("Y", StringComparison.OrdinalIgnoreCase) && grossPayment >= 2000m;
        }



        /// <summary>
        /// Define the class for tax rates with thresholds
        /// </summary>
        public class TaxRateWithThreshold
        {
            public decimal Low { get; set; }
            public decimal High { get; set; }
            public decimal TaxRate1 { get; set; }
            public decimal TaxRate2 { get; set; }
        }

        /// <summary>
        /// Define the class for tax rates
        /// </summary>
        public class TaxRate
        {
            [Index(0)]
            public decimal Low { get; set; }

            [Index(1)]
            public decimal High { get; set; }

            [Index(2)]
            public decimal TaxRate1 { get; set; }

            [Index(3)]
            public decimal TaxRate2 { get; set; }
        }

        /// <summary>
        ///  Define the base class for a person
        /// </summary>
        public class Person
        {
            public string Id { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }

            public Person(string id, string username, string password)
            {
                Id = id;
                Username = username;
                Password = password;
            }
        }

        /// <summary>
        /// Define the derived class for an employee
        /// </summary>
        public class Employee : Person
        {
            private readonly decimal hourlyRate;
            private readonly decimal hoursWorked;

            public Employee(string id, string username, string password, decimal hourlyRate, decimal hoursWorked)
                : base(id, username, password)
            {
                this.hourlyRate = hourlyRate;
                this.hoursWorked = hoursWorked;
            }


            /// <summary>
            /// Calculates the total gross payment
            /// </summary>
            /// <returns></returns>
            public decimal CalculateGross()
            {
                return hourlyRate * hoursWorked;
            }
        }

        /// <summary>
        /// Define the class for a payslip
        /// </summary>
        public class Payslip
        {
            private readonly string id;
            private readonly string employeeId;
            private readonly decimal grossPayment;
            private decimal netPayment;
            private decimal taxAmount;
            private decimal superAmount;
            private bool approved;

            public Payslip(string id, string employeeId, decimal grossPayment, decimal netPayment, decimal taxAmount, decimal superAmount)
            {
                this.id = id;
                this.employeeId = employeeId;
                this.grossPayment = grossPayment;
                this.netPayment = netPayment;
                this.taxAmount = taxAmount;
                this.superAmount = superAmount;
                this.approved = false;
            }

            public string Id
            {
                get { return id; }
            }

            public string EmployeeId
            {
                get { return employeeId; }
            }

            public decimal GrossPayment
            {
                get { return grossPayment; }
            }

            public decimal NetPayment
            {
                get { return netPayment; }
                set { netPayment = value; }
            }

            public decimal TaxAmount
            {
                get { return taxAmount; }
                set { taxAmount = value; }
            }

            public decimal SuperAmount
            {
                get { return superAmount; }
                set { superAmount = value; }
            }

            public bool Approved
            {
                get { return approved; }
                set { approved = value; }
            }
        }


        /// <summary>
        /// Method for reading nothreshold csv
        /// </summary>
        public static void ReadTaxRatesFromCSV()
        {
            string csvPath = "C:\\Users\\jacob\\Documents\\Tafe Cert 4\\c#\\Wednesday_Shaun_OOP\\Assesments\\Project_14June\\TaxMaui\\ProjectTemplate\\taxrate-nothreshold.csv";
            ObservableCollection<TaxRate> taxRates;

            using var csv = new CsvReader(new StreamReader(csvPath), CultureInfo.InvariantCulture);
            taxRates = new ObservableCollection<TaxRate>(csv.GetRecords<TaxRate>());
        }



        /// <summary>
        /// Method for reading withinthreshold csv
        /// </summary>
        public static void ReadTaxRatesWithThresholdFromCSV()
        {
            string csvPath = "C:\\Users\\jacob\\Documents\\Tafe Cert 4\\c#\\Wednesday_Shaun_OOP\\Assesments\\Project_14June\\TaxMaui\\ProjectTemplate\\taxrate-withthreshold.csv";
            ObservableCollection<TaxRateWithThreshold> taxRatesWithThreshold;

            using var csv = new CsvReader(new StreamReader(csvPath), CultureInfo.InvariantCulture);
            taxRatesWithThreshold = new ObservableCollection<TaxRateWithThreshold>(csv.GetRecords<TaxRateWithThreshold>());
        }


        /// <summary>
        /// Defines the class for PayCalculator
        /// </summary>
        public class PayCalculator
        {
            /// <summary>
            /// 10% superannuation rate
            /// </summary>
            private const decimal SuperRate = 0.10m;

            /// <summary>
            /// Calculate the superannuation amount based on the gross payment
            /// </summary>
            /// <param name="grossPayment"></param>
            /// <returns></returns>
            public static decimal CalculateSuperannuation(decimal grossPayment)
            {
                return grossPayment * SuperRate;
            }

            /// <summary>
            /// Calculate the gross pay based on the hourly rate and hours worked
            /// </summary>
            /// <param name="hourlyRate"></param>
            /// <param name="hoursWorked"></param>
            /// <returns></returns>
            public static decimal CalculateGross(decimal hourlyRate, decimal hoursWorked)
            {
                return hourlyRate * hoursWorked;
            }

            /// <summary>
            /// Calculate the net pay
            /// </summary>
            /// <param name="grossPayment"></param>
            /// <param name="taxAmount"></param>
            /// <returns></returns>
            public static decimal CalculateNet(decimal grossPayment, decimal taxAmount)
            {
                return grossPayment - taxAmount;
            }
        }

        /// <summary>
        /// Method to save the payslip to CSV
        /// </summary>
        /// <param name="payslip"></param>
        private void SavePayslipToCSV(Payslip payslip)
        {
            try
            {
                string filename = @"C:\Users\jacob\Documents\Tafe Cert 4\c#\Wednesday_Shaun_OOP\Assesments\Project_14June\TaxMaui\ProjectTemplate\Payslips.csv";
                using (StreamWriter writer = new StreamWriter(filename, true))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    // Write the payslip values to the CSV
                    csv.WriteField(payslip.Id);
                    csv.WriteField(payslip.EmployeeId);
                    csv.WriteField(payslip.GrossPayment);
                    csv.WriteField(payslip.NetPayment);
                    csv.WriteField(payslip.TaxAmount);
                    csv.WriteField(payslip.SuperAmount);
                    csv.WriteField(payslip.Approved);
                    csv.NextRecord();
                }

                DisplayAlert("Success", "Payslip saved.", "OK");
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", "Payslip failed to save " + ex.Message, "OK");
            }
        }


        /// <summary>
        /// Event handler for the Save button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveButtonClicked(object sender, EventArgs e)
        {
            if (sFlag)
            {
                // Get user input
                var hoursWorked = Convert.ToDecimal(hrsEntered.Text);

                // Perform calculations
                decimal grossPayment = PayCalculator.CalculateGross(CurrentdgPerson.hourlyRate, hoursWorked);
                decimal superAmount = PayCalculator.CalculateSuperannuation(grossPayment);

                // Calculate the tax amount 
                decimal taxAmount = grossPayment * 0.2m;

                // Convert taxthreshold to bool
                bool taxThreshold = false;
                if (string.Equals(CurrentdgPerson.taxthreshold, "Y", StringComparison.OrdinalIgnoreCase))
                    taxThreshold = true;

                // Calculate the net pay
                decimal netPay = PayCalculator.CalculateNet(grossPayment, taxAmount);

                // Update the Payslip Summary labels with the calculated values
                Name.Text = "Employee Name: " + CurrentdgPerson.firstName + " " + CurrentdgPerson.lastName;
                TaxThreshold.Text = taxThreshold ? "Tax Threshold: Yes" : "Tax Threshold: No";
                HoursWorked.Text = "Hours Worked: " + hoursWorked.ToString();
                HourlyRate.Text = "Hourly Rate: " + CurrentdgPerson.hourlyRate.ToString();
                GrossPay.Text = "Gross Pay: " + grossPayment.ToString();
                NetPay.Text = "Net Pay: " + netPay.ToString();
                SuperAnnuation.Text = "Super Annuation: " + superAmount.ToString();
                Date.Text = "Date: " + DateTime.Now.ToString("dd/MM/yyyy");
                Time.Text = "Time: " + DateTime.Now.ToString("HH:mm:ss");

                // Create and save the payslip
                Payslip payslip = new("1", CurrentdgPerson.employeeID.ToString(), grossPayment, netPay, taxAmount, superAmount);
                SavePayslipToCSV(payslip);
            }
        }
    }
}


