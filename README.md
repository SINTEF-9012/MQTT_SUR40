# MQTT_SUR40
Broadcast input events from a PixelSense table (SUR40) to a MQTT message broker.

![Schema](https://cloud.githubusercontent.com/assets/45740/23794427/b90384e2-0598-11e7-92a9-cb9c8d329442.png)

## Documentation

The messages are published on the topic `SUR40/{Id}` by default. Subscribe to `SUR40/+` to receive the events in JSON.

When a touch input is finished/removed, a new *empty* message is published on `SUR40/{Id}`.

```json
{
    "Id": 16842750,
    "Type": "Finger",
    "X": 583.97,
    "Y": 329.99,
    "Orientation": 270.0,
    "Width": 25.97,
    "Height": 39.99
}
```

```json
{
    "Id": 16842749,
    "Type": "Blob",
    "X": 640.0,
    "Y": 313.98,
    "Orientation": 0.0,
    "Width": 101.97,
    "Height": 67.99
}
```

```json
{
    "Id": 16842747,
    "Type": "Tag",
    "X": 882.0,
    "Y": 159.99,
    "Orientation": 0.0,
    "Width": 0.0,
    "Height": 0.0,
    "TagSchema": "00000000",
    "TagSeries": "0000000000000042",
    "TagExtendedValue": "0000000000000000",
    "TagValue": "0000000000026773"
}
```

## How to display an external video input while still receiving touch input events

When you select the external video input on the SUR40 device, Windows still runs in the background but stops receiving touch input events. You can open the table and connect the HDMI output from the PC mainboard to the screen HDMI IN, and connect your external video HDMI cable to the HDMI port where the SUR40 PC was connected to.

