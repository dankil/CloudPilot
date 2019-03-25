using Microsoft.Azure.ServiceBus;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ImageFunctions
{
    public class ServiceBusTopicCommunicationClient
    {
        private static readonly string ServiceUri = Environment.GetEnvironmentVariable("AzureServiceBus");
        private static readonly string TopicName = Environment.GetEnvironmentVariable("AzureServiceBusTopic");
        private TopicClient sendClient;

        /// <summary>
        /// Sends a message to the ServiceFabric Service.
        /// </summary>
        /// <returns></returns>
        public async Task SendMessageAsync(Image<Rgba32> image)
        {
            CreateClient();

            using (MemoryStream m = new MemoryStream())
            {
                image.Save(m, new PngEncoder());
                byte[] imageBytes = m.ToArray();

                await sendClient.SendAsync(new Message(imageBytes));
            }

            await sendClient.CloseAsync();
        }

        private void CreateClient()
        {
            if (sendClient != null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(TopicName))
            {
                sendClient = new TopicClient(new ServiceBusConnectionStringBuilder(ServiceUri));
            }
            else
            {
                sendClient = new TopicClient(ServiceUri, TopicName);
            }
        }
    }
}
