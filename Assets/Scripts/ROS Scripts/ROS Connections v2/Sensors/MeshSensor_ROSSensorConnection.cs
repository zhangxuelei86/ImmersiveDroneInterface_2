﻿using System;
using UnityEngine;
using System.Collections.Generic;
using SimpleJSON;
using System.Threading;


using ROSBridgeLib;
using ROSBridgeLib.geometry_msgs;
using ROSBridgeLib.sensor_msgs;
using ROSBridgeLib.std_msgs;
using ROSBridgeLib.interface_msgs;
using ROSBridgeLib.voxblox_msgs;

using ISAACS;

public class MeshSensor_ROSSensorConnection : MonoBehaviour, ROSTopicSubscriber, ROSSensorConnectionInterface {

    // Private connection variables
    private ROSBridgeWebSocketConnection ros = null;
    public string client_id;
    private Thread rosMsgThread;
    private Dictionary<string, bool> sensorSubscriberTopicsDict = new Dictionary<string, bool>();

    /// Queue of jsonMsgs to be parsed on thread
    private Queue<JSONNode> jsonMsgs = new Queue<JSONNode>();

    /// Queue of MeshArray Dictionary parsed and generated by thread and to be visualized
    private Queue<Dictionary<long[], MeshArray>> meshDicts = new Queue<Dictionary<long[], MeshArray>>();

    /// <summary>
    /// Visualizer to set meshes.
    /// </summary>
    private MeshVisualizer visualizer;

    /// <summary>
    /// Spawns a thread that parses messages in jsonMsgs.
    /// </summary>
    private void CreateThread()
    {
        rosMsgThread = new Thread( new ThreadStart(ParseJSON));
        rosMsgThread.IsBackground = true;
        rosMsgThread.Start();
    }

    /// <summary>
    /// Parses jsonMsgs and populates meshDicts
    /// </summary>
    private void ParseJSON()
    {
        while (true)
        {
            // Check if any json msgs have been recieved
            if (jsonMsgs.Count > 0)
            {
                //Debug.Log("JSON Message Count: " + jsonMsgs.Count);
                // Parse json msg to mesh msg
                DateTime startTime = DateTime.Now;
                JSONNode rawMsg = jsonMsgs.Dequeue();
                MeshMsg meshMsg = new MeshMsg(rawMsg);
//                meshMsgs.Enqueue(meshMsg);
                //Debug.Log("Message Generation: " + DateTime.Now.Subtract(startTime).TotalMilliseconds.ToString() + "ms");
                startTime = DateTime.Now;
                meshDicts.Enqueue(visualizer.generateMesh(meshMsg));
                //Debug.Log("Generate MeshArray: " + DateTime.Now.Subtract(startTime).TotalMilliseconds.ToString() + "ms");

            }
        }
    }

    public void OnApplicationQuit()
    {
        rosMsgThread.Abort();
    }

    // Initilize the sensor
    public void InitilizeSensor(int uniqueID, string sensorIP, int sensorPort, List<string> sensorSubscribers)
    {
        Debug.Log("Init Mesh Connection at IP " + sensorIP + " Port " + sensorPort.ToString());

        ros = new ROSBridgeWebSocketConnection("ws://" + sensorIP, sensorPort);
        client_id = uniqueID.ToString();

        foreach (string subscriber in sensorSubscribers)
        {
            string subscriberTopic = "";

            switch (subscriber)
            {
                case "mesh":
                    subscriberTopic = "/voxblox_node/" + subscriber;
                    break;
                default:
                    Debug.Log("Subscriber not defined: " + subscriber);
                    break;
            }
            Debug.Log(" Mesh Subscribing to : " + subscriberTopic);
            sensorSubscriberTopicsDict.Add(subscriberTopic, true);
            ros.AddSubscriber(subscriberTopic, this);
        }

        Debug.Log("Mesh Connection Established");
        ros.Connect();

        // Initilize visualizer
        visualizer = this.gameObject.AddComponent<MeshVisualizer>();
        visualizer.CreateMeshVisualizer();
        
        //visualizer = GameObject.Find(rendererObjectName).GetComponent<MeshVisualizer>();
        CreateThread();

        // Hardcode Parent transform
        //SetLocalPosition(new Vector3(1.98f, 0f, -6.65f));
        //SetLocalOrientation(Quaternion.Euler(0f, 124.654f ,0f));
    }

