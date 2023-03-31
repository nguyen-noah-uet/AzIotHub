using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;

namespace AzIotHub.Cli;

internal class Program
{
	private static DeviceClient _deviceClient;
	private static readonly string 
		DeviceConnectionString = "HostName=cosodoluong-hub.azure-devices.net;DeviceId=RaspberryPi;SharedAccessKey=gBmI4erLbh0ua9SkbKKMhZBLpyIoXddExhrcprhQp4c=";

	private static async Task Main(string[] args)
	{
		_deviceClient = DeviceClient.CreateFromConnectionString(DeviceConnectionString, TransportType.Mqtt);
		var telemetryDataPoint = new
		{
			time_stamp=DateTime.Now,
			temperature = 25,
			humidity = 70
		};
		string s = JsonConvert.SerializeObject(telemetryDataPoint);
		Message message = new Message(Encoding.ASCII.GetBytes(s)) { ContentType = "application/json" };
		try
		{
			await _deviceClient.SendEventAsync(message);
			Console.WriteLine(s);

		}
		catch (Exception ex)
		{
			Console.WriteLine("Exception while sending message: {0}", ex.Message);
		}
		
	}
}