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

            passw = None
            if len(split) == 3:
                passw = split[2]

            # add new user
            if op == "ADD":
                b = registeredAccountExists(user, passw)
                if b == "USER_UNREGISTERED_NO_PW":
                    print("adding account")
                    a = addAccount(user, passw)
                    print("account added")
                    socket.send_string(str(a))
                else:
                    # account exists, cannot overwrite
                    socket.send_string(str(b))

            # verify correctness of PW / presence in credential set
            if op == "VERIFY":
                b = registeredAccountExists(user, passw)
                socket.send_string(str(b))

            print("response sent!")

        except Exception as ex:
            print(ex)
            continue


def show_exception_and_exit(exc_type, exc_value, tb):
    import traceback
    traceback.print_exception(exc_type, exc_value, tb)
    input("Press key to exit.")
    sys.exit(-1)


sys.excepthook = show_exception_and_exit


def registeredAccountExists(user, passw):
    # Read JSON file
    with open('accounts.json') as data_file:
        json_data = json.load(data_file)

        print(f"JSON contents: {json_data}")
        u = user in json_data
        p = json_data[user]["password"] == passw

        #  user is in file and no password provided (presence check)
        if (None is passw):
            if u is True:
                return "USER_REGISTERED_NO_PW"
            else:
                return "USER_UNREGISTERED_NO_PW"

        # user in file and password provided (validation)
        else:
            print("here")
            if u is True and p is True:
                print("passw is not none check a")
                return "USER_REGISTERED_GOOD_PW"
            else:
                if u is True and p is False:
                    print("passw is not none check b")
                    return "USER_REGISTERED_BAD_PW"


def addAccount(user, passw):
    print("in add method")
    user = {}
    user[user] = {
        "password": passw,
        "personal_data": {
            "entry1": "",
            "entry2": "",
            "entry3": ""
        }
    }

    print("account data created")

    # Write JSON file
    with io.open('data.json', 'w', encoding='utf8') as outfile:
        json.dump(user, outfile, indent=4, sort_keys=True,
                  separators=(',', ':'),
                  ensure_ascii=False)
    print("account data written to JSON")
    return "USER_ADDED_OK"


if __name__ == '__main__':
    sys.exit(main())
