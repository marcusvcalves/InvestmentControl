using Confluent.Kafka;
using InvestmentControl.Infrastructure.Configurations.Context;
using InvestmentControl.WorkerService;

var builder = Host.CreateApplicationBuilder(args);

var consumerConfig = new ConsumerConfig
{
    BootstrapServers = Environment.GetEnvironmentVariable("KAFKA_BOOTSTRAP_SERVERS"),
    GroupId = "quotation-processor-group",
    AutoOffsetReset = AutoOffsetReset.Earliest,
    EnableAutoCommit = true,
    AutoCommitIntervalMs = 5000
};

builder.Services.AddSingleton(consumerConfig);

builder.Services.AddBankDbContext(builder.Configuration);

builder.Services.AddHostedService<QuotationIntegrationService>();

var host = builder.Build();
host.Run();
