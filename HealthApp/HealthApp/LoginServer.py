# Microservice facilitating user login/authentication
# Author: Daniel Green (greend5@oregonstate.edu)

from os import system
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
                sys.exit(0)
        try:
            split = message.split(',')
            op = split[0]
            user = split[1]

            passw = None
            if len(split) == 3:
                passw = split[2]

            # add new user
            if op == "ADD":
                print("in add case")
                b = registeredAccountExists(user, passw)
                print(f"from add account check: {b}")
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
     
        #print(f"JSON contents: {json_data}")
        
        user_data = json_data.get(user)

        #  user is in file and no password provided (presence check)
        if (passw is None):
            if user_data:
                return "USER_REGISTERED_NO_PW"
            else:
                return "USER_UNREGISTERED_NO_PW"

        # user in file and password provided (validation)
        else:
            if user_data:
                user_pw = user_data.get("password")
                if user_pw == passw:
                    return "USER_REGISTERED_GOOD_PW"
                else:
                    return "USER_REGISTERED_BAD_PW"
            else:
                return "USER_UNREGISTERED_NO_PW"


def addAccount(user, passw):

    with open('accounts.json') as data_file:
        json_data = json.load(data_file)

    json_data[user] = {
        "password": passw,
        "personal_reminders": {
            "entry1": "",
            "entry2": "",
            "entry3": "",
            "entry4": "",
            "entry5": "",
            "entry6": "",
            "entry7": "",
            "entry8": "",
            "entry9": "",
            "entry10": "",
        }
    }

    print("account data created")

    # Write JSON file
    with open("accounts.json", "w") as file:
        json.dump(json_data, file, indent=2)

    print("account data written to JSON")
    return "USER_ADDED_OK"


if __name__ == '__main__':
    sys.exit(main())
