using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using System.IO;
using System.Threading.Tasks;

namespace ImageFunctions
{
    public class ServiceBusTopicCommunicationClient
    {
        private readonly string serviceUri = "Endpoint=sb://academyimageservicebusnamespace.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=EbYro1SiAvTtQhMo0U5iA8WRkvxvFOui8fmxdA86AOo=";
        private readonly string topicName = "ImageTopic";
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

            if (string.IsNullOrWhiteSpace(topicName))
            {
                sendClient = new TopicClient(new ServiceBusConnectionStringBuilder(serviceUri));
            }
            else
            {
                sendClient = new TopicClient(serviceUri, topicName);
            }
        }
    }
}
