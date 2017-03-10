using System;
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

        private MQTTClient()
        {
            
            _mqttClient = new MqttClient(Settings.Default.MQTT_Message_Broker,
                Settings.Default.MQTT_Message_Broker_Port,
                false, null, null, MqttSslProtocols.None);
        }

        public void Connect()
        {
            _mqttClient.Connect(Settings.Default.MQTT_Client_ID);
        }

        public void Disconnect()
        {
            try
            {
                _mqttClient.Disconnect();
            }
            catch
            {
            }
        }

        public void PublishMessage(MQTT_SUR40_Message message)
        {
            if (!_mqttClient.IsConnected) return;

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

            _mqttClient.Publish(Settings.Default.MQTT_Topic + id,new byte[]{} , MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
        }
    }
}
