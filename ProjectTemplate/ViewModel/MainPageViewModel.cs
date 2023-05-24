using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.ObjectModel;
using System.Globalization;

namespace ProjectTemplate.ViewModel
{

    public class CsvMap : ObservableObject
    {
        private int id;
        public int employeeID
        {
            get => id;
            set
            {
                if (SetProperty(ref id, value))
                    OnPropertyChanged(nameof(employeeID));
            }
        }

        private string firstname;
        public string firstName
        {
            get => firstname;
            set
            {
                if (SetProperty(ref firstname, value))
                    OnPropertyChanged(nameof(firstName));
            }
        }

        private string lastname;
        public string lastName
        {
            get => lastname;
            set
            {
                if (SetProperty(ref lastname, value))
                    OnPropertyChanged(nameof(lastName));
            }
        }

        private string tEmp;
        public string typeEmployee
        {
            get => tEmp;
            set
            {
                if (SetProperty(ref tEmp, value))
                    OnPropertyChanged(nameof(typeEmployee));
            }
        }

        private int hrlyRate;
        public int hourlyRate
        {
            get => hrlyRate;
            set
            {
                if (SetProperty(ref hrlyRate, value))
                    OnPropertyChanged(nameof(hourlyRate)); ;
            }
        }

        private string txThr;
        public string taxthreshold
        {
            get => txThr;
            set
            {
                if (SetProperty(ref txThr, value))
                    OnPropertyChanged(nameof(taxthreshold));
            }
        }
    }
    public sealed class CsvMapMap : ClassMap<CsvMap>
    {
        public CsvMapMap()
        {
            Map(m => m.employeeID).Index(0);
            Map(m => m.firstName).Index(1);
            Map(m => m.lastName).Index(2);
            Map(m => m.typeEmployee).Index(3);
            Map(m => m.hourlyRate).Index(4);
            Map(m => m.taxthreshold).Index(5);
        }
    }


    public partial class MainPageViewModel: ObservableObject
    {
        public MainPageViewModel()
        {
            Person = new ObservableCollection<CsvMap>();
        }

        [ObservableProperty]
        ObservableCollection<CsvMap> person = new ObservableCollection<CsvMap>();


        [RelayCommand]
        public async void ImportSomeRecords()
        {
            string fileName = "employee.csv";
            using Stream fileStream = await FileSystem.Current.OpenAppPackageFileAsync(fileName);

            using (var reader = new StreamReader(fileStream))
            {
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    csv.Context.RegisterClassMap<CsvMapMap>();


                    int employeeID;
                    string firstName;
                    string lastName;
                    string typeEmployee;
                    int hourlyRate;
                    string taxthreshold;


                    while (csv.Read())
                    {
                        employeeID = csv.GetField<int>(0);
                        firstName = csv.GetField<string>(1);
                        lastName = csv.GetField<string>(2);
                        typeEmployee = csv.GetField<string>(3);
                        hourlyRate = csv.GetField<int>(4);
                        taxthreshold = csv.GetField<string>(5);
                        Person.Add(CreateRecord(employeeID, firstName, lastName, typeEmployee, hourlyRate, taxthreshold));

                    }

                }

            }
        }

        public static CsvMap CreateRecord(int employeeID, string firstName, string lastName, string typeEmployee, int hourlyRate, string taxthreshold)
        {
            CsvMap record = new CsvMap();

            record.employeeID = employeeID;
            record.firstName = firstName;
            record.lastName = lastName;
            record.typeEmployee = typeEmployee;
            record.hourlyRate = hourlyRate;
            record.taxthreshold = taxthreshold;

            return record;
        }
    }
}


