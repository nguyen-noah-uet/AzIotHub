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
	//private static SerialPort _serialPort = new SerialPort(PortName, BaudRate);
	//private static readonly string PortName = "COM4"; // "/dev/ttyACM0"
	private static readonly int BaudRate = 9600;
	private static async Task Main(string[] args)
	{
		//ConfigureSerial(_serialPort);
		_deviceClient = DeviceClient.CreateFromConnectionString(DeviceConnectionString, TransportType.Mqtt);
		await _deviceClient.SetMethodDefaultHandlerAsync(fan, null);
		
		
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
				//	string correctData = sensorData
				//			.Remove(sensorData.LastIndexOf(';'), 1)
				//			.Replace(';', ',')
				//			.Replace("temperature", "Temperature")
				//			.Replace("humidity", "Humidity")
				//			.Replace("soil_moisture", "SoilMoisture")
				//		;
				//	Message message = new Message(Encoding.ASCII.GetBytes(correctData))
				//		{ ContentType = "application/json" };
				//	await _deviceClient.SendEventAsync(message);
				//	Console.WriteLine(correctData);
				//}
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

	//private static void ConfigureSerial(SerialPort? serialPort)
	//{
	//	serialPort.Open();
	//	Console.WriteLine("Open Serial Connection.");
	//	Console.CancelKeyPress += (_, _) =>
	//	{
	//		_serialPort.Close();
	//		Console.WriteLine("Close Serial Connection.");
	//		Environment.Exit(0);
	//	};
	//}

	private static Task<MethodResponse> fan(MethodRequest methodrequest, object usercontext)
	{
		try
		{
			string payload = Encoding.ASCII.GetString(methodrequest.Data);
			int newState = int.Parse(payload);
			if(_fanState == newState)
				return Task.FromResult(new MethodResponse(Encoding.ASCII.GetBytes("{\"Message\":\"Fan state not changed\"}"), 200));
			_fanState = newState;
			//_serialPort.Write(_fanState == 1 ? "1" : "0");
			
			Console.WriteLine($"Fan state updated: {_fanState}");
			return Task.FromResult(new MethodResponse(Encoding.ASCII.GetBytes("{\"Message\":\"Fan state updated\"}"), 200));
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			return Task.FromResult(new MethodResponse(Encoding.ASCII.GetBytes($"\"Message\":\""+e.Message+"\""), 400));
		}
		
	}

	private static double GetDummySoilMoisture()
	{
		return NextGaussian(60.0, 2.5);
	}

	private static double GetDummyHumidity()
	{
		return NextGaussian(70.0, 10);
	}

	private static double GetDummyTemperature()
	{
		return NextGaussian(26.0, 3.6);
	}

	private static double NextGaussian(double mean, double standardDeviation)
	{
		double u1 = 1.0 - Random.Shared.NextDouble();
		double u2 = 1.0 - Random.Shared.NextDouble();
		double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
		double randNormal = mean + standardDeviation * randStdNormal;
		return randNormal;
	}


	private static double GetRandomNumber(double min, double max)
	{
		if (min > max)
		{
			throw new ArgumentException("min value have to less than max value");
		}
		return Random.Shared.NextDouble() * (max - min) + min;
	}
}
public struct SensorInfo
{
	public double Temperature;
	public double Humidity;
	public double SoilMoisture;
}

