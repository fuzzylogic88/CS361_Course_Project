# Microservice which delivers image data to main application via Unsplash API
# Author: Daniel Green (greend5@oregonstate.edu)

import zmq
import requests
import base64
from imageio import imread

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
        image = imread(image_location)
        
        # base64 encode image data before transmission
        print("encoding data...")
        bytes = bytearray(image)
        b64_str = base64.b64encode(bytes)
        socket.send(b64_str)
        print("image data sent!")

    except Exception as ex:
        socket.send_string("QUERY_FAIL")
        print(ex)
        continue