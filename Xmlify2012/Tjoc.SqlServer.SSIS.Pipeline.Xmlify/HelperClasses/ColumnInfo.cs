// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ColumnInfo.cs" company="CodePlex">
//   XMLIfy (c) 2012
// </copyright>
// <summary>
//   The column info.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Tjoc.SqlServer.SSIS.Pipeline.Xmlify2012.HelperClasses
{
    #region Directives

    using Microsoft.SqlServer.Dts.Pipeline.Wrapper;

    #endregion

    /// <summary>
    ///     The column info.
    /// </summary>
    internal class ColumnInfo
    {
        #region Public Properties

        /// <summary>
        ///     The buffer column index.
        /// </summary>
        public int BufferColumnIndex { get; set; }

        /// <summary>
        ///     The column disposition.
        /// </summary>
        public DTSRowDisposition ColumnDisposition { get; set; }

        /// <summary>
        ///     The lineage id.
        /// </summary>
        public int LineageId { get; set; }

        /// <summary>
        ///     The name.
        /// </summary>
        public string Name { get; set; }

        #endregion
    }
}