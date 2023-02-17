using FTN.Common;
using FTN.ServiceContracts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private ModelResourcesDesc modelResourcesDesc = new ModelResourcesDesc();
        private static NetworkModelGDAProxy gdaQueryProxy = null;

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
        public NetworkModelGDAProxy GDAQueryProxy
        {
            get { return gdaQueryProxy; }
        }

        public MainWindow()
        {
            InitializeComponent();

            if (gdaQueryProxy == null)
            {
                gdaQueryProxy = new NetworkModelGDAProxy("NetworkModelGDAEndpoint");
                try
                {
                    gdaQueryProxy.Open();
                }
                catch
                {

                }
            }

            ComboBoxGid1 = GetAllGids();
            ComboBoxModelCode = Enum.GetValues(typeof(DMSType)).Cast<DMSType>().ToList().FindAll(t => t != DMSType.MASK_TYPE);
            ComboBoxGid3 = GetAllGids();

            DataContext = this;
        }

        #region GetValue

        private long gid1;
        private List<long> comboBox1 = new List<long>();
        private List<ModelCode> property1 = new List<ModelCode>();

        public long Gid1 {
            get { return gid1; }
            set
            {
                gid1 = value;
                OnPropertyChanged("Gid1");
                OnPropertyChanged("Property1");
            }
        }
        public List<long> ComboBoxGid1
        {
            get { return comboBox1; }
            set
            {
                comboBox1 = value;
                OnPropertyChanged("ComboBoxGid1");
            }
        }
        public List<ModelCode> Property1 {
            get
            {
                if(gid1!=0)
                {
                    ModelResourcesDesc modResDes = new ModelResourcesDesc();
                    List<ModelCode> lista = modResDes.GetAllPropertyIdsForEntityId(Gid1);

                    return lista;
                }
                return null;
            }
            set
            {
                property1 = value;
                OnPropertyChanged("Property1");
                OnPropertyChanged("Gid1");
            }
        }  
        public List<long> GetAllGids()
        {
            ModelResourcesDesc modelResourcesDesc = new ModelResourcesDesc();
            List<ModelCode> properties = new List<ModelCode>();
            List<long> ids = new List<long>();

            int iteratorId = 0;
            int numberOfResources = 1000;
            DMSType currType = 0;
            properties.Add(ModelCode.IDOBJ_GID);
            try
            {
                foreach (DMSType type in Enum.GetValues(typeof(DMSType)))
                {
                    currType = type;

                    if (type != DMSType.MASK_TYPE)
                    {
                        iteratorId = GDAQueryProxy.GetExtentValues(modelResourcesDesc.GetModelCodeFromType(type), properties);
                        int count = GDAQueryProxy.IteratorResourcesLeft(iteratorId);

                        while (count > 0)
                        {
                            List<ResourceDescription> rds = GDAQueryProxy.IteratorNext(numberOfResources, iteratorId);

                            for (int i = 0; i < rds.Count; i++)
                            {
                                ids.Add(rds[i].Id);
                            }

                            count = GDAQueryProxy.IteratorResourcesLeft(iteratorId);
                        }

                        bool ok = GDAQueryProxy.IteratorClose(iteratorId);

                    }
                }
            }

            catch (Exception)
            {
                throw;
            }

            return ids;
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            if (listBox1.SelectedItems == null || Gid1 == 0)
            {
                MessageBox.Show("You must chose a propertie");
                return;
            }

            List<ModelCode> l = new List<ModelCode>();
            foreach (var v in listBox1.SelectedItems)
            {
                l.Add((ModelCode)v);
            }


            result1.Text = GetValues(Gid1, l);
        }
        public string GetValues(long globalId, List<ModelCode> props)
        {
            ResourceDescription rd = null;
            string ss = "";
            bool gidBool = true;
            List<ModelCode> properties = new List<ModelCode>();
            try
            {
                short type = ModelCodeHelper.ExtractTypeFromGlobalId(globalId);
                properties = props;
                if (props.Contains(ModelCode.IDOBJ_GID) == false)
                {
                    properties.Add(ModelCode.IDOBJ_GID);
                    gidBool = false;
                }
                rd = GDAQueryProxy.GetValues(globalId, properties);
                ss += String.Format("Item with gid: 0x{0:x16}:\n", globalId);
                foreach (Property p in rd.Properties)
                {
                    ss += String.Format("\t{0} =", p.Id);
                    switch (p.Type)
                    {
                        case PropertyType.Float:
                            ss += String.Format(" {0}:\n", p.AsFloat());
                            break;
                        case PropertyType.Bool:
                            ss += String.Format("{0}\n", p.AsBool());
                            break;
                        case PropertyType.Int32:
                            if (p.Id == ModelCode.IDOBJ_GID)                            
                                ss += (String.Format("0x{0:x16}\n", p.AsLong()));                            
                            else
                                ss += String.Format("{0}\n", p.AsLong());
                            break;
                        case PropertyType.Int64:
                            if (p.Id == ModelCode.IDOBJ_GID)                           
                                ss += (String.Format("0x{0:x16}\n", p.AsLong()));                            
                            else
                                ss += String.Format("{0}\n", p.AsLong());
                            break;
                        case PropertyType.DateTime:
                            if (p.Id == ModelCode.IDOBJ_GID)                            
                                ss += (String.Format("0x{0:x16}\n", p.AsLong()));                            
                            else                            
                                ss += String.Format("{0}\n", p.AsDateTime());                                                 
                            break;                        
                        case PropertyType.Reference:
                            ss += (String.Format("0x{0:x16}\n", p.AsReference()));
                            break;
                        case PropertyType.String:
                            if (p.PropertyValue.StringValue == null)                            
                                p.PropertyValue.StringValue = String.Empty;                            
                            ss += String.Format("{0}\n", p.AsString());
                            break;
                        case PropertyType.Enum:
                            if(p.Id == ModelCode.REGULATINGCONTROL_MODE)
                                ss += String.Format("{0}\n", (RegulatingControlModeKind)p.AsEnum());
                            else if(p.Id == ModelCode.REGULATINGCONTROL_MONITOREDPHASE)
                                ss += String.Format("{0}\n", (PhaseCode)p.AsEnum());
                            break;
                        case PropertyType.ReferenceVector:
                            if (p.AsLongs().Count > 0)
                            {
                                string s = "";
                                for (int j = 0; j < p.AsLongs().Count; j++)
                                {
                                    s += (String.Format("0x{0:x16},\n", p.AsLongs()[j]));
                                }

                                ss += s;// (sb.ToString(0, sb.Length - 2));
                            }
                            else                            
                                ss += ("empty long/reference vector\n");                            
                            break;

                        default:
                            throw new Exception("Failed to export Resource Description as XML. Invalid property type.");
                    }
                }
            }
            catch (Exception)
            {

            }

            return ss;
        }
        #endregion

        #region GetExtendValue

        private List<DMSType> comboBox2 = new List<DMSType>();
        private DMSType modelCode2;
        private List<ModelCode> property2 = new List<ModelCode>();

        public List<DMSType> ComboBoxModelCode
        {
            get
            {
                return comboBox2;
            }
            set
            {
                comboBox2 = value;
                OnPropertyChanged("ComboBoxModelCode");
            }
        }
        public DMSType ModelCode2
        {
            get { return modelCode2; }
            set
            {
                modelCode2 = value;
                OnPropertyChanged("ModelCode2");
                OnPropertyChanged("Property2");
            }
        }
        public List<ModelCode> Property2
        {
            get
            {
                if (modelCode2 != 0)
                {
                    ModelResourcesDesc modResDes = new ModelResourcesDesc();
                    List<ModelCode> lista = modResDes.GetAllPropertyIds(modelCode2);
                    return lista;
                }
                return null;
            }
            set
            {
                property2 = value;
                OnPropertyChanged("Property2");
                OnPropertyChanged("ModelKod2");
            }
        }
        public string GetExtentValues(DMSType type, List<ModelCode> props)
        {
            int iteratorId = 0;
            List<long> ids = new List<long>();
            string ss = "";
            bool gidBool = true;
            ModelCode modelCode = modelResourcesDesc.GetModelCodeFromType(type);
            try
            {
                int numberOfResources = 2;
                int resourcesLeft = 0;

                List<ModelCode> properties = props;// modelResourcesDesc.GetAllPropertyIds(modelCode);
                if (props.Contains(ModelCode.IDOBJ_GID) == false)
                {
                    properties.Add(ModelCode.IDOBJ_GID);
                    gidBool = false;
                }
                iteratorId = GDAQueryProxy.GetExtentValues(modelCode, properties);
                resourcesLeft = GDAQueryProxy.IteratorResourcesLeft(iteratorId);
                ss += String.Format("Items with ModelCode: {0}:\n", modelCode.ToString());
                while (resourcesLeft > 0)
                {
                    List<ResourceDescription> rds = GDAQueryProxy.IteratorNext(numberOfResources, iteratorId);

                    for (int i = 0; i < rds.Count; i++)
                    {
                        ss += String.Format("\tItem with gid: 0x{0:x16}\n", rds[i].Properties.Find(r => r.Id == ModelCode.IDOBJ_GID).AsLong());
                        foreach (Property p in rds[i].Properties)
                        {
                            if (p.Id == ModelCode.IDOBJ_GID && gidBool == false)
                            {

                            }
                            else
                            {
                                ss += String.Format("\t\t{0} =", p.Id);
                                switch (p.Type)
                                {
                                    case PropertyType.Float:
                                        ss += String.Format(" {0}:\n", p.AsFloat());
                                        break;
                                    case PropertyType.Bool:
                                        ss += String.Format("{0}\n", p.AsBool());
                                        break;
                                    case PropertyType.Int32:
                                        if (p.Id == ModelCode.IDOBJ_GID)                                        
                                            ss += (String.Format("0x{0:x16}\n", p.AsLong()));                                        
                                        else
                                            ss += String.Format("{0}\n", p.AsLong());
                                        break;
                                    case PropertyType.Int64:
                                        if (p.Id == ModelCode.IDOBJ_GID)                                        
                                            ss += (String.Format("0x{0:x16}\n", p.AsLong()));                                        
                                        else
                                            ss += String.Format("{0}\n", p.AsLong());
                                        break;
                                    case PropertyType.DateTime:
                                        if (p.Id == ModelCode.IDOBJ_GID)                                        
                                            ss += (String.Format("0x{0:x16}\n", p.AsLong()));                                        
                                        else                                        
                                            ss += String.Format("{0}\n", p.AsDateTime());                                        
                                        break;
                                    case PropertyType.Reference:
                                        ss += (String.Format("0x{0:x16}\n", p.AsReference()));
                                        break;
                                    case PropertyType.String:
                                        if (p.PropertyValue.StringValue == null)                                        
                                            p.PropertyValue.StringValue = String.Empty;                                        
                                        ss += String.Format("{0}\n", p.AsString());
                                        break;
                                    case PropertyType.Enum:
                                        if (p.Id == ModelCode.REGULATINGCONTROL_MODE)
                                            ss += String.Format("{0}\n", (RegulatingControlModeKind)p.AsEnum());
                                        else if (p.Id == ModelCode.REGULATINGCONTROL_MONITOREDPHASE)
                                            ss += String.Format("{0}\n", (PhaseCode)p.AsEnum());
                                        break;
                                    case PropertyType.ReferenceVector:
                                        if (p.AsLongs().Count > 0)
                                        {
                                            string s = "";
                                            for (int j = 0; j < p.AsLongs().Count; j++)
                                            {
                                                s += (String.Format("0x{0:x16},\n", p.AsLongs()[j]));
                                            }
                                            ss += s;//(sb.ToString(0, sb.Length - 2));
                                        }
                                        else                                        
                                            ss += ("empty long/reference vector\n");                                        
                                        break;

                                    default:
                                        throw new Exception("Failed to export Resource Description as XML. Invalid property type.");
                                }
                            }
                        }
                    }
                    resourcesLeft = GDAQueryProxy.IteratorResourcesLeft(iteratorId);
                }
                GDAQueryProxy.IteratorClose(iteratorId);
            }
            catch (Exception) { }
            
            return ss;
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            if (listBox2.SelectedItems == null || ModelCode2 == 0)
            {
                MessageBox.Show("You must chose a propertie");
                return;
            }

            List<ModelCode> l = new List<ModelCode>();
            foreach (var v in listBox2.SelectedItems)
            {
                l.Add((ModelCode)v);
            }
            result2.Text = GetExtentValues(ModelCode2, l);

        }
        #endregion

        #region GetRelatedValue

        private List<long> comboBox3 = new List<long>();
        private long gid3;
        private List<ModelCode> comboBoxPropertyID = new List<ModelCode>();
        private ModelCode propertyID;
        private List<ModelCode> comboBoxType = new List<ModelCode>();
        private ModelCode type;
        private List<ModelCode> property3 = new List<ModelCode>();

        public List<long> ComboBoxGid3
        {
            get { return comboBox3; }
            set
            {
                comboBox3 = value;
                OnPropertyChanged("ComboBoxGid3");
            }
        }
        public long Gid3
        {
            get { return gid3; }
            set
            {
                gid3 = value;
                OnPropertyChanged("Gid3");
                OnPropertyChanged("ComboBoxPropertyID");
                OnPropertyChanged("ComboBoxType");
                OnPropertyChanged("Property3");
            }
        }
        public List<ModelCode> ComboBoxPropertyID
        {
            get
            {
                if(gid3 != 0)
                {
                    ModelResourcesDesc modResDes = new ModelResourcesDesc();
                    List<ModelCode> lista = modResDes.GetAllPropertyIdsForEntityId(gid3);
                    List<ModelCode> rez = new List<ModelCode>();

                    foreach (ModelCode mc in lista)
                    {
                        if (Property.GetPropertyType(mc) == PropertyType.Reference || Property.GetPropertyType(mc) == PropertyType.ReferenceVector)
                        {
                            rez.Add(mc);
                        }

                    }

                    return rez;
                }
                return null;
            }
            set
            {
                comboBoxPropertyID = value;
                OnPropertyChanged("ComboBoxPropertyID");
                OnPropertyChanged("ComboBoxType");
            }
        }
        public ModelCode PropertyID
        {
            get { return propertyID; }
            set
            {
                propertyID = value;
                OnPropertyChanged("PropertyID");
                NadjiTipove(propertyID);
                OnPropertyChanged("ComboBoxType");
            }
        }
        public List<ModelCode> ComboBoxType
        {
            get { return comboBoxType; }
            set
            {
                comboBoxType = value;
                OnPropertyChanged("ComboBoxType");
                OnPropertyChanged("Property3");
            }
        }
        public ModelCode Type
        {
            get { return type; }
            set
            {
                type = value;
                OnPropertyChanged("Type");
                OnPropertyChanged("Property3");
            }
        }
        public List<ModelCode> Property3
        {
            get
            {
                if(type != 0)
                {
                    ModelResourcesDesc modResDes = new ModelResourcesDesc();
                    List<ModelCode> lista = modResDes.GetAllPropertyIds(type);

                    return lista;
                }
                return null;
            }
            set
            {
                property3 = value;
                OnPropertyChanged("Property3");
            }
        }

        private List<ModelCode> NadjiTipove(ModelCode kodProp)
        {
            ModelResourcesDesc modResDes = new ModelResourcesDesc();
            //List<DMSType> tip = new List<DMSType>();

            string[] props = (kodProp.ToString()).Split('_');
            props[1] = props[1].TrimEnd('S');

            DMSType propertyCode = ModelResourcesDesc.GetTypeFromModelCode(kodProp);


            ModelCode mc;
            ModelCodeHelper.GetModelCodeFromString(propertyCode.ToString(), out mc);

            foreach (ModelCode modelCode in Enum.GetValues(typeof(ModelCode)))
            {

                if ((String.Compare(modelCode.ToString(), mc.ToString()) != 0) && (String.Compare(kodProp.ToString(), modelCode.ToString()) != 0) && (String.Compare(props[1], modelCode.ToString())) == 0)
                {
                    DMSType type = ModelCodeHelper.GetTypeFromModelCode(modelCode);
                    if (type == 0)
                    {
                        NadjiKonkretne(modelCode);
                    }
                    else
                    {
                        comboBoxType = new List<ModelCode>();
                        comboBoxType.Add(modelCode);
                    }

                }
            }


            return new List<ModelCode>();
        }
        private void NadjiKonkretne(ModelCode modelCode)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("0x");
            List<ModelCode> retCodes = new List<ModelCode>();

            long lmc = (long)modelCode;
            string smc = String.Format("0x{0:x16}", lmc);

            string[] newS = smc.Split('x');
            char[] c = (newS[1]).ToCharArray();


            foreach (char ch in c)
            {
                if (ch != '0')
                {
                    sb.Append(ch);
                }
                else
                {
                    break;
                }

            }

            foreach (ModelCode mc in Enum.GetValues(typeof(ModelCode)))
            {
                DMSType type = ModelCodeHelper.GetTypeFromModelCode(mc);
                short sh = (short)mc;
                if ((modelCode != mc) && (sh == 0) && (type != 0))
                {
                    lmc = (long)mc;
                    smc = String.Format("0x{0:x16}", lmc);
                    if (smc.StartsWith(sb.ToString()))
                    {
                        retCodes.Add(mc);

                    }
                }
            }
            comboBoxType = retCodes;
        }

        private void Button3_Click(object sender, RoutedEventArgs e)
        {
            if (listBox3.SelectedItems == null || PropertyID == 0 || Gid3 == 0 || Type == 0)
            {
                MessageBox.Show("You must chose a propertie");
                return;
            }

            List<ModelCode> l = new List<ModelCode>();
            foreach (var v in listBox3.SelectedItems)
            {
                l.Add((ModelCode)v);
            }

            Association association = new Association();
            association.PropertyId = PropertyID;
            association.Type = Type;

            result3.Text = GetRelatedValues(Gid3, association, l);
        }
        public string GetRelatedValues(long sourceGlobalId, Association association, List<ModelCode> props)
        {

            string ss = "";
            int numberOfResources = 2;
            bool gidBool = true;
            try
            {
                List<ModelCode> properties = props;
                if (props.Contains(ModelCode.IDOBJ_GID) == false)
                {
                    properties.Add(ModelCode.IDOBJ_GID);
                    gidBool = false;
                }
                int iteratorId = GDAQueryProxy.GetRelatedValues(sourceGlobalId, properties, association);
                int resourcesLeft = GDAQueryProxy.IteratorResourcesLeft(iteratorId);

                while (resourcesLeft > 0)
                {
                    List<ResourceDescription> rds = GDAQueryProxy.IteratorNext(numberOfResources, iteratorId);

                    for (int i = 0; i < rds.Count; i++)
                    {
                        ss += String.Format("Item with gid: 0x{0:x16}\n", rds[i].Properties.Find(r => r.Id == ModelCode.IDOBJ_GID).AsLong());
                        foreach (Property p in rds[i].Properties)
                        {
                            if (p.Id == ModelCode.IDOBJ_GID && gidBool == false)
                            {

                            }
                            else
                            {
                                ss += String.Format("\t{0} =", p.Id);
                                switch (p.Type)
                                {
                                    case PropertyType.Float:
                                        ss += String.Format(" {0}:\n", p.AsFloat());
                                        break;
                                    case PropertyType.Bool:
                                        ss += String.Format("{0}\n", p.AsBool());
                                        break;
                                    case PropertyType.Int32:
                                        if (p.Id == ModelCode.IDOBJ_GID)                                        
                                            ss += (String.Format("0x{0:x16}\n", p.AsLong()));                                        
                                        else
                                            ss += String.Format("{0}\n", p.AsLong());
                                        break;
                                    case PropertyType.Int64:
                                        if (p.Id == ModelCode.IDOBJ_GID)                                        
                                            ss += (String.Format("0x{0:x16}\n", p.AsLong()));                                        
                                        else                                        
                                            ss += String.Format("{0}\n", p.AsLong());                                        
                                        break;
                                    case PropertyType.DateTime:
                                        if (p.Id == ModelCode.IDOBJ_GID)                                       
                                            ss += (String.Format("0x{0:x16}\n", p.AsLong()));                                        
                                        else                                        
                                            ss += String.Format("{0}\n", p.AsDateTime());                                        
                                        break;
                                    case PropertyType.Reference:
                                        ss += (String.Format("0x{0:x16}\n", p.AsReference()));
                                        break;
                                    case PropertyType.String:
                                        if (p.PropertyValue.StringValue == null)                                        
                                            p.PropertyValue.StringValue = String.Empty;                                        
                                        ss += String.Format("{0}\n", p.AsString());
                                        break;
                                    case PropertyType.Enum:
                                        if (p.Id == ModelCode.REGULATINGCONTROL_MODE)
                                            ss += String.Format("{0}\n", (RegulatingControlModeKind)p.AsEnum());
                                        else if (p.Id == ModelCode.REGULATINGCONTROL_MONITOREDPHASE)
                                            ss += String.Format("{0}\n", (PhaseCode)p.AsEnum());
                                        break;
                                    case PropertyType.ReferenceVector:
                                        if (p.AsLongs().Count > 0)
                                        {
                                            string s = "";
                                            for (int j = 0; j < p.AsLongs().Count; j++)
                                            {
                                                s += (String.Format("0x{0:x16},\n", p.AsLongs()[j]));
                                            }
                                            ss += s;//(sb.ToString(0, sb.Length - 2));
                                        }
                                        else                                        
                                            ss += ("empty long/reference vector\n");                                        
                                        break;

                                    default:
                                        throw new Exception("Failed to export Resource Description as XML. Invalid property type.");
                                }
                            }
                        }
                    }
                    resourcesLeft = GDAQueryProxy.IteratorResourcesLeft(iteratorId);
                }
                GDAQueryProxy.IteratorClose(iteratorId);
            }
            catch (Exception) { }
            
            if (ss == "")
                return "Ooops...\nThis is empty.\nTry another :)";
            return ss;
        }
        #endregion


    }
}
