using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace wipm.exchangestats.data.ingress.core {

    /// <summary>
    /// Serialise a Service response
    /// </summary>
    public class ServiceResponseJsonConverter {

        public static string Serialise
                               ( ServiceResponse serviceResponse ) {

            if ( serviceResponse == null ) throw new ArgumentNullException( nameof( serviceResponse ) );


            var binder 
                  = new ServiceResponseTypeNameSerializationBinder("YourAppNamespace.{0}, YourAppAssembly");
 
            var serializationSettings 
                  = new JsonSerializerSettings {
                       TypeNameHandling = TypeNameHandling.Auto
                      ,Binder = binder
                    };
                        
            var json 
                  = JsonConvert
                      .SerializeObject( serviceResponse, serializationSettings );

            return json;
        }
    }


    public class ServiceResponseTypeNameSerializationBinder 
                   : SerializationBinder {

      public string TypeFormat { get; private set; }
 
      public ServiceResponseTypeNameSerializationBinder 
               ( string typeFormat ) {

        TypeFormat = typeFormat;
      }
 
      public override void BindToName
                             ( Type serializedType
                             , out string assemblyName
                             , out string typeName ) {

        assemblyName = null;
        typeName = serializedType.Name;
      }
 
      public override Type BindToType
                            ( string assemblyName
                            , string typeName ) {

        var resolvedTypeName = string.Format(TypeFormat, typeName);
 
        return Type.GetType(resolvedTypeName, true);
      }
    }

}
