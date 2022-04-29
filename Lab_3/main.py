import requests
import json
import time
import paho.mqtt.client as mqttclient
import serial.tools.list_ports

mess = ""
bbc_port = "COM9"
temp = 0
light = 0
if len(bbc_port) > 0:
    ser = serial.Serial(port=bbc_port, baudrate=115200)

print("Iot_Gateway")


BROKER_ADDRESS = "demo.thingsboard.io"
PORT = 1883
THINGS_BOARD_ACCESS_TOKEN = "R5AX5OWilCPYXRwfUoxJ"


def subscribed(client, userdata, mid, granted_qos):
    print("Subscribed...")


def recv_message(client, userdata, message):
    print("Received: ", message.payload.decode("utf-8"))
    temp_data = {'value': True}
    cmd = 0
    # TODO: Update the cmd to control 2 devices
    try:
        jsonobj = json.loads(message.payload)
        # if jsonobj['method'] == "setValue":
        #     temp_data['value'] = jsonobj['params']
        #     client.publish('v1/devices/me/attributes',
        #                    json.dumps(temp_data), 1)
        if jsonobj['method'] == "setLED":
            temp_data['valueLED'] = jsonobj['params']
            if jsonobj['params'] == True:
                cmd = 1
            else:
                cmd = 0
            client.publish('v1/devices/me/attributes',
                           json.dumps(temp_data), 1)
        if jsonobj['method'] == "setFan":
            temp_data['valueFan'] = jsonobj['params']
            if jsonobj['params'] == True:
                cmd = 3
            else:
                cmd = 2
            client.publish('v1/devices/me/attributes',
                           json.dumps(temp_data), 1)
    except:
        pass
    if len(bbc_port) > 0:
        ser.write((str(cmd) + "#").encode())


def connected(client, usedata, flags, rc):
    if rc == 0:
        print("Thingsboard connected successfully!!")
        client.subscribe("v1/devices/me/rpc/request/+")
    else:
        print("Connection is failed")


def processData(data):
    data = data.replace("!", "")
    data = data.replace("#", "")
    splitData = data.split(":")
    print(splitData)
    # TODO: Add your source code to publish data to the server
    tmp = splitData[2]
    global temp, light

    if splitData[1] == 'TEMP':
        temp = int(tmp)
    if splitData[1] == 'LIGHT':
        light = int(tmp)
    collect_data = {'temperature': temp,
                    'light': light,
                    }
    client.publish('v1/devices/me/telemetry', json.dumps(collect_data), 1)


def readSerial():
    bytesToRead = ser.inWaiting()
    if (bytesToRead > 0):
        global mess
        mess = mess + ser.read(bytesToRead).decode("UTF-8")
        while ("#" in mess) and ("!" in mess):
            start = mess.find("!")
            end = mess.find("#")
            processData(mess[start:end + 1])
            if (end == len(mess)):
                mess = ""
            else:
                mess = mess[end+1:]


client = mqttclient.Client("Gateway_Thingsboard")
client.username_pw_set(THINGS_BOARD_ACCESS_TOKEN)

client.on_connect = connected
client.connect(BROKER_ADDRESS, 1883)
client.loop_start()

client.on_subscribe = subscribed
client.on_message = recv_message

temp = 30
humi = 50
light_intensity = 100
longitude = 106.6297
latitude = 10.8231
counter = 0
while True:
    # collect_data = {'temperature': temp,
    #                 'humidity': humi,
    #                 'light': light_intensity,
    #                 'longitude': longitude,
    #                 'latitude': latitude
    #                 }
    # temp += 1
    # humi += 1
    # light_intensity += 1
    # client.publish('v1/devices/me/telemetry', json.dumps(collect_data), 1)

    if len(bbc_port) > 0:
        readSerial()
    time.sleep(1)
