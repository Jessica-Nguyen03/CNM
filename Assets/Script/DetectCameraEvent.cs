using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Unity.Barracuda;
using uPLibrary.Networking.M2Mqtt; // MQTT library
using uPLibrary.Networking.M2Mqtt.Messages; // MQTT message

public class CameraMoodDetector : MonoBehaviour
{
    public NNModel modelAsset;
    public Text txtResult;
    public RawImage img;
    public Text txtNotifi;

    private Model m_RuntimeModel;
    private IWorker m_Worker;
    private string[] label = { "Angry", "Happy", "Sad", "Surprise" };
    private MqttClient client;
    private string brokerAddress = "io.adafruit.com";
    private string username = "Nguyen_Tham";
    private string key = "aio_WoJt11tkaH0LH79RzDPW2GIRKACe";
    private string[] feeds = { "detectmood" };

    void Start()
    {
        // Khởi tạo mô hình AI
        m_RuntimeModel = ModelLoader.Load(modelAsset);
        m_Worker = WorkerFactory.CreateWorker(WorkerFactory.Type.Auto, m_RuntimeModel);

        // Khởi tạo MQTT
        InitializeMqtt();

        // Bắt đầu Coroutine tự động dò và gửi
        StartCoroutine(DetectPeriodically());
    }

    void OnDestroy()
    {
        m_Worker?.Dispose();
        client?.Disconnect();
    }

    private void InitializeMqtt()
    {
        client = new MqttClient(brokerAddress);
        client.Connect(username, username, key);
        txtNotifi.text = client.IsConnected ? "Connected to Adafruit IO" : "Connection Failed";
    }

    private IEnumerator DetectPeriodically()
    {
        while (true)
        {
            string mood = PredictFromCamera();
            txtResult.text = $"Mood: {mood}";
            SendToAdafruit(mood);
            yield return new WaitForSeconds(10); // Chờ 10 giây
        }
    }

    private string PredictFromCamera()
    {
        if (img.texture is not WebCamTexture webCamTexture)
        {
            Debug.LogWarning("RawImage không phải là WebCamTexture.");
            return "Invalid Image";
        }

        // Chuyển WebCamTexture sang Texture2D
        Texture2D texture2D = new Texture2D(webCamTexture.width, webCamTexture.height, TextureFormat.RGB24, false);
        texture2D.SetPixels(webCamTexture.GetPixels());
        texture2D.Apply();

        // Thực hiện dự đoán
        using (Tensor input = new Tensor(texture2D, channels: 3)) // Giả định ảnh RGB
        {
            m_Worker.Execute(input);
            Tensor output = m_Worker.PeekOutput();
            int maxIndex = output.ArgMax()[0]; // Chỉ số của giá trị lớn nhất
            string result = label[maxIndex]; // Nhãn tương ứng

            Debug.Log($"Kết quả dự đoán: {result}");
            Debug.Log($"Giá trị đầu ra: {string.Join(", ", output.ToReadOnlyArray().Select(v => v.ToString("F6")))}");

            output.Dispose();
            return result;
        }
    }

    private void SendToAdafruit(string data)
    {
        if (client != null && client.IsConnected)
        {
            string topic = $"{username}/feeds/{feeds[0]}";
            client.Publish(topic, System.Text.Encoding.UTF8.GetBytes(data), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
            txtNotifi.text = $"Đã gửi: {data}";
        }
        else
        {
            txtNotifi.text = "Không kết nối với Adafruit IO";
        }
    }
}
