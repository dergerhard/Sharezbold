﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Dieser Code wurde von einem Tool generiert.
//     Laufzeitversion:4.0.30319.34003
//
//     Änderungen an dieser Datei können falsches Verhalten verursachen und gehen verloren, wenn
//     der Code erneut generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Sharezbold.FileMigration.FileMigration {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="FileMigration.IFileMigration")]
    public interface IFileMigration {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IFileMigration/GetMaxFileSizePerExtension", ReplyAction="http://tempuri.org/IFileMigration/GetMaxFileSizePerExtensionResponse")]
        System.Collections.Generic.Dictionary<string, int> GetMaxFileSizePerExtension();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IFileMigration/GetMaxFileSizePerExtension", ReplyAction="http://tempuri.org/IFileMigration/GetMaxFileSizePerExtensionResponse")]
        System.Threading.Tasks.Task<System.Collections.Generic.Dictionary<string, int>> GetMaxFileSizePerExtensionAsync();
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IFileMigrationChannel : Sharezbold.FileMigration.FileMigration.IFileMigration, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class FileMigrationClient : System.ServiceModel.ClientBase<Sharezbold.FileMigration.FileMigration.IFileMigration>, Sharezbold.FileMigration.FileMigration.IFileMigration {
        
        public FileMigrationClient() {
        }
        
        public FileMigrationClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public FileMigrationClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public FileMigrationClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public FileMigrationClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public System.Collections.Generic.Dictionary<string, int> GetMaxFileSizePerExtension() {
            return base.Channel.GetMaxFileSizePerExtension();
        }
        
        public System.Threading.Tasks.Task<System.Collections.Generic.Dictionary<string, int>> GetMaxFileSizePerExtensionAsync() {
            return base.Channel.GetMaxFileSizePerExtensionAsync();
        }
    }
}