using ClientUIApp.GDA;
using ClientUIApp.Models;
using FTN.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientUIApp.ViewModels
{
    public class MainViewModel : ObservableModel
    {
        private GDAQuery gdaQuery = null;
        private List<string> allGlobalIDs = null;
        public ObservableCollection<PropertyViewModel> AllGIDProperties { get; set; }
        public ObservableCollection<PropertyViewModel> AllModelCodeProperties { get; set; }
        public ObservableCollection<PropertyViewModel> AllReferenceProperties { get; set; }
        public ObservableCollection<ModelCode> AllReferenceModelCodes { get; set; } // reference koje ima ModelCode za odredjeni GID
        public Dictionary<string, ModelCode> ReferenceTypes { get; set; } //asocijacije za odredjeni ModelCode
        public ObservableCollection<string> ReferenceTypesStrings { get; set; } // asocijace koje se ispusuju u comboBox

        private string logResultFirstTab;
        private string logResultSecondTab;
        private string logResultThirdTab;

        public string LogResultFirstTab
        {
            get { return logResultFirstTab; }
            set
            {
                logResultFirstTab = value;
                OnPropertyChanged("LogResultFirstTab");
            }
        }
        public string LogResultSecondTab
        {
            get { return logResultSecondTab; }
            set
            {
                logResultSecondTab = value;
                OnPropertyChanged("LogResultSecondTab");
            }
        }
        public string LogResultThirdTab
        {
            get { return logResultThirdTab; }
            set
            {
                logResultThirdTab = value;
                OnPropertyChanged("LogResultThirdTab");
            }
        }

        public MainViewModel()
        {
            gdaQuery = new GDAQuery();
            AllGIDProperties = new ObservableCollection<PropertyViewModel>();
            AllModelCodeProperties = new ObservableCollection<PropertyViewModel>();
            AllReferenceProperties = new ObservableCollection<PropertyViewModel>();
            AllReferenceModelCodes = new ObservableCollection<ModelCode>();
            ReferenceTypes = new Dictionary<string, ModelCode>();
            ReferenceTypesStrings = new ObservableCollection<string>();
            allGlobalIDs = ConvertToHexa(gdaQuery.GetAllGIDs(MainHelper.AllTypesModelCodes));
        }

        public List<string> AllGlobalIds
        {
            get
            {
                return allGlobalIDs;
            }
            set
            {
                allGlobalIDs = value;

            }
        }

        //Prvi Tab:
        public void GetValues(long globalId)
        {
            bool anySelected = false;
            List<ModelCode> selectedProperties = new List<ModelCode>();
            ResourceDescription rd = null;

            foreach (PropertyViewModel pvm in AllGIDProperties)
            {
                if (pvm.IsSelected)
                {
                    anySelected = true;
                    selectedProperties.Add(pvm.GetModelCode());
                }
            }

            if (anySelected)
                rd = gdaQuery.GetValues(globalId, selectedProperties);
            else
                rd = gdaQuery.GetValues(globalId, null);

            if (rd == null)
            {
                LogResultFirstTab = $"Projekat102 element with GID=0x{globalId.ToString("X16")} could not be found";
                return;
            }

            LogResultFirstTab = File.ReadAllText(Config.Instance.ResultDirecotry + "\\GetValues_Results.xml");
        }

        public void GetExtentValues(ModelCode modelCode)
        {
            bool anySelected = false;
            List<ModelCode> properties = new List<ModelCode>();
            List<long> extentResult = null;

            foreach (PropertyViewModel pvm in AllModelCodeProperties)
            {
                if (pvm.IsSelected)
                {
                    anySelected = true;
                    properties.Add(pvm.GetModelCode());
                }
            }

            if (anySelected)
                extentResult = gdaQuery.GetExtentValues(modelCode, properties);
            else
                extentResult = gdaQuery.GetExtentValues(modelCode, null);

            if (extentResult == null)
            {
                LogResultSecondTab = $"Projekat102 element with ModelCode={modelCode} could not be found";
                return;
            }

            LogResultSecondTab = File.ReadAllText(Config.Instance.ResultDirecotry + "\\GetExtentValues_Results.xml");
        }

        public void GetRelatedValues(long globalId, ModelCode propertyId, ModelCode type)
        {
            bool anySelected = false;
            List<ModelCode> properties = new List<ModelCode>();
            List<long> relatedResult = null;
            LogResultThirdTab = "";

            Association association = new Association();
            if (type == ModelCode.IDOBJ)
            {
                association.PropertyId = propertyId;
                association.Inverse = false;

            }
            else
            {
                association.PropertyId = propertyId;
                association.Inverse = false;
                association.Type = type;

                foreach (PropertyViewModel pvm in AllReferenceProperties)
                {
                    if (pvm.IsSelected)
                    {
                        anySelected = true;
                        properties.Add(pvm.GetModelCode());
                    }
                }
            }

            if (anySelected)
            {
                relatedResult = gdaQuery.GetRelatedValues(globalId, properties, association);
            }
            else
            {
                if (type == ModelCode.IDOBJ) //odabrao None
                {
                    Dictionary<string, List<ModelCode>> allTypeProps = new Dictionary<string, List<ModelCode>>(); //Kljuc je model, value su propertiji modela
                    List<string> refTypesMCForEx = new List<string>();
                    foreach (ModelCode refTypeMC in ReferenceTypes.Values)
                    {
                        if (refTypeMC == ModelCode.IDOBJ)
                            continue;
                        allTypeProps.Add(refTypeMC.ToString(), new List<ModelCode>());
                        refTypesMCForEx.Add(refTypeMC.ToString());

                        allTypeProps[refTypeMC.ToString()].AddRange(gdaQuery.GetAllPropertiesFromModelCode(refTypeMC));
                    }
                    int counter = 0;

                    try
                    {
                        foreach (var selectedProperties in allTypeProps.Values) //SVE propertije svih referenci
                        {
                            try
                            {
                                relatedResult = gdaQuery.GetRelatedValues(globalId, selectedProperties, association);
                                if (relatedResult != null && relatedResult.Count != 0)
                                {
                                    LogResultThirdTab += File.ReadAllText(Config.Instance.ResultDirecotry + "\\GetRelatedValues_Results.xml");
                                }
                                else //u slucaju da je nema u file-u
                                {
                                    LogResultThirdTab += $"There is no reference from (TYPE: {type}) to object with GID=0x{globalId.ToString("X16")}.";
                                }
                            }
                            catch
                            {
                                LogResultThirdTab += $"\nThe concrete class {refTypesMCForEx[counter]} doesn't exist ";
                                continue;

                            }
                            finally
                            {
                                counter++;
                            }

                        }
                        return;

                    }
                    catch (Exception e)
                    {
                        LogResultThirdTab += "\nA concrete class doesn't contain a specific property.";
                        return;
                    }
                }
                else
                {
                    // Ako nije selektovan ni NONE ni property
                    List<ModelCode> allTypeProps = gdaQuery.GetAllPropertiesFromModelCode(type);
                    try
                    {
                        relatedResult = gdaQuery.GetRelatedValues(globalId, allTypeProps, association);
                    }
                    catch (Exception e)
                    {
                        LogResultThirdTab = "A concrete class doesn't contain a specific property.";
                        return;
                    }
                }
            }


            if (relatedResult == null)
            {
                LogResultThirdTab = $"Projekat102 element with GID=0x{globalId.ToString("X16")} could not be found";
                return;
            }

            if (relatedResult.Count == 0)
                LogResultThirdTab = $"There is no reference from (TYPE: {type}) to object with GID=0x{globalId.ToString("X16")}.";
            else
                LogResultThirdTab = File.ReadAllText(Config.Instance.ResultDirecotry + "\\GetRelatedValues_Results.xml");

        }

        //Za prvi tab propertije
        public void UpdatePropertiesForGetValues(long globalId)
        {
            short type = ModelCodeHelper.ExtractTypeFromGlobalId(globalId);
            List<ModelCode> allProps = gdaQuery.GetAllPropertiesFromDMSType((DMSType)type);

            if (AllGIDProperties.Count > 0)
                AllGIDProperties.Clear();

            foreach (ModelCode mc in allProps)
            {
                AllGIDProperties.Add(new PropertyViewModel(new Property(mc), false));
            }
            OnPropertyChanged("AllGIDProperties");
        }
        //za drugi tab
        public void UpdatePropertiesForGetExtValues(ModelCode modelCode)
        {
            List<ModelCode> allProps = gdaQuery.GetAllPropertiesFromModelCode(modelCode);

            if (AllModelCodeProperties.Count > 0)
                AllModelCodeProperties.Clear();

            foreach (ModelCode mc in allProps)
            {
                AllModelCodeProperties.Add(new PropertyViewModel(new Property(mc), false));
            }
            OnPropertyChanged("AllModelCodeProperties");
        }
        //za treci tab
        public void UpdatePropertiesForGetRelated(long globalId)
        {
            short type = ModelCodeHelper.ExtractTypeFromGlobalId(globalId);
            List<ModelCode> allProps = gdaQuery.GetAllPropertiesFromDMSType((DMSType)type);

            if (AllReferenceModelCodes.Count > 0)
                AllReferenceModelCodes.Clear();

            foreach (ModelCode mc in allProps)
            {
                short referencesType = 0x19;
                short referenceType = 0x09;

                byte[] mcBytes = BitConverter.GetBytes((long)mc);
                short mcType = mcBytes[0];   //0x19

                if (mcType == referenceType || mcType == referencesType)
                {
                    AllReferenceModelCodes.Add(mc);
                    OnPropertyChanged("AllReferenceModelCodes");
                }
            }
        }

        public void UpdateTypePropertiesForGetRelated(ModelCode modelCode)
        {
            if (modelCode != ModelCode.IDOBJ) // ako je NONE onda mu daj sve
            {
                DMSType type = ModelCodeHelper.GetTypeFromModelCode(modelCode);
                List<ModelCode> allProps = gdaQuery.GetAllPropertiesFromDMSType(type);

                if (AllReferenceProperties.Count > 0)
                    AllReferenceProperties.Clear();

                foreach (ModelCode mc in allProps)
                {
                    AllReferenceProperties.Add(new PropertyViewModel(new Property(mc), false));
                }
                OnPropertyChanged("AllReferenceProperties");
            }
            else
            {
                if (AllReferenceProperties.Count > 0)
                    AllReferenceProperties.Clear();

                OnPropertyChanged("AllReferenceProperties");
            }


        }

        //za drugi deo:
        public void UpdateTypePropertiesForGetRelatedNONE(List<ModelCode> modelCodes)
        {
            if (AllReferenceProperties.Count > 0)
                AllReferenceProperties.Clear();

            foreach (ModelCode modelCode in modelCodes)
            {
                DMSType type = ModelCodeHelper.GetTypeFromModelCode(modelCode);
                List<ModelCode> allProps = gdaQuery.GetAllPropertiesFromDMSType(type);



                foreach (ModelCode mc in allProps)
                {

                    if (AllReferenceProperties.Any(x => x.Name == mc.ToString()))
                    {
                        continue;
                    }

                    AllReferenceProperties.Add(new PropertyViewModel(new Property(mc), false));
                }
                OnPropertyChanged("AllReferenceProperties");
            }

        }

        public void UpdateTypesForGetRelated(ModelCode referenceTypeModelCode)
        {
            if (ReferenceTypes.Count > 0)
                ReferenceTypes.Clear();

            ReferenceTypes.Add("NONE", ModelCode.IDOBJ);
            OnPropertyChanged("ReferenceType");

            switch (referenceTypeModelCode)
            {
                case ModelCode.TERMINAL_REGULATINGCONTROL:
                    ReferenceTypes.Add($"{ModelCode.REGULATINGCONTROL}", ModelCode.REGULATINGCONTROL);
                    OnPropertyChanged("ReferenceType");
                    break;
                case ModelCode.REGULATINGCONTROL_TERMINAL:
                    ReferenceTypes.Add($"{ModelCode.TERMINAL}", ModelCode.TERMINAL);
                    OnPropertyChanged("ReferenceType");
                    break;
                case ModelCode.REGULATINGCONTROL_REGULATINGCONDEQ:
                    ReferenceTypes.Add($"{ModelCode.REGULATINGCONTROL_REGULATINGCONDEQ}", ModelCode.REGULATINGCONTROL_REGULATINGCONDEQ);
                    OnPropertyChanged("ReferenceType");
                    break;
                case ModelCode.REGULATINGCONDEQ_REGULATINGCONTROL:
                    ReferenceTypes.Add($"{ModelCode.REGULATINGCONTROL}", ModelCode.REGULATINGCONTROL);
                    OnPropertyChanged("ReferenceType");
                    break;
                case ModelCode.CONTROL_REGULATINGCONDEQ:
                    ReferenceTypes.Add($"{ModelCode.STATICVARCOMPENSATOR}", ModelCode.STATICVARCOMPENSATOR);
                    ReferenceTypes.Add($"{ModelCode.SYNCHRONOUSMACHINE}", ModelCode.SYNCHRONOUSMACHINE);
                    OnPropertyChanged("ReferenceType");
                    break;
                case ModelCode.REGULATINGCONDEQ_CONTROLS:
                    ReferenceTypes.Add($"{ModelCode.CONTROL}", ModelCode.CONTROL);
                    OnPropertyChanged("ReferenceType");
                    break;
                case ModelCode.SYNCHRONOUSMACHINE_REACTIVECAPABILITYCURVES:
                    ReferenceTypes.Add($"{ModelCode.REACTIVECAPABILITYCURVE}", ModelCode.REACTIVECAPABILITYCURVE);
                    OnPropertyChanged("ReferenceType");
                    break;
                case ModelCode.REACTIVECAPABILITYCURVE_SYNCHRONOUSMACHINE:
                    ReferenceTypes.Add($"{ModelCode.SYNCHRONOUSMACHINE}", ModelCode.SYNCHRONOUSMACHINE);
                    OnPropertyChanged("ReferenceType");
                    break;
            }

            if (ReferenceTypesStrings.Count > 0)
                ReferenceTypesStrings.Clear();
            // Update strings for Third ComboBox
            foreach (var key in ReferenceTypes.Keys)
                ReferenceTypesStrings.Add(key);
            OnPropertyChanged("ReferenceTypesStrings");
        }


        private List<string> ConvertToHexa(List<long> ids)
        {
            List<string> hexaIds = new List<string>(ids.Count);

            foreach (long id in ids)
                hexaIds.Add($"0x{id.ToString("X16")}");

            return hexaIds;
        }

    }
}
