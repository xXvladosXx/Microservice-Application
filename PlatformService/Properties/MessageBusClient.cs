using System;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using PlatformService.AsyncDataServices;
using PlatformService.Dtos;
using RabbitMQ.Client;

namespace PlatformService.Properties
{
    public class MessageBusClient : IMessageBusClient
    {
        private readonly IConfiguration _configuration;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public MessageBusClient(IConfiguration configuration)
        {
            _configuration = configuration;
            var factory = new ConnectionFactory()
            {
                HostName = _configuration["RabbitMQHost"],
                Port = int.Parse(_configuration["RabbitMQPort"])
            };

            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();
                
                _channel.ExchangeDeclare("trigget", ExchangeType.Fanout);
                _connection.ConnectionShutdown += RabbitMqShutDown;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;  
            }
        }

        private void RabbitMqShutDown(object? sender, ShutdownEventArgs e)
        {
            
        }

        public void PublishNewPlatform(PlatformPublishedDto platformPublishedDto)
        {
            var message = JsonSerializer.Serialize(platformPublishedDto);

            if (_connection.IsOpen)
            {
                Console.WriteLine("Rabbit connection open, sending message");
                SendMessage(message);
            }
            else
            {
                Console.WriteLine("Rabbit connection closed");
            }
        }

        private void SendMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish("trigger", "", null, body);
            
            Console.WriteLine($"we sent message {message}");
        }

        public void Dispose()
        {
            Console.WriteLine("MessageBus Disposed");

            if (_channel.IsOpen)
            {
                _channel.Close();
                _connection.Close();
            }
        }
    }
}