namespace FTN.ESI.SIMES.CIM.CIMAdapter.Importer
{
    using FTN.Common;

    public static class Projekat46Converter
    {
        #region Populate ResourceDescription
        // IDENTIFIED OBJECT
        public static void PopulateIdentifiedObjectProperties(FTN.IdentifiedObject cimIdentifiedObject, ResourceDescription rd)
        {
            if ((cimIdentifiedObject != null) && (rd != null))
            {
                if (cimIdentifiedObject.AliasNameHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.IDOBJ_ALIASNAME, cimIdentifiedObject.AliasName));
                }
                if (cimIdentifiedObject.MRIDHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.IDOBJ_MRID, cimIdentifiedObject.MRID));
                }
                if (cimIdentifiedObject.NameHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.IDOBJ_NAME, cimIdentifiedObject.Name));
                }
            }
        }

        // POWER SYSTEM RESOURCE
        public static void PopulatePowerSystemResourceProperties(FTN.PowerSystemResource cimPowerSystemResource, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
            if ((cimPowerSystemResource != null) && (rd != null))
            {
                Projekat46Converter.PopulateIdentifiedObjectProperties(cimPowerSystemResource, rd);
            }
        }

        // CONNECTIVITY NODE CONTAINER
        public static void PopulateConnectivityNodeContainerProperties(ConnectivityNodeContainer cimNodeContainer, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
            if ((cimNodeContainer != null) && (rd != null))
            {
                Projekat46Converter.PopulatePowerSystemResourceProperties(cimNodeContainer, rd, importHelper, report);
            }
        }

        // EQUIPMENT CONTAINER
        public static void PopulateEquipmentContainerProperties(EquipmentContainer cimEqContainer, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
            if ((cimEqContainer != null) && (rd != null))
            {
                Projekat46Converter.PopulateConnectivityNodeContainerProperties(cimEqContainer, rd, importHelper, report);
            }
        }

        // LINE
        public static void PopulateLineProperties(FTN.Line cimLine, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
            if ((cimLine != null) && (rd != null))
            {
                Projekat46Converter.PopulateEquipmentContainerProperties(cimLine, rd, importHelper, report);

                // Reference
                if (cimLine.RegionHasValue)
                {
                    long gid = importHelper.GetMappedGID(cimLine.Region.ID);
                    if (gid < 0)
                    {
                        report.Report.Append("WARNING: Convert ").Append(cimLine.GetType().ToString()).Append(" rdfID = \"").Append(cimLine.ID);
                        report.Report.Append("\" - Failed to set reference to EquipmentContainer: rdfID \"").Append(cimLine.Region.ID).AppendLine(" \" is not mapped to GID!");
                    }
                    rd.AddProperty(new Property(ModelCode.LINE_REGION, gid));
                }
            }
        }

        // SUB GEOGRAPHICAL REGION
        public static void PopulateRegionProperties(FTN.SubGeographicalRegion cimRegion, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
            if ((cimRegion != null) && (rd != null))
            {
                Projekat46Converter.PopulateIdentifiedObjectProperties(cimRegion, rd);
            }
        }

        // EQUIPMENT
        public static void PopulateEquipmentProperties(FTN.Equipment cimEquipment, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
            if ((cimEquipment != null) && (rd != null))
            {
                Projekat46Converter.PopulatePowerSystemResourceProperties(cimEquipment, rd, importHelper, report);

                // Reference
                if (cimEquipment.EquipmentContainerHasValue)
                {
                    long gid = importHelper.GetMappedGID(cimEquipment.EquipmentContainer.ID);
                    if (gid < 0)
                    {
                        report.Report.Append("WARNING: Convert ").Append(cimEquipment.GetType().ToString()).Append(" rdfID = \"").Append(cimEquipment.ID);
                        report.Report.Append("\" - Failed to set reference to EquipmentContainer: rdfID \"").Append(cimEquipment.EquipmentContainer.ID).AppendLine(" \" is not mapped to GID!");
                    }
                    rd.AddProperty(new Property(ModelCode.EQUIPMENT_EQUIPMENTCONTAINER, gid));
                }
            }
        }

        // CONDUCTING EQUIPMENT
        public static void PopulateConductingEquipmentProperties(FTN.ConductingEquipment cimConductingEquipment, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
            if ((cimConductingEquipment != null) && (rd != null))
            {
                Projekat46Converter.PopulateEquipmentProperties(cimConductingEquipment, rd, importHelper, report);
            }
        }

        // CONDUCTOR
        public static void PopulateConductorProperties(FTN.Conductor cimConductor, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
            if ((cimConductor != null) && (rd != null))
            {
                Projekat46Converter.PopulateConductingEquipmentProperties(cimConductor, rd, importHelper, report);

                if (cimConductor.LengthHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.CONDUCTOR_LENGTH, cimConductor.Length));
                }
            }
        }

        // DC LINE SEGMENT
        public static void PopulateDCLineSegmentProperties(FTN.DCLineSegment cimDCLineSegment, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
            if ((cimDCLineSegment != null) && (rd != null))
            {
                Projekat46Converter.PopulateConductorProperties(cimDCLineSegment, rd, importHelper, report);
            }
        }

        // AC LINE SEGMENT
        public static void PopulateACLineSegmentProperties(FTN.ACLineSegment cimACLineSegment, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
            if ((cimACLineSegment != null) && (rd != null))
            {
                Projekat46Converter.PopulateConductorProperties(cimACLineSegment, rd, importHelper, report);

                if (cimACLineSegment.B0chHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.ACLINESEGMENT_B0CH, cimACLineSegment.B0ch));
                }
                if (cimACLineSegment.BchHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.ACLINESEGMENT_BCH, cimACLineSegment.Bch));
                }
                if (cimACLineSegment.G0chHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.ACLINESEGMENT_G0CH, cimACLineSegment.G0ch));
                }
                if (cimACLineSegment.GchHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.ACLINESEGMENT_GCH, cimACLineSegment.Gch));
                }
                if (cimACLineSegment.RHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.ACLINESEGMENT_R, cimACLineSegment.R));
                }
                if (cimACLineSegment.R0HasValue)
                {
                    rd.AddProperty(new Property(ModelCode.ACLINESEGMENT_R0, cimACLineSegment.R0));
                }
                if (cimACLineSegment.XHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.ACLINESEGMENT_X, cimACLineSegment.X));
                }
                if (cimACLineSegment.X0HasValue)
                {
                    rd.AddProperty(new Property(ModelCode.ACLINESEGMENT_X0, cimACLineSegment.X0));
                }

                // Referenca na PLI
                if (cimACLineSegment.PerLengthImpedanceHasValue)
                {
                    long gid = importHelper.GetMappedGID(cimACLineSegment.PerLengthImpedance.ID);
                    if (gid < 0)
                    {
                        report.Report.Append("WARNING: Convert ").Append(cimACLineSegment.GetType().ToString()).Append(" rdfID = \"").Append(cimACLineSegment.ID);
                        report.Report.Append("\" - Failed to set reference to PerLengthImpedance: rdfID \"").Append(cimACLineSegment.PerLengthImpedance.ID).AppendLine(" \" is not mapped to GID!");
                    }
                    rd.AddProperty(new Property(ModelCode.ACLINESEGMENT_PERLENGTHIMPENDANCE, gid));
                }
            }
        }

        // SERIES COMPENSATOR
        public static void PopulateSeriesCompensatorProperties(FTN.SeriesCompensator cimSeriesCompensator, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
            if ((cimSeriesCompensator != null) && (rd != null))
            {
                Projekat46Converter.PopulateConductingEquipmentProperties(cimSeriesCompensator, rd, importHelper, report);

                if (cimSeriesCompensator.RHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.SERIESCOMPENSATOR_R, cimSeriesCompensator.R));
                }
                if (cimSeriesCompensator.R0HasValue)
                {
                    rd.AddProperty(new Property(ModelCode.SERIESCOMPENSATOR_R0, cimSeriesCompensator.R0));
                }
                if (cimSeriesCompensator.XHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.SERIESCOMPENSATOR_X, cimSeriesCompensator.X));
                }
                if (cimSeriesCompensator.X0HasValue)
                {
                    rd.AddProperty(new Property(ModelCode.SERIESCOMPENSATOR_X0, cimSeriesCompensator.X0));
                }
            }
        }

        // PER LENGTH IMPENDANCE
        public static void PopulatePerLengthImpedanceProperties(FTN.PerLengthImpedance cimPerLengthImpedance, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
            if ((cimPerLengthImpedance != null) && (rd != null))
            {
                Projekat46Converter.PopulateIdentifiedObjectProperties(cimPerLengthImpedance, rd);
            }
        }

        // PER LENGTH SEQUENCE IMPENDANCE
        public static void PopulatePerLengthSequenceImpedanceProperties(FTN.PerLengthSequenceImpedance cimPerLengthSequenceImpedance, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
            if ((cimPerLengthSequenceImpedance != null) && (rd != null))
            {
                Projekat46Converter.PopulatePerLengthImpedanceProperties(cimPerLengthSequenceImpedance, rd, importHelper, report);

                if (cimPerLengthSequenceImpedance.B0chHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.PERLENGTHSEQUENCEIMPENDANCE_B0CH, cimPerLengthSequenceImpedance.B0ch));
                }
                if (cimPerLengthSequenceImpedance.BchHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.PERLENGTHSEQUENCEIMPENDANCE_BCH, cimPerLengthSequenceImpedance.Bch));
                }
                if (cimPerLengthSequenceImpedance.G0chHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.PERLENGTHSEQUENCEIMPENDANCE_G0CH, cimPerLengthSequenceImpedance.G0ch));
                }
                if (cimPerLengthSequenceImpedance.GchHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.PERLENGTHSEQUENCEIMPENDANCE_GCH, cimPerLengthSequenceImpedance.Gch));
                }
                if (cimPerLengthSequenceImpedance.RHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.PERLENGTHSEQUENCEIMPENDANCE_R, cimPerLengthSequenceImpedance.R));
                }
                if (cimPerLengthSequenceImpedance.R0HasValue)
                {
                    rd.AddProperty(new Property(ModelCode.PERLENGTHSEQUENCEIMPENDANCE_R0, cimPerLengthSequenceImpedance.R0));
                }
                if (cimPerLengthSequenceImpedance.XHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.PERLENGTHSEQUENCEIMPENDANCE_X, cimPerLengthSequenceImpedance.X));
                }
                if (cimPerLengthSequenceImpedance.X0HasValue)
                {
                    rd.AddProperty(new Property(ModelCode.PERLENGTHSEQUENCEIMPENDANCE_X0, cimPerLengthSequenceImpedance.X0));
                }
            }
        }
        #endregion Populate ResourceDescription

        #region Enums convert
        // Ništa za enumeracije
        #endregion Enums convert
    }
}
