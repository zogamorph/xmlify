// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XMLIfyCustomPropites.cs" company="">
//   
// </copyright>
// <summary>
//   The xml ify custom propites.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Tjoc.SqlServer.Dts.Pipeline.Xmlify.HelperClasses
{
    #region Using Directives

    using System.Collections.Generic;

    using Microsoft.SqlServer.Dts.Pipeline.Wrapper;

    #endregion

    /// <summary>
    /// The xml ify custom propites.
    /// </summary>
    internal static class XMLIfyCustomPropites
    {
        #region Constants and Fields

        /// <summary>
        ///   The columnelementname.
        /// </summary>
        public const string COLUMNELEMENTNAME = "ColumnElementName";

        /// <summary>
        ///   The converttoxml.
        /// </summary>
        public const string CONVERTTOXML = "ConvertToXml";

        /// <summary>
        ///   The Element Format
        /// </summary>
        public const string ELEMENTFORMAT = "ElementFormat";

        /// <summary>
        ///   The includecolumnname.
        /// </summary>
        public const string INCLUDECOLUMNNAME = "IncludeColumnName";

        /// <summary>
        /// The includexmltag.
        /// </summary>
        public const string INCLUDEXMLTAG = "IncludeXMLTag";

        /// <summary>
        ///   The nameattributename.
        /// </summary>
        public const string NAMEATTRIBUTENAME = "NameAttributeName";

        /// <summary>
        ///   The null attribute name property key name.
        /// </summary>
        public const string NULLATTRIBUTENAME = "NullAttributeName";

        /// <summary>
        ///   The rowelementname.
        /// </summary>
        public const string ROWELEMENTNAME = "RowElementName";

        /// <summary>
        ///   The xmlnamespace.
        /// </summary>
        public const string XMLNAMESPACE = "XMLNamespace";

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Creates the custom property.
        /// </summary>
        /// <param name="property">
        /// The property. 
        /// </param>
        /// <param name="name">
        /// The name. 
        /// </param>
        /// <param name="defaultValue">
        /// The default Value. 
        /// </param>
        /// <param name="description">
        /// The description. 
        /// </param>
        /// <param name="propertyExpressionType">
        /// Type of the property expression. 
        /// </param>
        /// <param name="persistState">
        /// State of the persist. 
        /// </param>
        public static void CreateCustomProperty(
            IDTSCustomProperty100 property,
            string name,
            object defaultValue,
            string description,
            DTSCustomPropertyExpressionType propertyExpressionType,
            DTSPersistState persistState)
        {
            property.Name = name;
            property.Value = defaultValue;
            property.Description = description;
            property.ExpressionType = propertyExpressionType;
            property.State = persistState;
        }

        /// <summary>
        /// The create custom property list.
        /// </summary>
        public static Dictionary<string, CustomProperty> CreateCustomPropertyList()
        {
            Dictionary<string, CustomProperty> customPropertiesList = new Dictionary<string, CustomProperty>();

            // Creating the Column Element Name property
            customPropertiesList.Add(
                COLUMNELEMENTNAME,
                new CustomProperty
                    {
                        Name = "Column Element Name",
                        DefaultValue = "col",
                        Description = "Set the Name of column Element",
                        PropertyExpressionType = DTSCustomPropertyExpressionType.CPET_NOTIFY,
                        PersistState = DTSPersistState.PS_DEFAULT
                    });

            // Creating the Row Element Name property
            customPropertiesList.Add(
                ROWELEMENTNAME,
                new CustomProperty
                    {
                        Name = "Row Element Name",
                        DefaultValue = "row",
                        Description = "Set the Name of row Element",
                        PropertyExpressionType = DTSCustomPropertyExpressionType.CPET_NOTIFY,
                        PersistState = DTSPersistState.PS_DEFAULT
                    });

            // Creating the Null Attribute Name property
            customPropertiesList.Add(
                NULLATTRIBUTENAME,
                new CustomProperty
                    {
                        Name = "Null Attribute Name",
                        DefaultValue = "null",
                        Description = "Set the Name of column Element",
                        PropertyExpressionType = DTSCustomPropertyExpressionType.CPET_NOTIFY,
                        PersistState = DTSPersistState.PS_DEFAULT
                    });

            // Creating the Name Attribute Name property
            customPropertiesList.Add(
                NAMEATTRIBUTENAME,
                new CustomProperty
                    {
                        Name = "Name Attribute Name",
                        DefaultValue = "name",
                        Description = "Set the Name of Attribute",
                        PropertyExpressionType = DTSCustomPropertyExpressionType.CPET_NOTIFY,
                        PersistState = DTSPersistState.PS_DEFAULT
                    });

            // Creating the XML Namespace property
            customPropertiesList.Add(
                XMLNAMESPACE,
                new CustomProperty
                    {
                        Name = "XML Namespace",
                        DefaultValue = string.Empty,
                        Description = "Set the XML namespace",
                        PropertyExpressionType = DTSCustomPropertyExpressionType.CPET_NOTIFY,
                        PersistState = DTSPersistState.PS_DEFAULT
                    });

            // Creating the Include Column Name property
            customPropertiesList.Add(
                INCLUDECOLUMNNAME,
                new CustomProperty
                    {
                        Name = "Include Column Name",
                        DefaultValue = true,
                        Description = "Set the flag to Include the column name as attribute",
                        PropertyExpressionType = DTSCustomPropertyExpressionType.CPET_NOTIFY,
                        PersistState = DTSPersistState.PS_DEFAULT
                    });

            // Creating the Include Column Name property
            customPropertiesList.Add(
                ELEMENTFORMAT,
                new CustomProperty
                    {
                        Name = "Use Element Format",
                        DefaultValue = true,
                        Description = "Set the flag to switch between Element / Attribute Formatting",
                        PropertyExpressionType = DTSCustomPropertyExpressionType.CPET_NOTIFY,
                        PersistState = DTSPersistState.PS_DEFAULT
                    });

            customPropertiesList.Add(
                INCLUDEXMLTAG,
                new CustomProperty
                    {
                        Name = "Include XML Declaration",
                        DefaultValue = true,
                        Description = "Set the flag to include XML Declaration",
                        PropertyExpressionType = DTSCustomPropertyExpressionType.CPET_NOTIFY,
                        PersistState = DTSPersistState.PS_DEFAULT
                    });

            return customPropertiesList;
        }

        #endregion
    }
}