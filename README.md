# IoT Edge Secure Remote Access 
[![Build Status](https://dev.azure.com/suneetnangia/IotEdgeAccess/_apis/build/status/suneetnangia.IoTEdgeAccess?branchName=master)](https://dev.azure.com/suneetnangia/IotEdgeAccess/_build/latest?definitionId=13&branchName=master)

This is a solution for IoT Edge module which allows remote access by using device stream feature of IoT Hub. Module runs IoT device virtually on the edge device which takes advantage of the security features of device stream. Both clients and Edge/Device makes outbound connection to the streaming endpoint of IoTHub, no inbound connection is made to either client or Edge/Device. It uses WebSockets to stream binary data between clients and the edge device.

![solution design](./Architecture/EdgeAccess.JPG)


To learn more about device stream feature, see here-
https://docs.microsoft.com/en-us/azure/iot-hub/iot-hub-device-streams-overview
