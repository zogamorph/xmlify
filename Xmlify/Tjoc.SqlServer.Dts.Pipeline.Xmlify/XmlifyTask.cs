using System;
using System.Runtime.InteropServices;
using System.Xml;
using System.Text;
using Microsoft.SqlServer.Dts.Pipeline;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using Microsoft.SqlServer.Dts.Runtime.Wrapper;
using System.Diagnostics.CodeAnalysis;

namespace Tjoc.SqlServer.Dts.Pipeline.Xmlify
{
    [SuppressMessage("Microsoft.Interoperability", "CA1405:ComVisibleTypeBaseTypesShouldBeComVisible"), ComVisible(true)]
    [DtsPipelineComponent(
            DisplayName = "Xmlify",
            Description = "Converts multiple columns into a single Xml column",
            IconResource = "Tjoc.SqlServer.Dts.Pipeline.Xmlify.XmlifyTask.ico"
        )]
    public class XmlifyTask : PipelineComponent
    {
        private struct ColumnInfo
        {
            public int BufferColumnIndex;
            public DTSRowDisposition ColumnDisposition;
            public int LineageId;
            public string Name;
        }

        private ColumnInfo[] _inputColumnInfos;
        private ColumnInfo[] _outputColumnInfos;

        private string _namespace = string.Empty;
        private string _rowElementName = "row";
        private string _columnElementName = "col";
        private string _nameAttributeName = "name";
        private string _nullAttributeName = "null";
        private bool _includeColumnName = true;

        public string Namespace
        {
            get { return _namespace; }
            set { _namespace = value; }
        }

        public string RowElementName
        {
            get { return _rowElementName; }
            set { _rowElementName = value; }
        }

        public string ColumnElementName
        {
            get { return _columnElementName; }
            set { _columnElementName = value; }
        }

        public string NameAttributeName
        {
            get { return _nameAttributeName; }
            set { _nameAttributeName = value; }
        }

        public string NullAttributeName
        {
            get { return _nullAttributeName; }
            set { _nullAttributeName = value; }
        }


        public bool IncludeColumnName
        {
            get { return _includeColumnName = true; }
            set { _includeColumnName = value; }
        }

        /// <summary>
        /// Called when the component is initially added to the data flow task. Add the input, output, and error output.
        /// </summary>
        public override void ProvideComponentProperties()
        {
            RemoveAllInputsOutputsAndCustomProperties();

            ComponentMetaData.UsesDispositions = true;

            //	Add the input
            var input = ComponentMetaData.InputCollection.New();
            input.Name = "XmlifyInput";
            input.ErrorRowDisposition = DTSRowDisposition.RD_FailComponent;

            //	Add the output
            var output = ComponentMetaData.OutputCollection.New();
            output.Name = "XmlifyOutput";
            output.SynchronousInputID = input.ID;
            output.ExclusionGroup = 1;

            //	Add the error output 
            AddErrorOutput("XmlifyErrorOutput", input.ID, output.ExclusionGroup);

            AddXmlColumn();
        }

        [CLSCompliant(false)]
        public override DTSValidationStatus Validate()
        {
            ///	If there is an input column that no longer exists in the Virtual input collection,
            /// return needs new meta data. The designer will then call ReinitalizeMetadata which will clean up the input collection.
            if (ComponentMetaData.AreInputColumnsValid == false)
                return DTSValidationStatus.VS_NEEDSNEWMETADATA;

            return base.Validate();
        }


        /// <summary>
        /// Called after the component has returned VS_NEEDSNEWMETADATA from Validate. Removes any input columns that 
        /// no longer exist in the Virtual Input Collection.
        /// </summary>
        public override void ReinitializeMetaData()
        {
            ComponentMetaData.RemoveInvalidInputColumns();
            base.ReinitializeMetaData();
        }

        /// <summary>
        /// Called when a user has selected an Input column for the component. This component only accepts input columns
        /// that have DTSUsageType.UT_READWRITE. Any other usage types are rejected.
        /// </summary>
        /// <param name="inputId">The ID of the input that the column is inserted in.</param>
        /// <param name="virtualInput">The virtual input object containing that contains the new column.</param>
        /// <param name="lineageId">The LineageId of the virtual input column.</param>
        /// <param name="usageType">The DTSUsageType parameter that specifies how the column is used by the component.</param>
        /// <returns>The newly created IDTSInputColumn100.</returns>
        [SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "System.Exception.#ctor(System.String)"), CLSCompliant(false)]
        public override IDTSInputColumn100 SetUsageType(int inputId, IDTSVirtualInput100 virtualInput, int lineageId, DTSUsageType usageType)
        {
            //	Get the column
            var col = base.SetUsageType(inputId, virtualInput, lineageId, usageType);

            return col;
        }

