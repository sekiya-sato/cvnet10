using ProtoBuf.Grpc;
using System.Runtime.Serialization;
using System.ServiceModel;


namespace CodeShare;

[DataContract]
public class HelloRequest {
	[DataMember(Order = 1)]
	public string Name { get; set; } = "";
}
[DataContract]
public class HelloReply {
	[DataMember(Order = 1)]
	public string Message { get; set; } = "";

	[DataMember(Order = 2)]
	public Type Type0 { get; set; } = typeof(string);

	[DataMember(Order = 3)]
	public string JsonMsg { get; set; } ="";
}

[ServiceContract]
public interface IGreeter {
	[OperationContract]
	Task<HelloReply> SayHello(HelloRequest request, CallContext context = default);
}
