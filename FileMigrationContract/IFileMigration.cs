//-----------------------------------------------------------------------
// <copyright file="IFileMigration.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Thomas Holzgethan (35224@fhwn.ac.at)</author>
//-----------------------------------------------------------------------

namespace Sharezbold.FileMigration.Contract
{
    using System.Collections.Generic;
    using System.ServiceModel;

    /// <summary>
    /// Interface of the webservice of FileMigration.
    /// </summary>
    [ServiceContract]
    public interface IFileMigration
    {
        /// <summary>
        /// Gets the maximal file size.
        /// </summary>
        /// <returns>max file size</returns>
        [OperationContract]
        int GetMaxFileSize();

        /// <summary>
        /// Returns the max file size per extension as dictionary.
        /// </summary>
        /// <returns>max file size per extension</returns>
        [OperationContract]
        IDictionary<string, int> GetMaxFileSizePerExtension();

        /// <summary>
        /// Returns the max message size.
        /// </summary>
        /// <returns>max message size</returns>
        [OperationContract]
        int GetMaxMessageSize();
    }
}
