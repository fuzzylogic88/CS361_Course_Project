# Microservice facilitating user login/authentication
# Author: Daniel Green (greend5@oregonstate.edu)

from os import system
import io
import zmq
import json
import sys


def main():
    system("title Login Service")

    context = zmq.Context()
    socket = context.socket(zmq.REP)
    socket.bind("tcp://127.0.0.1:5556")

    while True:
        print("waiting for login request...")
        message = socket.recv().decode()
        print(f'Recieved client message: {message}')
        if len(message) > 0:
            if message == 'Q':
                print('Client has requested to close connection!')
        try:
            split = message.split(',')
            op = split[0]
            user = split[1]
            passw = split[2]

            # add new user
            if op == "ADD":
                b = accountExists(user, passw)
                if b is False:
                    a = addAccount(user, passw)
                    socket.send(a)
                else:
                    # account exists, cannot overwrite
                    socket.send(b)

            # verify correctness of PW / presence in credential set
            if op == "VERIFY":
                b = accountExists(user, passw)
                print(b)
                socket.send(b)

            print("response sent!")

        except Exception as ex:
            print(ex)
            continue


def accountExists(user, passw):
    # Read JSON file
    with open('accounts.json') as data_file:
        json_data = json.load(data_file)

        if (None is not passw):
            # password provided, check validity
            return user in json_data and passw in json_data

        else:
            # no password, check if username is registered
            return user in json_data


def addAccount(user, passw):
    user = {}
    user[user] = {
        "password": passw,
        "personal_data": {
            "entry1": "",
            "entry2": "",
            "entry3": ""
        }
    }

    # Write JSON file
    with io.open('data.json', 'w', encoding='utf8') as outfile:
        json.dump(user, outfile, indent=4, sort_keys=True,
                  separators=(',', ':'),
                  ensure_ascii=False)


if __name__ == '__main__':
    sys.exit(main())
