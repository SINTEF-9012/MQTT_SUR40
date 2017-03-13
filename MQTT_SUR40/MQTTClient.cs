using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using DataVisualizer.Properties;
using Newtonsoft.Json;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace MQTT_SUR40
{
    public class MQTTClient
    {
        private static MQTTClient _instance;
        
        private readonly MqttClient _mqttClient;
        private readonly UdpClient _udpClient;

        private readonly IPEndPoint _ipEndPoint;

        private MQTTClient()
        {

            if (Settings.Default.MQTT_Mode)
            {
                _mqttClient = new MqttClient(Settings.Default.MQTT_Message_Broker,
                    Settings.Default.MQTT_Message_Broker_Port,
                    false, null, null, MqttSslProtocols.None);
            }

            _udpClient = new UdpClient();
            _ipEndPoint = new IPEndPoint(IPAddress.Parse(Settings.Default.UDP_IP), Settings.Default.UDP_Port);

        }

        private static byte[] Combine(params byte[][] arrays)
        {
            byte[] ret = new byte[arrays.Sum(x => x.Length)];
            int offset = 0;
            foreach (byte[] data in arrays)
            {
                Buffer.BlockCopy(data, 0, ret, offset, data.Length);
                offset += data.Length;
            }
            return ret;
        }

        public void Connect()
        {
            if (_mqttClient != null)
                _mqttClient.Connect(Settings.Default.MQTT_Client_ID);
        }

        public void Disconnect()
        {
            if (_mqttClient != null)
            {
                try
                {
                    _mqttClient.Disconnect();
                }
                catch
                {
                }
            }

        }

        public void PublishMessage(MQTT_SUR40_Message message)
        {
            var bytesId = BitConverter.GetBytes(message.Id);
            char type = 'u';
            switch (message.Type)
            {
                case "Tag":
                    type = 't';
                    break;
                case "Finger":
                    type = 'f';
                    break;
                case "Blob":
                    type = 'b';
                    break;
            }
            var bytesType = BitConverter.GetBytes(type);

            var bytesX = BitConverter.GetBytes((float)message.X);
            var bytesY = BitConverter.GetBytes((float)message.Y);
            var bytesOrientation = BitConverter.GetBytes((float)message.Orientation);

            byte[] binaryMessage = Combine(bytesId, bytesX, bytesY, bytesOrientation, bytesType);
            _udpClient.SendAsync(binaryMessage, binaryMessage.Length, _ipEndPoint);

            if (_mqttClient == null || !_mqttClient.IsConnected) return;

            Task.Run(() =>
            {
                var json = JsonConvert.SerializeObject(
                    message,
                    Formatting.None,
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });

                var bytes = System.Text.Encoding.UTF8.GetBytes(json);

                _mqttClient.Publish(Settings.Default.MQTT_Topic + message.Id, bytes, MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
            });
        }

        public String Status()
        {
            return (_mqttClient != null && _mqttClient.IsConnected) ? "Connected" : "Disconnected";
        }

        public static MQTTClient Instance
        {
            get { return _instance ?? (_instance = new MQTTClient()); }
        }

        public void RemoveTag(int id)
        {
            var bytesId = BitConverter.GetBytes(id);
            _udpClient.SendAsync(bytesId, bytesId.Length, _ipEndPoint);

            if (_mqttClient != null)
            {
                _mqttClient.Publish(Settings.Default.MQTT_Topic + id,new byte[]{} , MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
            }

        }
    }
}
