import socketio
import threading
import asyncio
from colorama import Fore
import sys

sio = socketio.Client()
lock = threading.Lock()

username = "Lualt"
chat = True


@sio.event
def host(data):
    if data["successful"]:
        print(Fore.MAGENTA + f"created party; {data['data']['party_id']}" + Fore.RESET)
    else:
        print(Fore.RED + data["data"]["error"] + Fore.RESET)


@sio.event
def join(data):
    if data["successful"]:
        print(Fore.GREEN + f"joined '{data['data']['party_name']}'; {data['data']}", Fore.RESET)
        sio.emit("joined", {"username": username})
    else:
        print(Fore.RED + data["data"]["error"] + Fore.RESET)

###

@sio.event
def joined(data):
    print(Fore.MAGENTA + data["message"] + Fore.RESET)


@sio.event
def message(data):
    print(Fore.YELLOW + f"{data['username']}: {data['message']}" + Fore.RESET)


def command_handler(command):
    global chat

    if command[0] == "host":
        print(Fore.GREEN + "creating party" + Fore.RESET)
        sio.emit("host", {"party_name": f"{username}'s party", "party_max": 6, "party_public": False})

    if command[0] == "ac":
        chat = True
    if command[0] == "pc":
        chat = False


def main():
    while True:
        try:
            input()
            with lock:
                message = input("> ")

                if message[0] == "/":
                    command = message[1:].split(" ")
                    command_handler(command)
                if chat:
                    print(Fore.WHITE + f"{username}: {message}" + Fore.RESET)
                else:
                    sio.emit("message", {"username": username, "message": message})
        except KeyboardInterrupt:
            sys.exit()


if __name__ == "__main__":
    sio.connect("http://localhost:8080")
    main()
