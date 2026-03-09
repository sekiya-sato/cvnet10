namespace Cvnet10Wpfclient.ViewModels.Sub;

public class SelectCodeParameter {
	public long? FromId { get; set; }
	public long? ToId { get; set; }
	public string? FromCode { get; set; }
	public string? ToCode { get; set; }
	public string? DisplayName { get; set; }
	public string? Name { get; set; }
	public int? MaxCount { get; set; }
}
