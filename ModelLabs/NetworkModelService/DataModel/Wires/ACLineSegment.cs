using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTN.Services.NetworkModelService.DataModel.Wires
{
    public class ACLineSegment : Conductor
    {
        private float b0ch;
        private float bch;
        private float g0ch;
        private float gch;
        private float r;
        private float r0;
        private float x;
        private float x0;

        public float B0ch { get => b0ch; set => b0ch = value; }
        public float Bch { get => bch; set => bch = value; }
        public float G0ch { get => g0ch; set => g0ch = value; }
        public float Gch { get => gch; set => gch = value; }
        public float R { get => r; set => r = value; }
        public float R0 { get => r0; set => r0 = value; }
        public float X { get => x; set => x = value; }
        public float X0 { get => x0; set => x0 = value; }

        private long perLenImp = 0;
        public long PerLenImp { get => perLenImp; set => perLenImp = value; }

        public ACLineSegment(long globalId) : base(globalId) { }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
            {
                ACLineSegment x = obj as ACLineSegment;
                return (x.b0ch == this.b0ch &&
                        x.bch == this.bch &&
                        x.g0ch == this.g0ch &&
                        x.gch == this.gch &&
                        x.r == this.r &&
                        x.r0 == this.r0 &&
                        x.x == this.x &&
                        x.x0 == this.x0 &&
                        x.perLenImp == this.perLenImp);
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
                case ModelCode.ACLINESEGMENT_B0CH:
                case ModelCode.ACLINESEGMENT_BCH:
                case ModelCode.ACLINESEGMENT_G0CH:
                case ModelCode.ACLINESEGMENT_GCH:
                case ModelCode.ACLINESEGMENT_R:
                case ModelCode.ACLINESEGMENT_R0:
                case ModelCode.ACLINESEGMENT_X:
                case ModelCode.ACLINESEGMENT_X0:
                case ModelCode.ACLINESEGMENT_PERLENGTHIMPENDANCE:
                    return true;
                default:
                    return base.HasProperty(property);
            }
        }

        public override void GetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.ACLINESEGMENT_B0CH:
                    property.SetValue(b0ch);
                    break;
                case ModelCode.ACLINESEGMENT_BCH:
                    property.SetValue(bch);
                    break;
                case ModelCode.ACLINESEGMENT_G0CH:
                    property.SetValue(g0ch);
                    break;
                case ModelCode.ACLINESEGMENT_GCH:
                    property.SetValue(gch);
                    break;
                case ModelCode.ACLINESEGMENT_R:
                    property.SetValue(r);
                    break;
                case ModelCode.ACLINESEGMENT_R0:
                    property.SetValue(r0);
                    break;
                case ModelCode.ACLINESEGMENT_X:
                    property.SetValue(x);
                    break;
                case ModelCode.ACLINESEGMENT_X0:
                    property.SetValue(x0);
                    break;
                case ModelCode.ACLINESEGMENT_PERLENGTHIMPENDANCE:
                    property.SetValue(perLenImp);
                    break;
                default:
                    base.GetProperty(property);
                    break;
            }
        }

        public override void SetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.ACLINESEGMENT_B0CH:
                    b0ch = property.AsFloat();
                    break;
                case ModelCode.ACLINESEGMENT_BCH:
                    bch = property.AsFloat();
                    break;
                case ModelCode.ACLINESEGMENT_G0CH:
                    g0ch = property.AsFloat();
                    break;
                case ModelCode.ACLINESEGMENT_GCH:
                    gch = property.AsFloat();
                    break;
                case ModelCode.ACLINESEGMENT_R:
                    r = property.AsFloat();
                    break;
                case ModelCode.ACLINESEGMENT_R0:
                    r0 = property.AsFloat();
                    break;
                case ModelCode.ACLINESEGMENT_X:
                    x = property.AsFloat();
                    break;
                case ModelCode.ACLINESEGMENT_X0:
                    x0 = property.AsFloat();
                    break;
                case ModelCode.ACLINESEGMENT_PERLENGTHIMPENDANCE:
                    perLenImp = property.AsReference();
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
            if (perLenImp != 0 && (refType == TypeOfReference.Both || refType == TypeOfReference.Target))
            {
                references[ModelCode.ACLINESEGMENT_PERLENGTHIMPENDANCE] = new List<long> { perLenImp };
            }
            base.GetReferences(references, refType);
        }
        #endregion IReference implementation
    }
}
