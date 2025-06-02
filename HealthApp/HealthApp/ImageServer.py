# Microservice which delivers image data to main application via Unsplash API
# Author: Daniel Green (greend5@oregonstate.edu)

import zmq
import requests
from os import system

system("title Image Service")

ACCESS_KEY = "k7uShp7l2QVykqSp-n7F6gU42uj2gU4uQPz9rihEGqo"

# URL to access random image on Unsplash's servers
access_url = f"https://api.unsplash.com/photos/random?client_id={ACCESS_KEY}"

context = zmq.Context()
socket = context.socket(zmq.REP)
socket.bind("tcp://localhost:5557")

while True:
    print("waiting for image request...")
    message = socket.recv().decode()
    print(f'Recieved client message: {message}')
    if len(message) > 0:
        if message == 'Q':
            print('Client has requested to close connection!')
        if message != 'IMG':
            socket.send_string('ERR')

    try:
        response = requests.get(access_url)
        photo = response.json()
        image_location = photo['urls']['raw']
        print(f"URL: {image_location}")

        # download photo at URL
        print("downloading image...")
        img_response = requests.get(image_location)
        image_bytes = img_response.content

        # transmit byte array
        bytes = bytearray(image_bytes)
        socket.send(bytes)

        print("image data sent!")

    except Exception as ex:
        print(ex)
        continue
