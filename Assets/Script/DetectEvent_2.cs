using System.Collections;
using System.Collections.Generic;
using Unity.Barracuda;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System;
using System.Linq;
public class DetectEvent : MonoBehaviour
{
    public NNModel modelAsset;
    public Text txtResult;
    public RawImage img;
    public Button btnDetect;
    private Model m_RuntimeModel;
    private IWorker m_Worker;
    private int channels;
    public Text txtNotifi;
    private string[] label = { "Angry",  "Happy", "Sad", "Surprise"};


    private MqttClient client;
    private string brokerAddress = "io.adafruit.com";
    private string username = "Nguyen_Tham";
    private string key = "aio_WoJt11tkaH0LH79RzDPW2GIRKACe";

    private string[] feeds = { "detectmood" };

    void Awake()
    {
        if (modelAsset == null)
        {
            Debug.LogError("Model asset is not assigned!");
            return;
        }

        m_RuntimeModel = ModelLoader.Load(modelAsset);
        m_Worker = WorkerFactory.CreateWorker(WorkerFactory.Type.CSharp, m_RuntimeModel);
        channels = 3;
        btnDetect.onClick.AddListener(OnButtonClick);
    }

    void OnButtonClick()
    {
        string predictedLabel = PredictImage();
        txtResult.text = predictedLabel;
        // gui ket qua len cloud
        SendMQTTMessage("detectmood", predictedLabel);
    }

    string PredictImage()
    {
        if (img.texture is not Texture2D texture2D)
        {
            Debug.LogWarning("Texture is not a valid Texture2D");
            return "Invalid Image";
        }

        using (Tensor input = new Tensor(texture2D, channels))
        {
            m_Worker.Execute(input);
            Tensor output = m_Worker.PeekOutput();
            int maxIndex = output.ArgMax()[0];
            string result = label[maxIndex];
            Debug.Log($"Output values: {string.Join(", ", output.ToReadOnlyArray().Select(v => v.ToString("F6")))}");

            output.Dispose();
            return result;
        }
         
    }




    void OnDestroy()
    {
        m_Worker?.Dispose(); // Đảm bảo giải phóng worker khi object bị phá hủy
    }

    void Start()
    {

        try
        {
            client = new MqttClient(brokerAddress);
            client.Connect(username, username, key);

            if (client.IsConnected)
            {
               txtNotifi.text = "MQTT Client connected successfully.!!!";
                client.Subscribe(new string[] { $"{username}/feeds/detectmood" }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
            }
            else
            {
                Debug.LogError("Failed to connect to MQTT broker.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"MQTT connection error: {ex.Message}");
        }
    }


    void SendMQTTMessage(string topic, string value)
    {
        if (client == null || !client.IsConnected)
        {
            Debug.LogError("MQTT client is not connected!");
            return;
        }

        string formattedTopic = $"{username}/feeds/{topic}";
        client.Publish(formattedTopic, Encoding.UTF8.GetBytes(value), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
        Debug.Log($"Publish {topic}: {value}");
    }

    
}
