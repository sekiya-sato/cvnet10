namespace Cvnet10Wpfclient.ViewModels.Sub;

public class SelectInputParameter {
	public long? FromId { get; set; }
	public long? ToId { get; set; }
	public string? FromDate { get; set; }
	public string? ToDate { get; set; }
	public string? FromToriCd { get; set; }
	public string? ToToriCd { get; set; }
	public string? FromSokoCd { get; set; }
	public string? ToSokoCd { get; set; }
	public string? FromShohinCd { get; set; }
	public string? ToShohinCd { get; set; }
	public int? MaxCount { get; set; }
	public string? DisplayName { get; set; }
}
