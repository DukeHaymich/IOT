import paho.mqtt.client as mqttclient
import time
import json
import random
import geocoder

BROKER_ADDRESS = "demo.thingsboard.io"
PORT = 1883
THINGS_BOARD_ACCESS_TOKEN = "vjbvCspr9EUMCvqUBhwc"


def subscribed(client, userdata, mid, granted_qos):
    print("Subscribed...")


def recv_message(client, userdata, message):
    print("Received: ", message.payload.decode("utf-8"))
    temp_data = {'value': True}
    try:
        jsonobj = json.loads(message.payload)
        if jsonobj['method'] == "setValue":
            temp_data['value'] = jsonobj['params']
            client.publish('v1/devices/me/attributes', json.dumps(temp_data), 1)
    except:
        pass


def connected(client, usedata, flags, rc):
    if rc == 0:
        print("Thingsboard connected successfully!!")
        client.subscribe("v1/devices/me/rpc/request/+")
    else:
        print("Connection is failed")


client = mqttclient.Client("DukeInTheBox")
client.username_pw_set(THINGS_BOARD_ACCESS_TOKEN)

client.on_connect = connected
client.connect(BROKER_ADDRESS, 1883)
client.loop_start()

client.on_subscribe = subscribed
client.on_message = recv_message

latitude = geocoder.ip('me').latlng[0]
longitude = geocoder.ip('me').latlng[1]

while True:
    temp = random.uniform(5.0, 40.0)
    humid = random.randint(0, 100)
    collect_data = {'temperature': temp,
                    'humidity': humid,
                    'longitude': longitude,
                    'latitude': latitude}
    client.publish('v1/devices/me/telemetry', json.dumps(collect_data), 1)
    time.sleep(10)

