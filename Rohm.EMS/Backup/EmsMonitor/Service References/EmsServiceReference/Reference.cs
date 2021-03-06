﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.3082
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EmsMonitor.EmsServiceReference {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="EmsServiceReference.IReporter")]
    public interface IReporter {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IReporter/ReportActivity", ReplyAction="http://tempuri.org/IReporter/ReportActivityResponse")]
        void ReportActivity(int machineID, int activityID, int processID);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IReporter/ReportLotEnd", ReplyAction="http://tempuri.org/IReporter/ReportLotEndResponse")]
        void ReportLotEnd(int machineID, string lotNo, int inputQty, int goodOutput, int ngOutput, int processID);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    public interface IReporterChannel : EmsMonitor.EmsServiceReference.IReporter, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    public partial class ReporterClient : System.ServiceModel.ClientBase<EmsMonitor.EmsServiceReference.IReporter>, EmsMonitor.EmsServiceReference.IReporter {
        
        public ReporterClient() {
        }
        
        public ReporterClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public ReporterClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ReporterClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ReporterClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public void ReportActivity(int machineID, int activityID, int processID) {
            base.Channel.ReportActivity(machineID, activityID, processID);
        }
        
        public void ReportLotEnd(int machineID, string lotNo, int inputQty, int goodOutput, int ngOutput, int processID) {
            base.Channel.ReportLotEnd(machineID, lotNo, inputQty, goodOutput, ngOutput, processID);
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="EmsServiceReference.IMonitor", CallbackContract=typeof(EmsMonitor.EmsServiceReference.IMonitorCallback))]
    public interface IMonitor {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMonitor/Connect", ReplyAction="http://tempuri.org/IMonitor/ConnectResponse")]
        void Connect(int areaID);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    public interface IMonitorCallback {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMonitor/MachineActivityChanged", ReplyAction="http://tempuri.org/IMonitor/MachineActivityChangedResponse")]
        void MachineActivityChanged(int machineID, int activityID, int processID);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMonitor/MachineLotEnd", ReplyAction="http://tempuri.org/IMonitor/MachineLotEndResponse")]
        void MachineLotEnd(int machineID, string lotNo, int inputQty, int goodOutput, int ngOutput, int processID);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMonitor/KeepAlive", ReplyAction="http://tempuri.org/IMonitor/KeepAliveResponse")]
        void KeepAlive();
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    public interface IMonitorChannel : EmsMonitor.EmsServiceReference.IMonitor, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    public partial class MonitorClient : System.ServiceModel.DuplexClientBase<EmsMonitor.EmsServiceReference.IMonitor>, EmsMonitor.EmsServiceReference.IMonitor {
        
        public MonitorClient(System.ServiceModel.InstanceContext callbackInstance) : 
                base(callbackInstance) {
        }
        
        public MonitorClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName) : 
                base(callbackInstance, endpointConfigurationName) {
        }
        
        public MonitorClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, string remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public MonitorClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public MonitorClient(System.ServiceModel.InstanceContext callbackInstance, System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, binding, remoteAddress) {
        }
        
        public void Connect(int areaID) {
            base.Channel.Connect(areaID);
        }
    }
}
