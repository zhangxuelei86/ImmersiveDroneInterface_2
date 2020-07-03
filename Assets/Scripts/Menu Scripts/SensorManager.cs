﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ISAACS;

public class SensorManager : MonoBehaviour {

    public Button leftButton;
    public Button rightButton;
    public Text sensorText;
    public GameObject togglePrefab;

    List<ROSSensorConnectionInterface> sensorList = new List<ROSSensorConnectionInterface>();
    List<string> subscriberList;
    ROSSensorConnectionInterface selectedSensor;

    //public bool leftArrow = false;
    //public bool rightArrow = false;
    //public bool displaySensorText = false;
    //any other buttons can be added here

    void Awake()
    {
        leftButton.onClick.AddListener(() => { OnClickEvent(); });
        rightButton.onClick.AddListener(() => { OnClickEvent(); });
       
        ////Adds listener to the button, if component is button
        //if (GetComponent<Button>() != null)
        //{
        //    thisButton = GetComponent<Button>();
        //    thisButton.onClick.AddListener(() => { OnClickEvent(); });
        //}

        ////Adds listener to toggle, if component is toggle
        //else if (GetComponent<Toggle>() != null)
        //{
        //    thisToggle = GetComponent<Toggle>();
        //    thisToggle.onValueChanged.AddListener(delegate { ToggleValueChanged(thisToggle); });
        //}

        //else
        //{
        //    thisSensorText = GetComponent<Text>();
        //}
    }


    void OnClickEvent()
    {
        if (leftButton)
        {
            showPreviousSensor();
        }

        if (rightButton)
        {
            showNextSensor();
        }

        //any other buttons can be added here, if needed
    }


    // function 1: new drone selected: update all the sensors
    // UpdateUI function: paramaters: List of Sensors
    // store the list of sensors as current sensors
    // selected sensor is the first sensor
    // update the ui with the first sensor's subscribers.

    public void initializeSensorUI(List<ROSSensorConnectionInterface> allSensors)
    {
        sensorList.Clear();
        sensorList.AddRange(allSensors);
        selectedSensor = sensorList[0];

        Debug.Log("The sensor's subscribers are: " + subscriberList.ToString());
        updateSensorUI(selectedSensor);

    }

    // function 2: update the ui with the current selected sensor
    // we get the list of subscribers: sensor.GetSensorSubscribers()
    // this list of subscribers: eg: mesh, rad_level_1, rad_level3
    // generate len(list) number of toggles  with the text of each being the subcsriber string
    // upper bound of 6 subscibers per sensor

    public void updateSensorUI(ROSSensorConnectionInterface inputSensor)
    {

        sensorText.text = inputSensor.GetSensorName(); 
        subscriberList = selectedSensor.GetSensorSubscribers();

        foreach (string subscriber in subscriberList)
        {
            GameObject toggleUI = Instantiate(togglePrefab, new Vector3(0, 0, 0), Quaternion.identity);
            Debug.Log(toggleUI.name);
            Toggle thisToggle = toggleUI.GetComponent<Toggle>();
            Text toggleName = toggleUI.GetComponentInChildren<Text>();
            toggleName.text = subscriber;
            thisToggle.onValueChanged.AddListener(delegate { ToggleValueChanged(thisToggle); });
        }
    }

    // function 3: toggles pressed
    // when a toggle is switched off for subscriber "s" : sensor.Unsubscribe(string s);
    // when a toggle is switched on for subscriber "s" : sensor.Subscribe(string s);

    void ToggleValueChanged(Toggle thisToggle)
    {
        Text selectedSubscriberName = thisToggle.GetComponentInChildren<Text>();

        if (thisToggle.isOn)
        {
            selectedSensor.Subscribe(selectedSubscriberName.text);
        }

        else
        {
            selectedSensor.Unsubscribe(selectedSubscriberName.text);
        }
    }

    // function 4: cycle sensor/ display next sensor
    // we can have a queue of all the sensors
    // add the current sensor to the end
    // make the current sensor the first element in the queue
    // call function 2 to update the ui wuth current selected sensor

    //these are seperated so that if later implementations where we want to switch sensors using VRTK buttons, we can

    public void showNextSensor()
    {
        if (selectedSensor == sensorList[sensorList.Count])
        {
            updateSensorUI(sensorList[0]);
        }
        else
        {
            //updateSensorUI( next one thru a queue)
        }
    }

    public void showPreviousSensor()
    {
        if (selectedSensor == sensorList[0])
        {
            updateSensorUI(sensorList[sensorList.Count]);
        }
        else
        {
            //updateSensorUI( previous one)
        }
    }

}
