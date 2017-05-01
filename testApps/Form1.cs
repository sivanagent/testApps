using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using System.Text;
using System.Web.Services.Description;
using System.Xml;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Runtime.Serialization;



namespace testApps
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        //download  remote file FROM remote ACIAF server
        private void button1_Click(object sender, EventArgs e)
        {
            /**
            //Serializer ss = new Serializer( "d:\\temp\\", "SerializerTest.xml");
            //ss.testSerialize();

            aciafServiceReference.ACIAFClient clntAciaf = new aciafServiceReference.ACIAFClient();
            //staAciaf.
            aciafServiceReference.RemoteACIAFAgentInstanceArchiveFileInfo fileInfo = new aciafServiceReference.RemoteACIAFAgentInstanceArchiveFileInfo();
            fileInfo.FileName = @"d:\abc.txt";
            var FileByteStream = new Object();

            System.IO.Stream fileStream; 

            clntAciaf.SendAgentInstanceArchiveToRemoteACIAFServer(ref fileInfo.FileName, out fileStream);

            byte[] buffer;
           
            try
            {
                //int length = (int)fileStream.Length;  // get file length
                buffer = new byte[11500];            // create buffer
                int count;                            // actual number of bytes read
                int sum = 0;                          // total number of bytes read
                int i = 0;

                // read until Read method returns 0 (end of the stream has been reached)
                while ((count = fileStream.Read(buffer, sum, 500)) > 0)
                {
                    Console.WriteLine("hello............{0}.", i);
                    
                    Console.WriteLine(System.Text.Encoding.UTF8.GetString(buffer));
                    sum += count;  // sum is a buffer offset for next reading
                    if (fileStream.CanRead != true)
                    {
                        break;
                    }
                }
            }
            finally
            {
                fileStream.Close();
            }

            ***/           
            try
            {
            //http://localhost/serviceACIAF/ACIAF.svc?wsdl
            //serviceACIAF.ACIAF serviceAciaf = new ACIAFClient();

            Uri mexAddress = new Uri(@"http://localhost/serviceACIAF/ACIAF.svc?wsdl");
            MetadataExchangeClientMode mexMode = MetadataExchangeClientMode.HttpGet;
            string contractName = "IACIAF";
            string operationNameSendAgent = "SendAgentInstanceArchiveToRemoteACIAFServer";
            string operationNameReceiveAgent = "ReceiveAgentInstanceArchiveFROMRemoteACIAFServer";

            serviceACIAF.DownloadArchivedAgentRequest dwnld_AgentFile_Request = new serviceACIAF.DownloadArchivedAgentRequest();
            dwnld_AgentFile_Request.FileName = @"d:\abc.txt" ;
            

            MetadataExchangeClient mexClient = new MetadataExchangeClient(mexAddress, mexMode);
            mexClient.ResolveMetadataReferences = true;
            MetadataSet metaSet = mexClient.GetMetadata();

            WsdlImporter importer = new WsdlImporter(metaSet);
                
            //BEGIN ............
            XsdDataContractImporter xsd = new XsdDataContractImporter();
            xsd.Options = new ImportOptions();
            xsd.Options.ImportXmlType = true;
            xsd.Options.GenerateSerializable = true;
            //xsd.Options.ReferencedTypes.Add(typeof(KeyValuePair<string, string>));
            //xsd.Options.ReferencedTypes.Add(typeof(System.Collections.Generic.List<KeyValuePair<string, string>>));
            //xsd.Options.ReferencedTypes.Add(typeof(System.Collections.Generic.List<Addition.MathData>));

            xsd.Options.ReferencedTypes.Add(typeof(serviceACIAF.DownloadArchivedAgentRequest));
            xsd.Options.ReferencedTypes.Add(typeof(serviceACIAF.RemoteACIAFAgentInstanceArchiveFileInfo));
            xsd.Options.ReferencedTypes.Add(typeof(System.IO.Stream));

                importer.State.Add(typeof(XsdDataContractImporter), xsd);
            //END ..............


            Collection<ContractDescription> contracts = importer.ImportAllContracts();
            ServiceEndpointCollection allEndpoints = importer.ImportAllEndpoints();

            ServiceContractGenerator generator = new ServiceContractGenerator();
            var endpointsForContracts = new Dictionary<string, IEnumerable<ServiceEndpoint>>();

            foreach (ContractDescription contract in contracts)
            {
                generator.GenerateServiceContractType(contract);
                endpointsForContracts[contract.Name] = allEndpoints.Where(
                    se => se.Contract.Name == contract.Name).ToList();
            }

            if (generator.Errors.Count != 0)
                throw new Exception("There were errors during code compilation.");

            CodeGeneratorOptions options = new CodeGeneratorOptions();
            options.BracingStyle = "C";
            CodeDomProvider codeDomProvider = CodeDomProvider.CreateProvider("C#");

            CompilerParameters compilerParameters = new CompilerParameters(
                new string[] {
            "System.dll", "System.ServiceModel.dll",
            "System.Runtime.Serialization.dll" });
            compilerParameters.GenerateInMemory = true;

            CompilerResults results = codeDomProvider.CompileAssemblyFromDom(
                compilerParameters, generator.TargetCompileUnit);

            if (results.Errors.Count > 0)
            {
                throw new Exception("There were errors during generated code compilation");
            }
            else
            {
                    Type clientProxyType = results.CompiledAssembly.GetTypes().First(
                        t => t.IsClass &&
                            t.GetInterface(contractName) != null &&
                            t.GetInterface(typeof(ICommunicationObject).Name) != null);

                    ServiceEndpoint se = endpointsForContracts[contractName].First();

                    object instance = results.CompiledAssembly.CreateInstance(
                        clientProxyType.Name,
                        false,
                        System.Reflection.BindingFlags.CreateInstance,
                        null,
                        new object[] { se.Binding, se.Address },
                        CultureInfo.CurrentCulture, null);

                    var methodInfoReceiveAgent  = instance.GetType().GetMethod(operationNameReceiveAgent);
                    var methodInfoSendAgent     = instance.GetType().GetMethod(operationNameSendAgent);

                    serviceACIAF.RemoteACIAFAgentInstanceArchiveFileInfo dwnld_agnt_fileInfo = new serviceACIAF.RemoteACIAFAgentInstanceArchiveFileInfo();
                    //dwnld_agnt_fileInfo.FileName
                    //dwnld_agnt_fileInfo.Length
                    //dwnld_agnt_fileInfo.FileByteStream
                    //dwnld_agnt_fileInfo.
                    System.IO.Stream fileStream1;
                    object[] arguments = new object[2];

                    //this expects two arguments ..FIrst "filename" to download from server, next reference to OUT parameter FILESTREAM 
                    //from aciaf server that this client can use to read the stream and download..hahaha 
                    arguments[0] = @"d:\\abc.txt";
                    var retval  = methodInfoSendAgent.Invoke(instance, BindingFlags.InvokeMethod, null, arguments, null);
                    fileStream1 = (System.IO.Stream) arguments[1];

                    try
                    {
                        //int length = (int)fileStream.Length;  // get file length
                        byte [] buffer = new byte[11500];            // create buffer
                        int count;                            // actual number of bytes read
                        int sum = 0;                          // total number of bytes read
                        int i = 0;

                        // read until Read method returns 0 (end of the stream has been reached)
                        while ((count = fileStream1.Read(buffer, sum, 500)) > 0)
                        {
                            Console.WriteLine("hello............{0}.", i);

                            Console.WriteLine(System.Text.Encoding.UTF8.GetString(buffer));
                            sum += count;  // sum is a buffer offset for next reading
                            if (fileStream1.CanRead != true)
                            {
                                break;
                            }
                        }
                    }
                    finally
                    {
                        fileStream1.Close();
                    }



                    //object retVal1 = methodInfo1.Invoke(instance, BindingFlags.InvokeMethod, null, operationParameters????, null);
                    Console.WriteLine(retval.ToString());
                }
            }
            catch (Exception ex)
            {
                //error in dynamic webservice invocation...
                Console.WriteLine(ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //upload local file to remote ACIAF server

            aciafServiceReference.RemoteACIAFAgentInstanceArchiveFileInfo aa = new aciafServiceReference.RemoteACIAFAgentInstanceArchiveFileInfo();
            aa.FileByteStream   = ;
            aa.FileName         = ;
            aa.Length = ;
        }
    }
}
