using Confluent.Kafka;
using InvestmentControl.Domain.Models.Entities;
using InvestmentControl.Domain.Models.Integrations;
using InvestmentControl.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace InvestmentControl.WorkerService;

public class QuotationIntegrationService : BackgroundService
{
    private readonly ILogger<QuotationIntegrationService> _logger;
    private readonly IConsumer<Ignore, string> _kafkaConsumer;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public QuotationIntegrationService(ILogger<QuotationIntegrationService> logger, ConsumerConfig consumerConfig, IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _kafkaConsumer = new ConsumerBuilder<Ignore, string>(consumerConfig)
            .SetErrorHandler((_, e) => _logger.LogError($"Kafka Consumer Error: {e.Reason}. IsFatal: {e.IsFatal}"))
            .SetStatisticsHandler((_, json) => _logger.LogDebug($"Kafka Consumer Stats: {json}"))
            .Build();
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _kafkaConsumer.Subscribe("asset-price-updates");

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var consumeResult = _kafkaConsumer.Consume(cancellationToken);

                if (consumeResult.IsPartitionEOF)
                {
                    _logger.LogInformation(
                        $"Reached end of topic {consumeResult.Topic}, partition {consumeResult.Partition}, offset {consumeResult.Offset}. Waiting for more messages...");
                    continue;
                }

                _logger.LogInformation($"Consumed message from Topic: {consumeResult.Topic}, Partition: {consumeResult.Partition}, Offset: {consumeResult.Offset}");

                try
                {
                     var quotationData = JsonSerializer.Deserialize<AssetResponse>(consumeResult.Message.Value);

                    if (quotationData is null)
                    {
                        _logger.LogWarning($"Failed to deserialize Kafka message or quotationData is null. Raw: {consumeResult.Message.Value}");
                        return;
                    }

                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<BankDbContext>();

                        var asset = await dbContext.Assets
                                                   .FirstOrDefaultAsync(x => x.Code == quotationData.Ticker, cancellationToken);

                        if (asset is null)
                        {
                            asset = Asset.Create(quotationData.Ticker, quotationData.Ticker);
                            dbContext.Assets.Add(asset);
                            _logger.LogInformation($"New Asset created: {asset.Code}");
                        }
                        else
                        {
                            _logger.LogInformation($"Existing Asset found: {asset.Code}");
                        }

                        await dbContext.SaveChangesAsync(cancellationToken);

                        var quotation = Quotation.Create(
                            assetId: asset.Id,
                            unitPrice: quotationData.Price
                        );

                        dbContext.Quotations.Add(quotation);
                        _logger.LogInformation($"New Quotation created for {asset.Code} with price {quotation.UnitPrice}.");

                        await dbContext.SaveChangesAsync(cancellationToken);

                        _logger.LogInformation($"Successfully persisted data for {quotationData.Ticker}.");
                    }

                }
                catch (JsonException jsonEx)
                {
                    _logger.LogError(jsonEx, $"Error deserializing Kafka message: {jsonEx.Message}. Raw: {consumeResult.Message.Value}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error processing Kafka message: {consumeResult.Message.Value}");
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Kafka Consumer Service is stopping gracefully.");
                break;
            }
            catch (ConsumeException ex)
            {
                _logger.LogError(ex, $"Consume error: {ex.Error.Reason}");
                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred during Kafka consumption.");
                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
            }
        }

        _kafkaConsumer.Close();
    }

    public override void Dispose()
    {
        _kafkaConsumer?.Dispose();
        base.Dispose();
    }
}