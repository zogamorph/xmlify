using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using Microsoft.SqlServer.Dts.Pipeline;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using Microsoft.SqlServer.Dts.Runtime.Wrapper;
using Tjoc.SqlServer.Dts.Pipeline.Xmlify.HelperClasses;

namespace Tjoc.SqlServer.Dts.Pipeline.Xmlify
{
    #region using directive

    

    #endregion

    /// <summary>
    ///   The xmlify task.
    /// </summary>
    [SuppressMessage("Microsoft.Interoperability", "CA1405:ComVisibleTypeBaseTypesShouldBeComVisible")]
    [ComVisible(true)]
    [DtsPipelineComponent(DisplayName = "Xmlify", Description = "Converts multiple columns into a single Xml column",
        IconResource = "Tjoc.SqlServer.SSIS.Pipeline.Xmlify.XmlifyTask.ico")]
    public class XmlifyTask : PipelineComponent
    {
        #region Constants and Fields

        /// <summary>
        ///   The cancel event.
        /// </summary>
        private bool cancelEvent;

        /// <summary>
        ///   Gets or sets ColumnElementName.
        /// </summary>
        public string columnElementName;

        /// <summary>
        ///   The custom properties list.
        /// </summary>
        private Dictionary<string, CustomProperty> customPropertiesList;

        private bool elementFormat;

        /// <summary>
        ///   Gets or sets a value indicating whether IncludeColumnName.
        /// </summary>
        public bool includeColumnName;

        /// <summary>
        ///   The _input column infos.
        /// </summary>
        private ColumnInfo[] inputColumnInfos;

        /// <summary>
        ///   Gets or sets NameAttributeName.
        /// </summary>
        public string nameAttributeName;

        /// <summary>
        ///   Gets or sets NullAttributeName.
        /// </summary>
        private string nullAttributeName;

        /// <summary>
        ///   The _output column infos.
        /// </summary>
        private ColumnInfo[] outputColumnInfos;

        /// <summary>
        ///   Gets or sets RowElementName.
        /// </summary>
        private string rowElementName;

        /// <summary>
        ///   Gets or sets Namespace.
        /// </summary>
        private string xmlNamespace;

        #endregion

        #region Public Methods

