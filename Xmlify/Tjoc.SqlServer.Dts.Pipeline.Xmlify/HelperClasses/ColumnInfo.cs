// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ColumnInfo.cs" company="">
//   
// </copyright>
// <summary>
//   The column info.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Tjoc.SqlServer.Dts.Pipeline.Xmlify.HelperClasses
{
    #region using directive

    using Microsoft.SqlServer.Dts.Pipeline.Wrapper;

    #endregion

    /// <summary>
    /// The column info.
    /// </summary>
    internal class ColumnInfo
    {
        #region Public Properties

        /// <summary>
        ///   The buffer column index.
        /// </summary>
        public int BufferColumnIndex { get; set; }

        /// <summary>
        ///   The column disposition.
        /// </summary>
        public DTSRowDisposition ColumnDisposition { get; set; }

        /// <summary>
        ///   The lineage id.
        /// </summary>
        public int LineageId { get; set; }

        /// <summary>
        ///   The name.
        /// </summary>
        public string Name { get; set; }

        #endregion
    }
}