        /// <summary>
        /// Called when an IDTSOutput100 is deleted from the component. Disallow outputs to be deleted by throwing an exception.
        /// </summary>
        /// <param name="outputId">The ID of the output to delete.</param>
        [SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "System.Exception.#ctor(System.String)")]
        public override void DeleteOutput(int outputId)
        {
            throw new Exception("Can't delete output " + outputId.ToString(System.Globalization.CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Called when an IDTSOutput100 is added to the component. Disallow new outputs by throwing an exception.
        /// </summary>
        /// <param name="insertPlacement">The location, relative to the output specified by outputId,to insert the new output.</param>
        /// <param name="outputId">The ID of the output that the new output is located next to.</param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "System.Exception.#ctor(System.String)"), CLSCompliant(false)]
        public override IDTSOutput100 InsertOutput(DTSInsertPlacement insertPlacement, int outputId)
        {
            throw new Exception("Can't add output to the component.");
        }


        /// <summary>
        /// Called prior to ProcessInput, the buffer column index, index of the character to change, and the operation
        /// for each column in the input collection is read, and stored.
        /// </summary>
        public override void PreExecute()
        {
            var input = ComponentMetaData.InputCollection[0];
            _inputColumnInfos = new ColumnInfo[input.InputColumnCollection.Count];

            for (var i = 0; i < input.InputColumnCollection.Count; i++)
            {
                var column = input.InputColumnCollection[i];
                _inputColumnInfos[i] = new ColumnInfo();
                _inputColumnInfos[i].BufferColumnIndex = BufferManager.FindColumnByLineageID(input.Buffer, column.LineageID);
                _inputColumnInfos[i].ColumnDisposition = column.ErrorRowDisposition;
                _inputColumnInfos[i].LineageId = column.LineageID;
                _inputColumnInfos[i].Name = column.Name;
            }

            var output = ComponentMetaData.OutputCollection[0];
            _outputColumnInfos = new ColumnInfo[output.OutputColumnCollection.Count];

            for (var i = 0; i < output.OutputColumnCollection.Count; i++)
            {
                var column = output.OutputColumnCollection[i];
                _outputColumnInfos[i] = new ColumnInfo();
                _outputColumnInfos[i].BufferColumnIndex = BufferManager.FindColumnByLineageID(input.Buffer, column.LineageID);
                _outputColumnInfos[i].ColumnDisposition = column.ErrorRowDisposition;
                _outputColumnInfos[i].LineageId = column.LineageID;
                _outputColumnInfos[i].Name = column.Name;
            }

        }

        private void AddXmlColumn()
        {
            var column = ComponentMetaData.OutputCollection[0].OutputColumnCollection.New();
            column.Name = "Xml";
            column.SetDataTypeProperties(DataType.DT_NTEXT, 0, 0, 0, 0);
        }

        /// <summary>
        /// Called when a PipelineBuffer is passed to the component.
        /// </summary>
        /// <param name="inputId">The ID of the Input that the buffer contains rows for.</param>
        /// <param name="buffer">The PipelineBuffer containing the columns defined in the IDTSInput100.</param>
        [SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "System.Exception.#ctor(System.String)")]
        public override void ProcessInput(int inputId, PipelineBuffer buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            var input = ComponentMetaData.InputCollection.GetObjectByID(inputId);

            var errorOutputId = -1;
            var errorOutputIndex = -1;
            var defaultOutputId = -1;

            GetErrorOutputInfo(ref errorOutputId, ref errorOutputIndex);

            if (errorOutputIndex == 0)
                defaultOutputId = ComponentMetaData.OutputCollection[1].ID;
            else
                defaultOutputId = ComponentMetaData.OutputCollection[0].ID;


            while (buffer.NextRow())
            {
                /// If the columnInfos array has zero dimensions, then 
                /// no input columns have been selected for the component. 
                /// Direct the row to the default output.
                if (_inputColumnInfos.Length == 0)
                    buffer.DirectRow(defaultOutputId);

                var isError = false;

                // TODO - namespace table.

                var sb = new StringBuilder();
                using (var writer = XmlWriter.Create(sb))
                {
                    if (!string.IsNullOrEmpty(_namespace))
                    {
                        writer.WriteAttributeString("xmlns", _namespace);
                    }

                    writer.WriteStartElement(_rowElementName);

                    /// Iterate the columns in the columnInfos array.
                    for (var i = 0; i < _inputColumnInfos.Length; i++)
                    {
                        var colInfo = _inputColumnInfos[i];

                        writer.WriteStartElement(_columnElementName);
                        writer.WriteAttributeString(_nameAttributeName, colInfo.Name);

                        /// Is the column null?
                        if (!buffer.IsNull(colInfo.BufferColumnIndex))
                        {
                            /// No, process it.
                            var columnValue = buffer[colInfo.BufferColumnIndex].ToString();

                            writer.WriteValue(columnValue);
                        }
                        else
                        {
                            writer.WriteAttributeString(_nullAttributeName, "true");
                        }

                        writer.WriteEndElement();
                    }

                    writer.WriteEndElement();
                }

                buffer.SetString(_outputColumnInfos[0].BufferColumnIndex, sb.ToString());

                /// Finished processing each of the columns in this row.
                /// If an error occurred and the error output is configured, then the row has already been directed to the error output, if configured.
                /// If not, then direct the row to the default output.
                if (!isError)
                    buffer.DirectRow(defaultOutputId);
            }
        }
    }
}