        /// <summary>
        ///   Called when an IDTSOutput100 is deleted from the component. Disallow outputs to be deleted by throwing an exception.
        /// </summary>
        /// <param name="outputId"> The ID of the output to delete. </param>
        [SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes")]
        [SuppressMessage("Microsoft.Globalization",
            "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "System.Exception.#ctor(System.String)")]
        public override void DeleteOutput(int outputId)
        {
            throw new Exception(
                "Can't delete output " + outputId.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        ///   The initialize.
        /// </summary>
        public override void Initialize()
        {
            customPropertiesList = XMLIfyCustomPropites.CreateCustomPropertyList();
            base.Initialize();
        }

        /// <summary>
        ///   Called when an IDTSOutput100 is added to the component. Disallow new outputs by throwing an exception.
        /// </summary>
        /// <param name="insertPlacement"> The location, relative to the output specified by outputId,to insert the new output. </param>
        /// <param name="outputId"> The ID of the output that the new output is located next to. </param>
        /// <returns> </returns>
        [SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes")]
        [SuppressMessage("Microsoft.Globalization",
            "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "System.Exception.#ctor(System.String)")]
        [CLSCompliant(false)]
        public override IDTSOutput100 InsertOutput(DTSInsertPlacement insertPlacement, int outputId)
        {
            throw new Exception("Can't add output to the component.");
        }

        /// <summary>
        ///   Called prior to ProcessInput, the buffer column index, index of the character to change, and the operation for each column in the input collection is read, and stored.
        /// </summary>
        public override void PreExecute()
        {
            xmlNamespace =
                ComponentMetaData.CustomPropertyCollection[
                    customPropertiesList[XMLIfyCustomPropites.XMLNAMESPACE].Name].Value as string;
            columnElementName =
                ComponentMetaData.CustomPropertyCollection[
                    customPropertiesList[XMLIfyCustomPropites.COLUMNELEMENTNAME].Name].Value as string;
            nameAttributeName =
                ComponentMetaData.CustomPropertyCollection[
                    customPropertiesList[XMLIfyCustomPropites.NAMEATTRIBUTENAME].Name].Value as string;
            nullAttributeName =
                ComponentMetaData.CustomPropertyCollection[
                    customPropertiesList[XMLIfyCustomPropites.NULLATTRIBUTENAME].Name].Value as string;
            rowElementName =
                ComponentMetaData.CustomPropertyCollection[
                    customPropertiesList[XMLIfyCustomPropites.ROWELEMENTNAME].Name].Value as string;
            includeColumnName =
                (bool)
                ComponentMetaData.CustomPropertyCollection[
                    customPropertiesList[XMLIfyCustomPropites.INCLUDECOLUMNNAME].Name].Value;

            elementFormat =
                (bool)
                ComponentMetaData.CustomPropertyCollection[customPropertiesList[XMLIfyCustomPropites.ELEMENTFORMAT].Name
                    ].Value;

            IDTSInput100 input = ComponentMetaData.InputCollection[0];
            inputColumnInfos = new ColumnInfo[input.InputColumnCollection.Count];

            for (int i = 0; i < input.InputColumnCollection.Count; i++)
            {
                IDTSInputColumn100 column = input.InputColumnCollection[i];
                inputColumnInfos[i] = new ColumnInfo
                                          {
                                              BufferColumnIndex =
                                                  BufferManager.FindColumnByLineageID(input.Buffer, column.LineageID),
                                              ColumnDisposition = column.ErrorRowDisposition,
                                              LineageId = column.LineageID,
                                              Name = column.Name
                                          };
            }

            IDTSOutput100 output = ComponentMetaData.OutputCollection[0];
            outputColumnInfos = new ColumnInfo[output.OutputColumnCollection.Count];

            for (int i = 0; i < output.OutputColumnCollection.Count; i++)
            {
                IDTSOutputColumn100 column = output.OutputColumnCollection[i];
                outputColumnInfos[i] = new ColumnInfo
                                           {
                                               BufferColumnIndex =
                                                   BufferManager.FindColumnByLineageID(input.Buffer, column.LineageID),
                                               ColumnDisposition = column.ErrorRowDisposition,
                                               LineageId = column.LineageID,
                                               Name = column.Name
                                           };
            }
        }

        /// <summary>
        ///   Called when a PipelineBuffer is passed to the component.
        /// </summary>
        /// <param name="inputId"> The ID of the Input that the buffer contains rows for. </param>
        /// <param name="buffer"> The PipelineBuffer containing the columns defined in the IDTSInput100. </param>
        [SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes")]
        [SuppressMessage("Microsoft.Globalization",
            "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "System.Exception.#ctor(System.String)")]
        public override void ProcessInput(int inputId, PipelineBuffer buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            IDTSInput100 input = ComponentMetaData.InputCollection.GetObjectByID(inputId);

            int errorOutputId = -1;
            int errorOutputIndex = -1;
            int defaultOutputId = -1;

            GetErrorOutputInfo(ref errorOutputId, ref errorOutputIndex);

            if (errorOutputIndex == 0)
            {
                defaultOutputId = ComponentMetaData.OutputCollection[1].ID;
            }
            else
            {
                defaultOutputId = ComponentMetaData.OutputCollection[0].ID;
            }

            while (buffer.NextRow())
            {
                // If the columnInfos array has zero dimensions, then 
                // no input columns have been selected for the component. 
                // Direct the row to the default output.
                if (inputColumnInfos.Length == 0)
                {
                    buffer.DirectRow(defaultOutputId);
                }

                bool isError = false;

                // TODO - namespace table.
                StringBuilder sb = new StringBuilder();
                using (XmlWriter writer = XmlWriter.Create(sb))
                {
                    if (!string.IsNullOrEmpty(xmlNamespace))
                    {
                        writer.WriteAttributeString("xmlns", xmlNamespace);
                    }

                    writer.WriteStartElement(rowElementName);

                    if (elementFormat)
                    {
                        formatElementOutput(buffer, writer);
                    }
                    else
                    {
                        formatAttributeOutput(buffer, writer);
                    }

                    writer.WriteEndElement();
                }

                buffer.SetString(outputColumnInfos[0].BufferColumnIndex, sb.ToString());

                // Finished processing each of the columns in this row.
                // If an error occurred and the error output is configured, then the row has already been directed to the error output, if configured.
                // If not, then direct the row to the default output.
                if (!isError)
                {
                    buffer.DirectRow(defaultOutputId);
                }
            }
        }

        private void formatAttributeOutput(PipelineBuffer buffer, XmlWriter writer)
        {
            ColumnInfo colInfo;
            for (int i = 0; i < inputColumnInfos.Length; i++)
            {
                colInfo = inputColumnInfos[i];
                if (!buffer.IsNull(colInfo.BufferColumnIndex))
                {
                    writer.WriteAttributeString(colInfo.Name, buffer[colInfo.BufferColumnIndex].ToString());
                }
            }
        }

        /// <summary>
        ///   Formats the element output.
        /// </summary>
        /// <param name="buffer"> The buffer. </param>
        /// <param name="writer"> The writer. </param>
        private void formatElementOutput(PipelineBuffer buffer, XmlWriter writer)
        {
            // Iterate the columns in the columnInfos array.
            for (int i = 0; i < inputColumnInfos.Length; i++)
            {
                ColumnInfo colInfo = inputColumnInfos[i];

                writer.WriteStartElement(columnElementName);
                if (includeColumnName)
                {
                    writer.WriteAttributeString(nameAttributeName, colInfo.Name);
                }

                // Is the column null?
                if (!buffer.IsNull(colInfo.BufferColumnIndex))
                {
                    // No, process it.
                    string columnValue = buffer[colInfo.BufferColumnIndex].ToString();
                    writer.WriteValue(columnValue);
                }
                else
                {
                    writer.WriteAttributeString(nullAttributeName, "true");
                }

                writer.WriteEndElement();
            }
        }

        /// <summary>
        ///   Called when the component is initially added to the data flow task. Add the input, output, and error output.
        /// </summary>
        public override void ProvideComponentProperties()
        {
            RemoveAllInputsOutputsAndCustomProperties();

            ComponentMetaData.UsesDispositions = true;

            // 	Add the input
            IDTSInput100 input = ComponentMetaData.InputCollection.New();
            input.Name = "XmlifyInput";
            input.ErrorRowDisposition = DTSRowDisposition.RD_FailComponent;

            // 	Add the output
            IDTSOutput100 output = ComponentMetaData.OutputCollection.New();
            output.Name = "XmlifyOutput";
            output.HasSideEffects = false;
            output.SynchronousInputID = input.ID;
            output.ExclusionGroup = 1;
            AddXmlColumn(output);

            // 	Add the error output 
            AddErrorOutput("XmlifyErrorOutput", input.ID, output.ExclusionGroup);

            foreach (CustomProperty currnetCustomProperty in
                customPropertiesList.Select(customProperty => customProperty.Value))
            {
                XMLIfyCustomPropites.CreateCustomProperty(
                    ComponentMetaData.CustomPropertyCollection.New(),
                    currnetCustomProperty.Name,
                    currnetCustomProperty.DefaultValue,
                    currnetCustomProperty.Description,
                    currnetCustomProperty.PropertyExpressionType,
                    currnetCustomProperty.PersistState);
            }
        }

        /// <summary>
        ///   Called after the component has returned VS_NEEDSNEWMETADATA from Validate. Removes any input columns that no longer exist in the Virtual Input Collection.
        /// </summary>
        public override void ReinitializeMetaData()
        {
            ComponentMetaData.RemoveInvalidInputColumns();
            base.ReinitializeMetaData();
        }

        /// <summary>
        ///   The validate.
        /// </summary>
        /// <returns> </returns>
        [CLSCompliant(false)]
        public override DTSValidationStatus Validate()
        {
            // Checking the there is the correct number of inputs
            if (ComponentMetaData.InputCollection.Count != 1)
            {
                InternalFireError("There should be only one Input. The metadata of this component is corrupt");
                return DTSValidationStatus.VS_ISCORRUPT;
            }

            // Checking the there is the correct number of outputs
            if (ComponentMetaData.OutputCollection.Count != 2)
            {
                InternalFireError("There should be only one output. The metadata of this component is corrupt");
                return DTSValidationStatus.VS_ISCORRUPT;
            }

            // Checking the transform custom prooperty
            if (ComponentMetaData.CustomPropertyCollection.Count > 0)
            {
                foreach (DTSValidationStatus validationStatus in
                    customPropertiesList.Keys.Where(
                        customPropertyKey =>
                        customPropertyKey != XMLIfyCustomPropites.XMLNAMESPACE
                        && customPropertyKey != XMLIfyCustomPropites.INCLUDECOLUMNNAME
                        && customPropertyKey != XMLIfyCustomPropites.ELEMENTFORMAT).Select(
                            customPropertyKey => CheckCustomPropertry(customPropertyKey)).Where(
                                validationStatus => validationStatus != DTSValidationStatus.VS_ISVALID))
                {
                    return validationStatus;
                }

                // Checking the XML Namespace is still there
                try
                {
                    IDTSCustomProperty100 customProperty =
                        ComponentMetaData.CustomPropertyCollection[
                            customPropertiesList[XMLIfyCustomPropites.XMLNAMESPACE].Name];
                    if (customProperty == null)
                    {
                        InternalFireError(
                            string.Format(
                                "The {0} property has been removed from the component.",
                                customPropertiesList[XMLIfyCustomPropites.XMLNAMESPACE].Name));
                        return DTSValidationStatus.VS_NEEDSNEWMETADATA;
                    }
                }
                catch
                {
                    // The property doesn't exist.
                    InternalFireError(
                        string.Format(
                            "The {0} property has been removed from the component.",
                            customPropertiesList[XMLIfyCustomPropites.XMLNAMESPACE].Name));
                    return DTSValidationStatus.VS_NEEDSNEWMETADATA;
                }

                // Checking the Include column is still there
                try
                {
                    IDTSCustomProperty100 customProperty =
                        ComponentMetaData.CustomPropertyCollection[
                            customPropertiesList[XMLIfyCustomPropites.INCLUDECOLUMNNAME].Name];
                    if (customProperty == null)
                    {
                        InternalFireError(
                            string.Format(
                                "The {0} property has been removed from the component.",
                                customPropertiesList[XMLIfyCustomPropites.INCLUDECOLUMNNAME].Name));
                        return DTSValidationStatus.VS_NEEDSNEWMETADATA;
                    }
                }
                catch
                {
                    // The property doesn't exist.
                    InternalFireError(
                        string.Format(
                            "The {0} property has been removed from the component.",
                            customPropertiesList[XMLIfyCustomPropites.INCLUDECOLUMNNAME].Name));
                    return DTSValidationStatus.VS_NEEDSNEWMETADATA;
                }

                // Checking the Element formatting column is still there
                try
                {
                    IDTSCustomProperty100 customProperty =
                        ComponentMetaData.CustomPropertyCollection[
                            customPropertiesList[XMLIfyCustomPropites.ELEMENTFORMAT].Name];
                    if (customProperty == null)
                    {
                        InternalFireError(
                            string.Format(
                                "The {0} property has been removed from the component.",
                                customPropertiesList[XMLIfyCustomPropites.ELEMENTFORMAT].Name));
                        return DTSValidationStatus.VS_NEEDSNEWMETADATA;
                    }
                }
                catch
                {
                    // The property doesn't exist.
                    InternalFireError(
                        string.Format(
                            "The {0} property has been removed from the component.",
                            customPropertiesList[XMLIfyCustomPropites.INCLUDECOLUMNNAME].Name));
                    return DTSValidationStatus.VS_NEEDSNEWMETADATA;
                }
            }
            else
            {
                InternalFireError("The Transform properties have been removed");
                return DTSValidationStatus.VS_NEEDSNEWMETADATA;
            }

            // If there is an input column that no longer exists in the Virtual input collection,
            // return needs new meta data. The designer will then call ReinitalizeMetadata which will clean up the input collection.
            return ComponentMetaData.AreInputColumnsValid == false
                       ? DTSValidationStatus.VS_NEEDSNEWMETADATA
                       : base.Validate();
        }

        #endregion

        #region Methods

        /// <summary>
        ///   The add xml column.
        /// </summary>
        /// <param name="output"> The output. </param>
        private void AddXmlColumn(IDTSOutput100 output)
        {
            IDTSOutputColumn100 column = output.OutputColumnCollection.New();
            column.Name = "Xml";
            column.SetDataTypeProperties(DataType.DT_NTEXT, 0, 0, 0, 0);
        }

        /// <summary>
        ///   The check custom propertry.
        /// </summary>
        /// <param name="customPropertyKeyName"> The custom Property Key Name. </param>
        /// <returns> </returns>
        /// <exception cref="NotImplementedException"></exception>
        private DTSValidationStatus CheckCustomPropertry(string customPropertyKeyName)
        {
            try
            {
                IDTSCustomProperty100 customProperty =
                    ComponentMetaData.CustomPropertyCollection[
                        customPropertiesList[customPropertyKeyName].Name];
                if (customProperty == null)
                {
                    InternalFireError(
                        string.Format(
                            "The {0} property has been removed from the component.",
                            customPropertiesList[customPropertyKeyName].Name));
                    return DTSValidationStatus.VS_NEEDSNEWMETADATA;
                }

                if (string.Compare((string) customProperty.Value, string.Empty) == 0)
                {
                    InternalFireError(string.Format("The {0} property is set incorrectly.", customProperty.Name));
                    return DTSValidationStatus.VS_NEEDSNEWMETADATA;
                }

                return DTSValidationStatus.VS_ISVALID;
            }
            catch
            {
                // The property doesn't exist.
                InternalFireError(
                    string.Format(
                        "The {0} property has been removed from the component.",
                        customPropertiesList[customPropertyKeyName].Name));
                return DTSValidationStatus.VS_NEEDSNEWMETADATA;
            }
        }

        /// <summary>
        ///   The internal fire error.
        /// </summary>
        /// <param name="message"> The message. </param>
        private void InternalFireError(string message)
        {
            ComponentMetaData.FireError(
                0, ComponentMetaData.Name, message, string.Empty, 0, out cancelEvent);
        }

        #endregion
    }
}