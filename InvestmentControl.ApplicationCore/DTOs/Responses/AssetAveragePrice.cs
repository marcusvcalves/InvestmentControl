namespace InvestmentControl.ApplicationCore.DTOs.Responses;

public class AssetAveragePrice
{
    public Guid AssetId { get; set; }
    public string AssetCode { get; set; }
    public string AssetName { get; set; }
    public int CurrentQuantity { get; set; }
    public decimal AveragePrice { get; set; }
}
