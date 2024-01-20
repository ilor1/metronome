// Starts up a Buttplug Server, creates a client, connects to it, and has that
// client run a device scan. All output goes to the Unity Debug log.
//
// This is just a generic behavior, so you can attach it to any active object in
// your scene and it'll run on scene load.

using System;
using System.Collections.Generic;
using Buttplug.Client;
using Buttplug.Client.Connectors.WebsocketConnector;
using Unity.Mathematics;
using UnityEngine;

public class Intiface : MonoBehaviour
{
    public float Intensity;
    private ButtplugClient _client;

    public List<ButtplugClientDevice> Devices { get; } = new List<ButtplugClientDevice>();


    public float HapticStrength { get; set; }


    private async void Start()
    {
        Intensity = 0f;
        HapticStrength = 1.0f;
        
        _client = new ButtplugClient("Metronome");
        Log("Trying to create client");

        // Set up client event handlers before we connect.
        _client.DeviceAdded += AddDevice;
        _client.DeviceRemoved += RemoveDevice;
        _client.ScanningFinished += ScanFinished;

        // Creating a Websocket Connector is as easy as using the right
        // options object.
        var connector = new ButtplugWebsocketConnector(new Uri("ws://localhost:12345/buttplug"));
        await _client.ConnectAsync(connector);
        await _client.StartScanningAsync();
    }

    private async void OnDestroy()
    {
        Devices.Clear();

        // On object shutdown, disconnect the client and just kill the server
        // process. Server process shutdown will be cleaner in future builds.
        if (_client != null)
        {
            _client.DeviceAdded -= AddDevice;
            _client.DeviceRemoved -= RemoveDevice;
            _client.ScanningFinished -= ScanFinished;
            await _client.DisconnectAsync();
            _client.Dispose();
            _client = null;
        }

        Log("I am destroyed now");
    }

    private void OnValidate()
    {
        UpdateDevices();
    }

    public void UpdateDevices()
    {
        float speed = Intensity > 0.001f ? math.max(Intensity * HapticStrength, 0.05f) : 0f;

        // Debug.Log($"Intiface speed:{_intensity}, adjusted speed:{speed}");
        foreach (ButtplugClientDevice device in Devices)
        {
            // if (device.LinearAttributes.Count > 0)
            // {
            //     uint duration = 1500;
            //     device.LinearAsync(duration, speed);
            // }

            if (device.VibrateAttributes.Count > 0)
            {
                device.VibrateAsync(speed);
            }

            if (device.OscillateAttributes.Count > 0)
            {
                device.OscillateAsync(speed);
            }

            if (device.RotateAttributes.Count > 0)
            {
                device.RotateAsync(speed, true);
            }
        }
    }

    private void AddDevice(object sender, DeviceAddedEventArgs e)
    {
        Log($"Device {e.Device.Name} Connected!");
        Devices.Add(e.Device);
        UpdateDevices();
    }

    private void RemoveDevice(object sender, DeviceRemovedEventArgs e)
    {
        Log($"Device {e.Device.Name} Removed!");
        Devices.Remove(e.Device);
        UpdateDevices();
    }

    private void ScanFinished(object sender, EventArgs e)
    {
        Log("Device scanning is finished!");
    }

    private void Log(object text)
    {
        Debug.Log("<color=red>Buttplug:</color> " + text, this);
    }
}