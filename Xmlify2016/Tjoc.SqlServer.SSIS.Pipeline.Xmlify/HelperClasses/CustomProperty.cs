// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomProperty.cs" company="CodePlex">
//   XMLIfy (c) 2012
// </copyright>
// <summary>
//   The custom property.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Tjoc.SqlServer.SSIS.Pipeline.Xmlify2016.HelperClasses
{
    #region Directives

    using Microsoft.SqlServer.Dts.Pipeline.Wrapper;

    #endregion

    /// <summary>
    ///     The custom property.
    /// </summary>
    internal class CustomProperty
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets DefaultValue.
        /// </summary>
        public object DefaultValue { get; set; }

        /// <summary>
        ///     Gets or sets Description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     Gets or sets Name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets PersistState.
        /// </summary>
        public DTSPersistState PersistState { get; set; }

        /// <summary>
        ///     Gets or sets PropertyExpressionType.
        /// </summary>
        public DTSCustomPropertyExpressionType PropertyExpressionType { get; set; }

        #endregion
    }
}