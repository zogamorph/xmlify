// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomProperty.cs" company="">
//   
// </copyright>
// <summary>
//   The custom property.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Tjoc.SqlServer.Dts.Pipeline.Xmlify.HelperClasses
{
    using Microsoft.SqlServer.Dts.Pipeline.Wrapper;

    /// <summary>
    /// The custom property.
    /// </summary>
    internal class CustomProperty
    {
        #region Public Properties

        /// <summary>
        ///   Gets or sets DefaultValue.
        /// </summary>
        public object DefaultValue { get; set; }

        /// <summary>
        ///   Gets or sets Description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///   Gets or sets Name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///   Gets or sets PersistState.
        /// </summary>
        public DTSPersistState PersistState { get; set; }

        /// <summary>
        ///   Gets or sets PropertyExpressionType.
        /// </summary>
        public DTSCustomPropertyExpressionType PropertyExpressionType { get; set; }

        #endregion
    }
}