using System.Collections.Generic;
using System.IO;
using System.Web.Script.Serialization;
using csfe.compilation;

namespace csfe.adapters
{
    /*
        Services reside in their own directories within the directory of a flow.
        Within each service directory a file called service.json needs to be present. Its structure:
        
        {
          "name":"check",
          "executable":"mono",
          "arguments": "testforoccurrenceofargs0.exe %argument"
        }
        
        where "name" is optional. If no name is given the name of the directory is assumed as the service name.
    */
    class ServiceCrawler
    {
        public static Dictionary<string, ServiceInfo> Compile_services(string path) {
            var services = new Dictionary<string, ServiceInfo>();
            var json = new JavaScriptSerializer();
            var serviceinfoFilenames = Directory.GetFiles(path, "service.json", SearchOption.AllDirectories);

            foreach (var f in serviceinfoFilenames) {
                var serviceJson = File.ReadAllText(f);
                var serviceDef = json.Deserialize<ServiceDef>(serviceJson);
                
                var serviceInfo = new ServiceInfo {
                    Path = Path.GetDirectoryName(f),
                    Executable = serviceDef.Executable,
                    Arguments = serviceDef.Arguments
                };

                var serviceName = string.IsNullOrEmpty(serviceDef.Name)
                    ? Path.GetFileName(Path.GetDirectoryName(f))
                    : serviceDef.Name;
                
                services.Add(serviceName, serviceInfo);
            }

            return services;
        }
        
        
        private class ServiceDef {
            public string Name { get; set; }
            public string Executable { get; set; }
            public string Arguments { get; set; }
        }
    }
}