using Confluent.Kafka;
using InvestmentControl.KafkaProducer;

var builder = Host.CreateApplicationBuilder(args);

var producerConfig = new ProducerConfig
{
    BootstrapServers = Environment.GetEnvironmentVariable("KAFKA_BOOTSTRAP_SERVERS"),
    Acks = Acks.All
};

builder.Services.AddSingleton(producerConfig);

builder.Services.AddHttpClient("B3Api", client =>
{
    client.BaseAddress = new Uri("https://b3api.vercel.app/api/");
    client.Timeout = TimeSpan.FromSeconds(30);
});


builder.Services.AddHostedService<AssetPriceProducerService>();

var host = builder.Build();
host.Run();
