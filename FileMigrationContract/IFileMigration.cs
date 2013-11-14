
namespace Sharezbold.FileMigration.Contract
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.ServiceModel;

    [ServiceContract]
    public interface IFileMigration
    {
        [OperationContract]
        int GetMaxFileSize();

        [OperationContract]
        IDictionary<string, int> GetMaxFileSizePerExtension();

        [OperationContract]
        int GetMaxMessageSize();
    }
}
