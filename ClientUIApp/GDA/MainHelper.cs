using System;
using FTN.Common;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientUIApp.GDA
{
    public class MainHelper
    {
        public static List<ModelCode> AllTypesModelCodes = new List<ModelCode>()
        {
            ModelCode.TERMINAL,
            ModelCode.REGULATINGCONTROL,
            ModelCode.STATICVARCOMPENSATOR,
            ModelCode.CONTROL,
            ModelCode.REACTIVECAPABILITYCURVE,
            ModelCode.SHUNTCOMPENSATOR,
            ModelCode.SYNCHRONOUSMACHINE
        };
    }
}
