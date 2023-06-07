using System;
using System.IO.Ports;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Azure.Amqp.Framing;
using Microsoft.Azure.Devices.Client;

namespace AzIotHub.Cli;

// This code for Raspberry PI.
internal class Program
{
	private static DeviceClient _deviceClient;
	private static readonly string 
		DeviceConnectionString = "HostName=test0001-hub.azure-devices.net;DeviceId=device01;SharedAccessKey=cElDhUvJTVbSgy3qfdNJLQLbnTwDfClD1IoeE3Az994=";
	private static int _fanState = 0;
	private static int _ledState = 0;
	private static readonly int BaudRate = 9600;
	private static readonly string PortName = "/dev/ttyACM0"; // "/dev/ttyACM0"
	private static SerialPort _serialPort;
	private static async Task Main(string[] args)
	{
		//ConfigureSerial();
		_deviceClient = DeviceClient.CreateFromConnectionString(DeviceConnectionString, TransportType.Mqtt);
		await _deviceClient.SetMethodHandlerAsync("fan",fan, null);
		await _deviceClient.SetMethodHandlerAsync("led", led,null);
		
		DateTime t1 = DateTime.Now;
		while (true)
		{
			try
			{
				//string sensorData = _serialPort.ReadLine();
				//DateTime t2 = DateTime.Now;
				//if ((t2 - t1).Seconds >= 4.5)
				//{
				//	t1 = t2;
				//	// string correctData = sensorData
				//	// 		.Remove(sensorData.LastIndexOf(';'), 1)
				//	// 		.Replace(';', ',')
				//	// 		.Replace("temperature", "Temperature")
				//	// 		.Replace("humidity", "Humidity")
				//	// 		.Replace("soil_moisture", "SoilMoisture")
				//	//	;
				//	Console.WriteLine(sensorData);
				//	Message message = new Message(Encoding.ASCII.GetBytes(sensorData))
				//		{ ContentType = "application/json" };
				//	await _deviceClient.SendEventAsync(message);
					
				//}
				Console.WriteLine("Running");
				await Task.Delay(1000);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				continue;
			}
			finally
			{
			}

			//await Task.Delay(100);
		}
		
		
	}

	private static void ConfigureSerial()
	{

		_serialPort = new SerialPort(PortName, BaudRate);
		_serialPort.Open();
		Console.WriteLine("Open Serial Connection.");
		Console.CancelKeyPress += (_, _) =>
		{
			_serialPort.Close();
			Console.WriteLine("Close Serial Connection.");
			Environment.Exit(0);
		};
	}

	private static Task<MethodResponse> fan(MethodRequest methodrequest, object usercontext)
	{
		try
		{
			string payload = Encoding.ASCII.GetString(methodrequest.Data);
			// Check if the payload only contains valid integer characters
			if (!int.TryParse(payload, out int newState))
			{
				throw new Exception("Invalid payload format: must be integer.");
			}
			if (_fanState == newState)
			{
				return Task.FromResult(new MethodResponse(Encoding.ASCII.GetBytes("{\"Message\":\"Fan state not changed\"}"), 200));
			}
			_fanState = newState;
			// _serialPort.Write(_fanState == 1 ? "1\n" : "-1\n");
			Console.WriteLine($"Fan state updated: {_fanState}");
			return Task.FromResult(new MethodResponse(Encoding.ASCII.GetBytes($"{{\"Message\":\"Fan state changed {_fanState}\"}}"), 200));
		}
		catch (Exception e)
		{
			// Log the exception message and return error response
			Console.WriteLine(e.Message);
			return Task.FromResult(new MethodResponse(Encoding.ASCII.GetBytes($"{{\"Message\":\"{e.Message}\"}}"), 400));
		}

	}
	private static Task<MethodResponse> led(MethodRequest methodrequest, object usercontext)
	{
		try
		{
			string payload = Encoding.ASCII.GetString(methodrequest.Data);
			// Check if the payload only contains valid integer characters
			if (!int.TryParse(payload, out int newState))
			{
				throw new Exception("Invalid payload format: must be integer.");
			}
			if (_ledState == newState)
			{
				return Task.FromResult(new MethodResponse(Encoding.ASCII.GetBytes("{\"Message\":\"Led state not changed\"}"), 200));
			}
			_ledState = newState;
			// _serialPort.Write(_ledState == 1 ? "1\n" : "-1\n");
			Console.WriteLine($"Led state updated: {_ledState}");
			return Task.FromResult(new MethodResponse(Encoding.ASCII.GetBytes($"{{\"Message\":\"Led state changed {_ledState}\"}}"), 200));
		}
		catch (Exception e)
		{
			// Log the exception message and return error response
			Console.WriteLine(e.Message);
			return Task.FromResult(new MethodResponse(Encoding.ASCII.GetBytes($"{{\"Message\":\"{e.Message}\"}}"), 400));
		}

	}

	//private static double GetDummySoilMoisture()
	//{
	//	return NextGaussian(60.0, 2.5);
	//}

	//private static double GetDummyHumidity()
	//{
	//	return NextGaussian(70.0, 10);
	//}

	//private static double GetDummyTemperature()
	//{
	//	return NextGaussian(26.0, 3.6);
	//}

	//private static double NextGaussian(double mean, double standardDeviation)
	//{
	//	double u1 = 1.0 - Random.Shared.NextDouble();
	//	double u2 = 1.0 - Random.Shared.NextDouble();
	//	double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
	//	double randNormal = mean + standardDeviation * randStdNormal;
	//	return randNormal;
	//}


	//private static double GetRandomNumber(double min, double max)
	//{
	//	if (min > max)
	//	{
	//		throw new ArgumentException("min value have to less than max value");
	//	}
	//	return Random.Shared.NextDouble() * (max - min) + min;
	//}
}
//public struct SensorInfo
//{
//	public double Temperature;
//	public double Humidity;
//	public double SoilMoisture;
//}

