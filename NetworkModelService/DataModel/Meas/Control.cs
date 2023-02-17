using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FTN.Services.NetworkModelService.DataModel.Meas
{
    public class Control : IdentifiedObject
    {
        private long regulatingCondEq = 0;

        public Control(long globalId) : base(globalId)
        {
        }

        public long RegulatingCondEq
        {
            get { return regulatingCondEq; }
            set { regulatingCondEq = value; }
        }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
            {
                Control x = (Control)obj;
                return (x.regulatingCondEq == this.regulatingCondEq);
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
                case ModelCode.CONTROL_REGULATINGCONDEQ:
                    return true;

                default:
                    return base.HasProperty(property);
            }
        }

        public override void GetProperty(Property prop)
        {
            switch (prop.Id)
            {
                case ModelCode.CONTROL_REGULATINGCONDEQ:
                    prop.SetValue(regulatingCondEq);
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
                case ModelCode.CONTROL_REGULATINGCONDEQ:
                    regulatingCondEq = property.AsReference();
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
            if (regulatingCondEq != 0 && (refType == TypeOfReference.Reference || refType == TypeOfReference.Both))
            {
                references[ModelCode.CONTROL_REGULATINGCONDEQ] = new List<long>();
                references[ModelCode.CONTROL_REGULATINGCONDEQ].Add(regulatingCondEq);
            }

            base.GetReferences(references, refType);
        }

        #endregion IReference implementation
    }
}
