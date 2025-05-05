# Microservice itself that upon request, sends an encouraging message to the client

import datetime
import zmq
import json

context = zmq.Context()
socket = context.socket(zmq.REP)
socket.bind("tcp://127.0.0.1:5555")

while True:
    message = socket.recv().decode()
    print(f'Recieved client message: {message}')
    if len(message) > 0:
        if message == 'Q':
            print('Client has requested to close connection!')
            break
        if message != 'MSG':
            socket.send_string('ERR')

        randInt = datetime.datetime.now()
        randInt = str(((randInt.second) * (randInt.microsecond)) % 100)
        with open('encouragement.json', 'r', encoding='utf-8') as file:
            data = json.load(file)
            friendlyMessage = data[randInt]
            file.close()
        print(f'Sending response: {friendlyMessage}')
        socket.send_string(friendlyMessage)

context.destroy()
