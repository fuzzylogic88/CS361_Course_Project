# Microservice which delivers configured reminders to the main window
# Author: Daniel Green (greend5@oregonstate.edu)

from os import system
import zmq
import json
import sys


def main():
    system("title Reminder Service")

    context = zmq.Context()
    socket = context.socket(zmq.REP)
    socket.bind("tcp://127.0.0.1:5558")

    while True:
        print("waiting for reminder request...")
        message = socket.recv().decode()
        print(f"Recieved client message: {message}")
        if len(message) > 0:
            if message == "Q":
                print("Client has requested to close connection!")
                
        try:
            split = message.split(",")
            op = split[0]
            user = split[1]

            reminder = None
            r_type = None
            date = None
            if len(split) == 5:
                reminder = split[2]
                r_type = split[3]
                date = split[4]

            # add new reminder
            if op == "ADD":
                a = addReminder(user, reminder, r_type, date)
                print("reminder added")
                socket.send_string(str(a))

            if op == "READ":
                dat = getUserData(user)
                socket.send_string(str(dat))

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


def getUserData(user):
    # Read JSON file
    with open("accounts.json", "r") as data_file:
        json_data = json.load(data_file)
        reminders = json_data.get(user, {}).get("personal_reminders", {})
        return reminders


def addReminder(user, reminder, r_type, date):
    with open("accounts.json", "r") as data_file:
        json_data = json.load(data_file)

    # determine first empty reminder slot
    reminders = json_data.get(user, {}).get("personal_reminders", {})
    empty_slot = next((k for k, v in reminders.items() if v == ""), None)

    # update the personal entry section to contain a new reminder
    if user in json_data and "personal_reminders" in json_data[user]:
        json_data[user]["personal_reminders"][empty_slot] = (
            f"{date},"
            f"{reminder},"
            f"{r_type}")

    # Write JSON file
    with open("accounts.json", "w") as file:
        json.dump(json_data, file, indent=2)

    return "REMINDER_ADDED_OK"


if __name__ == "__main__":
    sys.exit(main())
