using IoTHubTrigger = Microsoft.Azure.WebJobs.EventHubTriggerAttribute;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.Eventhubs;
using System.Text;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;


namespace SoilMoisture.Functions
{
    public static class IoTHubProcessor
    {
        private static HttpClient client = new HttpClient();

        [FunctionName("IoTHubProcessor")]
        public static void Run([IoTHubTrigger("messages/events", connection = "iotHubConn")]EventData message
        [CosmoDB(
            databasename:"my-iot-db",
            collectionName:"dev-data-store",
            ConnectionStringSetting = "dbConn"
        )] IAsyncCollector<dynamic> documentsOut,
        ILogger log)
    
    {
        //log.LogInformation($"C# IoT Hub trigger function processed a message: {Encoding.UTF8.GetString(message.Body.Array)}");

        SoilMoistureModel data = JsonConvert.DeserializeObject<SoilMoistureModel>(Encoding.UTF8.GetString(message.Body.Array));

        log.LogInformation($"Device name - {data.deviceId} \t Moisture Level - {data.moistureLevel} \t Recorded at - {data.recordedAt.ToLongTimeString}")


        if(data.moistureLevel < 400)
        {
            SendEmailAsync().wait()
        }

        documentsOut.AddSync(
            new
            {
                id = Guid.NewGuid().ToString(),
                deviceId = data.deviveId,
                moistureLevel = data.moistureLevel,
                recordedAt = data.recordedAt,                
            }
        ).wait();
    }
    
    
    public static async Task SendEmailAsync()
    {
         
    }
    
    
    
    
    }
}