namespace InvestmentControl.Domain.Models.Integrations;

public class AssetIntegationResponse
{
    public decimal? Price { get; set; }
    public double? PriceOpen { get; set; }
    public double? High { get; set; }
    public double? Low { get; set; }
    public long? Volume { get; set; }
    public long? MarketCap { get; set; }
    public DateTime? TradeTime { get; set; }
    public long? VolumeAvg { get; set; }
    public double? PE { get; set; }
    public double? EPS { get; set; }
    public double? High52 { get; set; }
    public double? Low52 { get; set; }
    public double? Change { get; set; }
    public double? ChangePct { get; set; }
    public double? CloseYest { get; set; }
    public long? Shares { get; set; }
    public string Ticker { get; set; } = String.Empty;
}
