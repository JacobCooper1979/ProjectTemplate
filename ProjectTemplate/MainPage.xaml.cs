using CsvHelper;
using ProjectTemplate.ViewModel;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using CsvHelper.Configuration.Attributes;
//using static ProjectTemplate.MainPage;

namespace ProjectTemplate
{
    public partial class MainPage : ContentPage
    {
        public CsvMap CurrentdgPerson = new();
        // Flag to indicate if an item is selected
        public bool sFlag = false;

        public MainPage(MainPageViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;

            viewModel.ImportSomeRecords();
            ReadTaxRatesFromCSV();
        }

        // Event handler for selecting a data grid item
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

        private async void CalcButton_Clicked(object sender, EventArgs e)
        {
            if (sFlag)
            {
                // Get user input
                var hoursWorked = Convert.ToDecimal(hrsEntered.Text);

                // Perform calculations
                decimal grossPayment = PayCalculator.CalculateGross(CurrentdgPerson.hourlyRate, hoursWorked);
                decimal superAmount = PayCalculator.CalculateSuperannuation(grossPayment);

                // Convert taxthreshold to bool
                bool taxThreshold = false;
                if (string.Equals(CurrentdgPerson.taxthreshold, "Y", StringComparison.OrdinalIgnoreCase))
                    taxThreshold = true;

                // Update the Payslip Summary labels with the calculated values
                Name.Text = CurrentdgPerson.firstName + " " + CurrentdgPerson.lastName;
                TaxThreshold.Text = taxThreshold ? "Yes" : "No";
                HoursWorked.Text = hoursWorked.ToString();
                HourlyRate.Text = CurrentdgPerson.hourlyRate.ToString();
                GrossPay.Text = grossPayment.ToString();
                NetPay.Text = "To be calculated";
                SuperAnnuation.Text = superAmount.ToString();
                Date.Text = DateTime.Now.ToString("dd/MM/yyyy");
                Time.Text = DateTime.Now.ToString("HH:mm:ss");

                // Create payslip
                Payslip payslip = new("1", CurrentdgPerson.employeeID.ToString(), grossPayment, 0, 0, superAmount);

                // Save payslip to CSV
                await SavePayslipToCSV(payslip);

                await DisplayAlert("Success", "Payslip saved", "OK");
            }
        }


        // Define the class for tax rates with thresholds
        public class TaxRateWithThreshold
        {
            public decimal Low { get; set; }
            public decimal High { get; set; }
            public decimal TaxRate1 { get; set; }
            public decimal TaxRate2 { get; set; }
        }

        // Define the class for tax rates
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

        // Define the base class for a person
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

        // Define the derived class for an employee
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


            //Calculates the total gross payment
            public decimal CalculateGross()
            {
                return hourlyRate * hoursWorked;
            }
        }

        // Define the class for a payslip
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

        //ISSUE WITH FILE PERMISSIONS
        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

        // Method for reading nothreshold csv
        public static void ReadTaxRatesFromCSV()
        {
            string csvPath = "C:\\Users\\jacob\\Documents\\Tafe Cert 4\\c#\\Wednesday_Shaun_OOP\\Assesments\\Project_14June\\TaxMaui\\ProjectTemplate\\taxrate-nothreshold.csv";
            ObservableCollection<TaxRate> taxRates;

            using var reader = new StreamReader(csvPath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            // Read the CSV records into the taxRates collection
            taxRates = new ObservableCollection<TaxRate>(csv.GetRecords<TaxRate>());
        }

        // Method for reading withinthreshold csv
        public static void ReadTaxRatesWithThresholdFromCSV()
        {
            string csvPath = "C:\\Users\\jacob\\Documents\\Tafe Cert 4\\c#\\Wednesday_Shaun_OOP\\Assesments\\Project_14June\\TaxMaui\\ProjectTemplate\\taxrate-withthreshold.csv";
            ObservableCollection<TaxRateWithThreshold> taxRatesWithThreshold;

            using var reader = new StreamReader(csvPath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            // Read the CSV records into the taxRatesWithThreshold collection
            taxRatesWithThreshold = new ObservableCollection<TaxRateWithThreshold>(csv.GetRecords<TaxRateWithThreshold>());
        }


        public class PayCalculator
        {
            // 15% superannuation rate
            private const decimal SuperRate = 0.15m;

            // Calculate the superannuation amount based on the gross payment
            public static decimal CalculateSuperannuation(decimal grossPayment)
            {
                return grossPayment * SuperRate;
            }

            // Calculate the gross pay based on the hourly rate and hours worked
            public static decimal CalculateGross(decimal hourlyRate, decimal hoursWorked)
            {
                return hourlyRate * hoursWorked;
            }

            // Calculate the net pay
            public static decimal CalculateNet(decimal grossPayment, decimal taxAmount)
            {
                return grossPayment - taxAmount;
            }
        }

        // Method to save the payslip to CSV
        private async Task SavePayslipToCSV(Payslip payslip)
        {
            string csvPath = "payslips.csv";

            try
            {
                using (var writer = new StreamWriter(csvPath, true))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    // Check if the CSV file exists
                    bool fileExists = File.Exists(csvPath);

                    // Write the CSV headers if the file doesn't exist
                    if (!fileExists)
                    {
                        csv.WriteHeader<Payslip>();
                        csv.NextRecord();
                    }

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


                await Task.Run(() => DisplayAlert("Success", "Payslip saved.", "OK"));
            }
            catch (Exception ex)
            {

                await Task.Run(() => DisplayAlert("Error", "Payslip failed to save " + ex.Message, "OK"));
            }
        }

        // Event handler for the Save button
        private void SaveButtonClicked(object sender, EventArgs e)
        {
            // Creates a new instance of Payslip
            Payslip payslip = new("1", CurrentdgPerson.employeeID.ToString(), 0, 0, 0, 0);

            SavePayslipToCSV(payslip).ContinueWith(task =>
            {
                if (task.IsCompletedSuccessfully)
                {
                    this.Dispatcher.Dispatch(() =>
                    {
                        DisplayAlert("Success", "Payslip saved", "OK");
                        ClearForm();
                    });
                }
                else
                {
                    this.Dispatcher.Dispatch(() =>
                    {
                        DisplayAlert("Error", "Failed to save payslip", "OK");
                    });
                }
            });
        }


        //Method for clearing the form
        private void ClearForm()
        {
            hrsEntered.Text = string.Empty;
        }


    }
}










