using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Confluent.Kafka;
using Partitioner = Confluent.Kafka.Partitioner;

namespace SimpleWebApp.Helpers
{
    public class KafkaProducer
    {
        private readonly KafkaOptions _kafkaOptions;
        private readonly IProducer<string, string> _producer;

        public KafkaProducer(IConfiguration configuration)
        {
            _kafkaOptions = new KafkaOptions();
            configuration.GetSection(KafkaOptions.Kafka).Bind(_kafkaOptions);

            var config = new ProducerConfig
            {
                BootstrapServers = _kafkaOptions.Broker,
                Partitioner = Partitioner.Murmur2,
                Acks = Acks.Leader,
                MessageTimeoutMs = 30000
            };

            _producer = new ProducerBuilder<string, string>(config).Build();
        }

        public async Task<DeliveryResult<string, string>> ProduceMessage(string key, string message)
        {
            DeliveryResult<string, string> deliveryResult = null;
            try
            {
                deliveryResult = await _producer.ProduceAsync(
                    _kafkaOptions.Performance_Guild_Awesome_Topic,
                    new Message<string, string>
                    {
                        Key = key,
                        Value = message
                    });
                Console.WriteLine(
                    $"Message with offset: {deliveryResult.Offset.Value}, key: {deliveryResult.Message.Key} and value: {deliveryResult.Value} \nWith date: " +
                    DateTime.UtcNow);
            }
            catch (ProduceException<Null, string> e)
            {
                Console.WriteLine($"Delivery failed: {e.Error.Reason}");
            }

            return deliveryResult;
        }
    }
}