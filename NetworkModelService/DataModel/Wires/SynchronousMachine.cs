using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FTN.Services.NetworkModelService.DataModel.Wires
{
    public class SynchronousMachine : RotatingMachine
    {
        private long reactiveCapabilityCurves = 0;

        public SynchronousMachine(long globalId) : base(globalId)
        {
        }

        public long ReactiveCapabilityCurves
        {
            get { return reactiveCapabilityCurves; }
            set { reactiveCapabilityCurves = value; }
        }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
            {
                SynchronousMachine x = (SynchronousMachine)obj;
                return (x.reactiveCapabilityCurves == this.reactiveCapabilityCurves);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #region IAccess implementation

        public override bool HasProperty(ModelCode property)
        {
            switch (property)
            {
                case ModelCode.SYNCHRONOUSMACHINE_REACTIVECAPABILITYCURVES:
                    return true;

                default:
                    return base.HasProperty(property);
            }
        }

        public override void GetProperty(Property prop)
        {
            switch (prop.Id)
            {
                case ModelCode.SYNCHRONOUSMACHINE_REACTIVECAPABILITYCURVES:
                    prop.SetValue(reactiveCapabilityCurves);
                    break;

                default:
                    base.GetProperty(prop);
                    break;
            }
        }

        public override void SetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.SYNCHRONOUSMACHINE_REACTIVECAPABILITYCURVES:
                    reactiveCapabilityCurves = property.AsReference();
                    break;

                default:
                    base.SetProperty(property);
                    break;
            }
        }

        #endregion IAccess implementation

        #region IReference implementation

        public override void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
        {
            if (reactiveCapabilityCurves != 0 && (refType == TypeOfReference.Reference || refType == TypeOfReference.Both))
            {
                references[ModelCode.SYNCHRONOUSMACHINE_REACTIVECAPABILITYCURVES] = new List<long>();
                references[ModelCode.SYNCHRONOUSMACHINE_REACTIVECAPABILITYCURVES].Add(reactiveCapabilityCurves);
            }

            base.GetReferences(references, refType);
        }

        #endregion IReference implementation
    }
}
