# IoT Edge Secure Remote Access 
[![Build Status](https://dev.azure.com/suneetnangia/IotEdgeAccess/_apis/build/status/suneetnangia.IoTEdgeAccess?branchName=master)](https://dev.azure.com/suneetnangia/IotEdgeAccess/_build/latest?definitionId=13&branchName=master)

This is a solution for IoT Edge module which allows remote access by using device stream feature of IoT Hub. Module runs IoT device virtually on the edge device which takes advantage of the security features of device stream. Both clients and Edge/Device makes outbound connection to the streaming endpoint of IoTHub, no inbound connection is made to either client or Edge/Device. It uses WebSockets to stream binary data between clients and the edge device.

![solution design](./Architecture/EdgeAccess.JPG)

Features-
1. JIT Access.
2. Auditing.
3. Secure Access via Device Stream.

Surface Attack Area-
In this instance access to your Edge device is delegated to IoT Hub. It is therefore important to ensure you take care before granting service connect access permission to any clients, follow a principle of least prevlidge.

Why hosting virtual devices in a module?
Hosting a device virtually in a module has some benefits which can be useful in edge scenarios.

1. You can host multiple virtual devices/protocols in a single module with lower resource (memory/cpu) footprint compared to hosting multiple modules one for each protocol (e.g. SSH, SCP, RDP)
2. You do not have to expose unbounded ports on the edge, each virtual device can be restricted to a specific port, reducing surface attack area.
3. Each virtual device can be individually turned off/on on-demand basis from IoT Hub to allow Just in Time (JIT) access, further reducing the surface attack area.
4. Single management plane (IoT Hub) for access management.
5. Secure reverse connect mechanism underpinned by device stream feature.

To learn more about device stream feature, see here-
https://docs.microsoft.com/en-us/azure/iot-hub/iot-hub-device-streams-overview
