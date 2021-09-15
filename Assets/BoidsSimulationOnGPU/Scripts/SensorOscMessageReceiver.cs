using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SensorOscMessageReceiver : MonoBehaviour
{
    [Range(0, 2048)]
    public int OscMessageQueueMaxCount = 256;
    public string SensorOscAddress = "/point";
    [SerializeField]
    Queue<object[]> _oscMessageQueue = new Queue<object[]>();
    public Queue<object[]> oscMessageQueue => _oscMessageQueue;
    public bool ShowReceivedMessageOnDebugLog = false;
    void OnDestroy()
    {
        _oscMessageQueue.Clear();
        _oscMessageQueue = null;
    }
    public void OnReceived(Osc.OscPort.Capsule c)
    {
        if (c.message.path == SensorOscAddress)
        {
            if (_oscMessageQueue.Count < OscMessageQueueMaxCount)
                _oscMessageQueue.Enqueue(c.message.data);
        }
        if (ShowReceivedMessageOnDebugLog)
        {
            Debug.Log(c.message.path + " : " + MessageToTextArray(c.message.data));
        }
    }
    public void OnReceivedDebug(object[] message)
    {
        if (_oscMessageQueue.Count < OscMessageQueueMaxCount)
            _oscMessageQueue.Enqueue(message);
        if (ShowReceivedMessageOnDebugLog)
        {
            Debug.Log("mouse debug : " + MessageToTextArray(message));
        }
    }
    string MessageToTextArray(object[] message)
    {
        var len = message.Length;
        string resultText = "";
        for (var i = 0; i < len; i++)
        {
            resultText += message[i].ToString() + (i < len ? " " : "");
        }
        return resultText;
    }
}












