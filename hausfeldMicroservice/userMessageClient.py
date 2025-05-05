# File to test out that the microservice (server.py) works as expected.
#  Will have to perform similar activities in your C# file as outlined here when officially implementing.
import zmq

# Setup connection
context = zmq.Context()
print("Client attempting to connect to server...")
socket = context.socket(zmq.REQ)
socket.connect("tcp://localhost:5555")

# Send request
print(f"Sending a request...")
socket.send_string('MSG')

# Recieve message
message = socket.recv()
print(f"Server sent back: {message.decode()}")

# Cancel request
socket.send_string('Q')
