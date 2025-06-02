# Microservice which delivers configured reminders to the main window
# Author: Daniel Green (greend5@oregonstate.edu)

from os import system
import datetime
import zmq
import json

system("title Reminder Service")

context = zmq.Context()
socket = context.socket(zmq.REP)
socket.bind("tcp://127.0.0.1:5558")

message = socket.recv().decode()
print(f'Recieved client message: {message}')
if len(message) > 0:
    if message == 'Q':
        print('Client has requested to close connection!')
    if message != 'MSG':
        socket.send_string('ERR')

    randInt = datetime.datetime.now()
    randInt = str(((randInt.second) * (randInt.microsecond)) % 2)
    with open('reminders.json', 'r', encoding='utf-8') as file:
        data = json.load(file)
        friendlyMessage = data[randInt]
        file.close()
    print(f'Sending response: {friendlyMessage}')
    socket.send_string(friendlyMessage)
context.destroy()
