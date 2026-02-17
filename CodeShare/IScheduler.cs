using ProtoBuf.Grpc;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace CodeShare;

/// <summary>
/// スケジュール登録のためのメッセージ
/// [Message for schedule registration]
/// </summary>
[DataContract]
public sealed class SchedulerMessage {
	[DataMember(Order = 1)]
	public string CronExpression { get; set; } = "* * * * *";
	[DataMember(Order = 2)]
	public Type SchedulerType { get; set; } = typeof(string);
}

[DataContract]
public sealed class SchedulerResult {
	[DataMember(Order = 1)]
	public int Result { get; set; }
	[DataMember(Order = 2)]
	public string Detail { get; set; } = string.Empty;
}


[ServiceContract]
public interface ICvnetScheduler {
	[OperationContract]
	Task<SchedulerResult> AddOneTaskAsync(SchedulerMessage msg, CallContext context = default);

	[OperationContract]
	Task<SchedulerResult> RemoveOneTaskAsync(SchedulerMessage msg, CallContext context = default);

	[OperationContract]
	Task<SchedulerResult> RemoveAllTaskAsync(CallContext context = default);
}