    // Update is called once per frame in Unity
    void Update()
    {
        if (ros != null)
        {
            ros.Render();
        }

        // Check if any mesh msgs are available to be visualized
        if (meshDicts.Count > 0)
        {
            Debug.Log("Mesh Dict Count: " + meshDicts.Count);
            DateTime startTime = DateTime.Now;
            Dictionary<long[], MeshArray> mesh_dict = meshDicts.Dequeue();
            visualizer.SetMesh(mesh_dict);
            Debug.Log("Set Mesh: " + DateTime.Now.Subtract(startTime).TotalMilliseconds.ToString() + "ms");
        }

    }

    public string GetSensorName()
    {
        return this.gameObject.name;
    }

    /// <summary>
    /// Returns a list of connected subscriber topics (which are unique identifiers).
    /// </summary>
    /// <returns></returns>
    public Dictionary<string,bool> GetSensorSubscribers()
    {
        return sensorSubscriberTopicsDict;
    }

    /// <summary>
    /// Function to disconnect a specific subscriber
    /// </summary>
    /// <param name="subscriberID"></param>
    public void Unsubscribe(string subscriberTopic)
    {
        if (sensorSubscriberTopicsDict.ContainsKey(subscriberTopic))
        {
            sensorSubscriberTopicsDict[subscriberTopic] = false;
            ros.RemoveSubscriber(subscriberTopic);
        }
        else
        {
            Debug.Log("No such subscriber exists: " + subscriberTopic);
        }

    }

    /// <summary>
    /// Function to connect a specific subscriber
    /// </summary>
    /// <param name="subscriberID"></param>
    public void Subscribe(string subscriberTopic)
    {
        if (sensorSubscriberTopicsDict.ContainsKey(subscriberTopic))
        {
            if (sensorSubscriberTopicsDict[subscriberTopic] == false)
            {
                ros.AddSubscriber(subscriberTopic, this);
                sensorSubscriberTopicsDict[subscriberTopic] = true;
                return;
            }
        }

        Debug.Log("Subscriber already registered: " + subscriberTopic);
    }

    // ROS Topic Subscriber methods
    public ROSBridgeMsg OnReceiveMessage(string topic, JSONNode raw_msg, ROSBridgeMsg parsed = null)
    {
        ROSBridgeMsg result = null;
        // Writing all code in here for now. May need to move out to separate handler functions when it gets too unwieldy.
        switch (topic)
        {
            case "/voxblox_node/mesh":
                // Add raw_msg to the jsonMsgs to be parsed on the thread
                jsonMsgs.Enqueue(raw_msg);
                break;
            default:
                Debug.LogError("Topic not implemented: " + topic);
                break;
        }
        return result;
    }

    public string GetMessageType(string topic)
    {
        Debug.Log("Mesh message type is returned as voxblox_msgs/Mesh by default");
        return "voxblox_msgs/Mesh";
    }

    public void DisconnectROSConnection()
    {
        ros.Disconnect();
    }

    public void SetLocalOrientation(Quaternion quaternion)
    {
        this.transform.localRotation = quaternion;
        // Hardcoded transform depreciated: this.transform.Rotate(Vector3.up, -60.0f);
    }
    public void SetLocalPosition(Vector3 position)
    {
        this.transform.localPosition = position;
    }

    public void SetLocalScale(Vector3 scale)
    {
        this.transform.localScale = scale;
    }

}
