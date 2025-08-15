// Starts up a Buttplug Server, creates a client, connects to it, and has that client run a device scan.

using System;
using Buttplug.Client;
using Unity.Mathematics;
using UnityEngine;

public class Intiface : MonoBehaviour
{
    public ButtplugClient Client;
    public Action IntifaceDisabled;

    public float HapticStrength { get; set; }


    private async void OnEnable()
    {
        Client = new ButtplugClient("Metronome");
        Log("Trying to create client");

        // Set up client event handlers before we connect.
        Client.DeviceAdded += AddDevice;
        Client.DeviceRemoved += RemoveDevice;
        Client.ScanningFinished += ScanFinished;
        Client.PingTimeout += TimeOut;

        // Creating a Websocket Connector is as easy as using the right
        // options object.
        var connector = new ButtplugWebsocketConnector(new Uri("ws://localhost:12345/buttplug"));

        try
        {
            await Client.ConnectAsync(connector);
            await Client.StartScanningAsync();
        }
        catch (Exception exception)
        {
            Debug.LogError(exception);
            enabled = false;
        }
    }

    private void TimeOut(object sender, EventArgs e)
    {
        Log($"TimeOut");
        enabled = false;
    }

    private async void OnDisable()
    {
        //Devices.Clear();

        // On object shutdown, disconnect the client and just kill the server
        // process. Server process shutdown will be cleaner in future builds.
        if (Client != null)
        {
            Client.DeviceAdded -= AddDevice;
            Client.DeviceRemoved -= RemoveDevice;
            Client.ScanningFinished -= ScanFinished;
            Client.PingTimeout -= TimeOut;
            await Client.DisconnectAsync();
            Client.Dispose();
            Client = null;
        }

        IntifaceDisabled?.Invoke();
        Log("I am destroyed now");
    }


    public void UpdateDevice(ButtplugClientDevice device, float intensity)
    {
        float speed = intensity > 0.001f ? math.max(intensity * HapticStrength, 0.05f) : 0f;
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

    private void AddDevice(object sender, DeviceAddedEventArgs e)
    {
        Log($"Device {e.Device.Name} Connected!");
    }

    private void RemoveDevice(object sender, DeviceRemovedEventArgs e)
    {
        Log($"Device {e.Device.Name} Removed!");
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