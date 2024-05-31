using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Wires;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTN.Services.NetworkModelService.DataModel.Core
{
    public class SubGeographicalRegion : IdentifiedObject
    {
        private List<long> lines = new List<long>();
        public List<long> Lines { get => lines; set => lines = value; }

        public SubGeographicalRegion(long globalId) : base(globalId) { }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
            {
                SubGeographicalRegion x = (SubGeographicalRegion)obj;
                return (CompareHelper.CompareLists(x.lines, this.lines, true));
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
                case ModelCode.SUBGEOGRAPHICALREGION_LINE:
                    return true;

                default:
                    return base.HasProperty(property);
            }
        }

        public override void GetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.SUBGEOGRAPHICALREGION_LINE:
                    property.SetValue(lines);
                    break;

                default:
                    base.GetProperty(property);
                    break;
            }
        }

        public override void SetProperty(Property property)
        {
            base.SetProperty(property);
        }
        #endregion IAccess implementation

        #region IReference implementation
        public override bool IsReferenced
        {
            get
            {
                return (lines.Count > 0 || base.IsReferenced);
            }
        }

        public override void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
        {
            if (lines != null && lines.Count > 0 && (refType == TypeOfReference.Target || refType == TypeOfReference.Both))
            {
                references[ModelCode.SUBGEOGRAPHICALREGION_LINE] = lines.GetRange(0, lines.Count);
            }

            base.GetReferences(references, refType);
        }

        public override void AddReference(ModelCode referenceId, long globalId)
        {
            switch (referenceId)
            {
                case ModelCode.LINE_REGION:
                    lines.Add(globalId);
                    break;

                default:
                    base.AddReference(referenceId, globalId);
                    break;
            }
        }

        public override void RemoveReference(ModelCode referenceId, long globalId)
        {
            switch (referenceId)
            {
                case ModelCode.LINE_REGION:
                    if (lines.Contains(globalId))
                        lines.Remove(globalId);
                    else
                        CommonTrace.WriteTrace(CommonTrace.TraceWarning, "Entity (GID = 0x{0:x16}) doesn't contain reference 0x{1:x16}.", this.GlobalId, globalId);
                    break;

                default:
                    base.RemoveReference(referenceId, globalId);
                    break;
            }
        }
        #endregion IReference implementation
    }
}
