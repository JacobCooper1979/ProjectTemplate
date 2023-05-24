using CsvHelper;
using ProjectTemplate.ViewModel;
using System.Collections.ObjectModel;
using System.Globalization;

namespace ProjectTemplate;

public partial class MainPage : ContentPage
{

    public CsvMap CurrentdgPerson = new CsvMap();
    //Flag nothing selected on start up
    public bool sFlag = false;

    public MainPage(MainPageViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;

        viewModel.ImportSomeRecords(); 
    }

    //Select DG
    private void PersonView_ItemSelected(object sender, SelectionChangedEventArgs e)
    {
        CsvMap selection = PersonView.SelectedItem as CsvMap;

        CurrentdgPerson.employeeID = selection.employeeID;
        CurrentdgPerson.firstName = selection.firstName;
        CurrentdgPerson.lastName = selection.lastName;
        CurrentdgPerson.typeEmployee = selection.typeEmployee;
        CurrentdgPerson.hourlyRate = selection.hourlyRate;
        CurrentdgPerson.taxthreshold = selection.taxthreshold;

        sFlag = true;
    }

    //Calculate BTN
    private async void CalcButton_Clicked(object sender, EventArgs e)
    {
        if(sFlag == true)
        {
            //Get selected person
            //CurrentdgPerson.employeeID

            //Get user input
            var input = hrsEntered.Text; //CONVERT THIS LATER?
            //var input2 = Convert.ToInt32(hrsEntered.Text);
            //await DisplayAlert("TITLE", input + input2.ToString(), "OK");

            //Calc
            //PayCalculator mycurrentcalc = new PayCalculator();

            //Create payslip
            //Payslip mycurrentpayslip = new Payslip();

        }
        else
        {
            await DisplayAlert("Nothing selected", "Please select and an employee", "OK");
        }
    }

    //Save BTN
    private void SaveButton_Clicked(object sender, EventArgs e)
    {
        //Save payslip
    }



}

public class Payslip
{
    //Set up the logic for our payslip
    //TAX information
    //EMPLOYEE information

    //CALL CALC OR USE PAYSLIP in CALC object later?
}

public class PayCalculator
{
    //methods for calculating the gross pay, net pay, superannuation and tax amounts. 

    //GROSS PAY


    //NET PAY


    //SUPER


    //TAX
    //CHILDREN TAX WITH OR WITHOUT
    //OR
    //TWO METHODS FOR TAX WITH OR WITHOUT

    //IMPORT and make objects + list as you import
    //WageCap { colA, colb } 
    //list<wageCap>

    //TaxRateObj {taxA, taxB}
    //list<TaxRateObj>

}

