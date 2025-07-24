using Confluent.Kafka;
using InvestmentControl.Domain.Models.Integrations;
using System.Text.Json;

namespace InvestmentControl.KafkaProducer;

public class AssetPriceProducerService : BackgroundService
{
    private readonly ILogger<AssetPriceProducerService> _logger;
    private readonly IProducer<Null, string> _kafkaProducer;
    private readonly HttpClient _httpClient;

    public AssetPriceProducerService(ILogger<AssetPriceProducerService> logger,
                                    IHttpClientFactory httpClientFactory,
                                    ProducerConfig producerConfig)
    {
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient("B3Api");
        _kafkaProducer = new ProducerBuilder<Null, string>(producerConfig).Build();
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        string apiRelativePath = "Assets/";
        string kafkaTopic = "asset-price-updates";

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var httpResponseMessage = await _httpClient.GetAsync(apiRelativePath, cancellationToken);
                httpResponseMessage.EnsureSuccessStatusCode();

                var content = await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken);

                var assetsResponse = JsonSerializer.Deserialize<List<AssetIntegationResponse>>(
                    content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (assetsResponse != null && assetsResponse.Any())
                {
                    foreach (var assetResponseFromApi in assetsResponse)
                    {
                        if (!string.IsNullOrWhiteSpace(assetResponseFromApi.Ticker) && assetResponseFromApi.Price.HasValue)
                        {
                            var messageValueJson = JsonSerializer.Serialize(assetResponseFromApi,
                                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                            var deliveryResult = await _kafkaProducer.ProduceAsync(
                                kafkaTopic,
                                new Message<Null, string> { Value = messageValueJson },
                                cancellationToken);

                            _logger.LogInformation($"Produced message for Ticker: {assetResponseFromApi.Ticker} to topic: {kafkaTopic}. Offset: {deliveryResult.Offset}");
                        }
                        else
                        {
                            _logger.LogWarning($"Skipping item from API (Ticker: {assetResponseFromApi.Ticker ?? "N/A"}) due to missing essential data (price or tradetime).");
                        }
                    }
                }
                else
                {
                    _logger.LogWarning("No valid quotation data received from API to produce Kafka messages.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching or processing data from API.");
            }

            // 1 minuto de espera para evitar rate limiting
            int seconds = 60;

            _logger.LogInformation($"Producer waiting for {seconds} seconds before next fetch...");
            await Task.Delay(TimeSpan.FromSeconds(seconds), cancellationToken);
        }

        _kafkaProducer.Flush(TimeSpan.FromSeconds(10));
        _kafkaProducer.Dispose();
    }
}