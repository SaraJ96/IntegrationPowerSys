using FTN.Common;
using ClientUIApp.ViewModels;
using ClientUIApp.GDA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClientUIApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainViewModel mvm = new MainViewModel();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = mvm;
            getExtValuesMCComboBox.ItemsSource = MainHelper.AllTypesModelCodes;
        }

        //prvi tab
        private void getValuesGIDcmb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (getValuesGIDcmb.SelectedIndex != -1)
            {

                long globalId = Convert.ToInt64(getValuesGIDcmb.SelectedValue.ToString().Split('x')[1], 16); // pretvaranje id-a u long
                mvm.UpdatePropertiesForGetValues(globalId);
            }
        }

        //button za prvi tab
        private void getValueButton_Click(object sender, RoutedEventArgs e)
        {
            if (getValuesGIDcmb.SelectedIndex != -1)
            {
                long globalId = Convert.ToInt64(getValuesGIDcmb.SelectedValue.ToString().Split('x')[1], 16);
                mvm.GetValues(globalId);
            }
        }

        private void getExtValuesMCComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (getExtValuesMCComboBox.SelectedIndex != -1)
            {
                mvm.UpdatePropertiesForGetExtValues((ModelCode)getExtValuesMCComboBox.SelectedValue);
            }
        }

        private void getExtButton_Click(object sender, RoutedEventArgs e)
        {
            if (getExtValuesMCComboBox.SelectedIndex != -1)
            {
                mvm.GetExtentValues((ModelCode)getExtValuesMCComboBox.SelectedValue);
            }
        }

        // First ComboBox
        private void getRelatedGIDcmb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            HideGetRelatedElements();
            if (getRelatedGIDcmb.SelectedIndex != -1)
            {
                long globalId = Convert.ToInt64(getRelatedGIDcmb.SelectedValue.ToString().Split('x')[1], 16);

                // Update Property ID for Association 
                mvm.UpdatePropertiesForGetRelated(globalId);
                // Show next Combobox
                getRelatedPropertyIDLabel.Visibility = Visibility.Visible;
                getRelatedValuesPropertyIdComboBox.Visibility = Visibility.Visible;
            }
        }



        // Second ComboBox
        private void getRelatedValuesPropertyIdComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Reset labels,list and below combo
            getRelatedListView.Visibility = Visibility.Hidden;
            getRelatedButton.Visibility = Visibility.Hidden;

            getRelatedValuesTypeComboBox.SelectedIndex = -1;             //typeCombo vratimo na neodabrano

            if (getRelatedValuesPropertyIdComboBox.SelectedIndex != -1)
            {
                // Update Types for Association
                mvm.UpdateTypesForGetRelated((ModelCode)getRelatedValuesPropertyIdComboBox.SelectedValue);

                // Show next ComboBox               
                getRelatedTypeLabel.Visibility = Visibility.Visible;
                getRelatedValuesTypeComboBox.Visibility = Visibility.Visible;

            }

        }

        // Third ComboBox
        private void getRelatedValuesTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (getRelatedValuesTypeComboBox.SelectedIndex != -1)
            {
                // Update properties for Type
                ModelCode selectedMC = mvm.ReferenceTypes[getRelatedValuesTypeComboBox.SelectedValue.ToString()];

                mvm.UpdateTypePropertiesForGetRelated(selectedMC);

                // Update UI
                ShowGetRelatedElements();
                if (selectedMC == ModelCode.IDOBJ) //ako je odabrao NONE onda mu ne treba propertyList
                {
                    // Hide  ListView
                    getRelatedListView.Visibility = Visibility.Hidden;
                }
            }
        }

        private void getRelatedButton_Click(object sender, RoutedEventArgs e)
        {
            if ((getRelatedValuesTypeComboBox.SelectedIndex != -1) &&
                (getRelatedValuesPropertyIdComboBox.SelectedIndex != -1) &&
                (getRelatedGIDcmb.SelectedIndex != -1))
            {
                // Update ReferenceTypes for combobox
                long globalId = Convert.ToInt64(getRelatedGIDcmb.SelectedValue.ToString().Split('x')[1], 16);
                ModelCode property = (ModelCode)getRelatedValuesPropertyIdComboBox.SelectedItem;
                ModelCode type = mvm.ReferenceTypes[getRelatedValuesTypeComboBox.SelectedValue.ToString()];

                // Update ListView
                mvm.GetRelatedValues(globalId, property, type);

            }
        }

        private void HideGetRelatedElements()
        {
            getRelatedValuesPropertyIdComboBox.SelectedIndex = -1;
            getRelatedValuesTypeComboBox.SelectedIndex = -1;


            getRelatedListView.Visibility = Visibility.Hidden;
            getRelatedButton.Visibility = Visibility.Hidden;
            getRelatedTypeLabel.Visibility = Visibility.Hidden;
            getRelatedValuesTypeComboBox.Visibility = Visibility.Hidden;
            getRelatedValuesPropertyIdComboBox.Visibility = Visibility.Hidden;
        }
        private void ShowGetRelatedElements()
        {
            getRelatedButton.Visibility = Visibility.Visible;
            getRelatedListView.Visibility = Visibility.Visible;
        }
    }
}
