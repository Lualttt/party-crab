from aiohttp import web
import socketio
import binascii
import os

sio = socketio.AsyncServer(cors_allowed_origins="*", ping_interval=10, ping_timeout=3)
app = web.Application()
sio.attach(app)

parties = {}
users = {}

## DEV
@sio.event
async def status(sid, data):
    print(parties)
## DEV

####################
# PARTY MANAGEMENT #
####################

@sio.event
async def host(sid, data):
    print(f"(host) {sid}: {data}")

    data_keys = list(data.keys())

    # checks if they are already in a party
    if sid in users:
        print("^- in a party already")
        await sio.emit("host", {"successful": False, "data": {"error": "in a party already"}})
        return

    # checks if "party_name", "party_max", "party_public" are in data
    if "party_name" not in data_keys or "party_max" not in data_keys or "party_public" not in data_keys:
        print("^- wrong host format")
        await sio.emit("host", {"successful": False, "data": {"error": "wrong host format"}})
        return

    # checks if "party_name", "party_max", "party_public" have the right data type
    if not type(data["party_name"]) == str or not type(data["party_max"]) == int or not type(data["party_public"]) == bool:
        print("^- wrong host data type")
        await sio.emit("host", {"successful": False, "data": {"error": "wrong host data type"}})
        return

    party = {
        "party_name": data["party_name"],
        "party_max": data["party_max"],
        "party_count": 1,
        "party_public": data["party_public"],
        "party_host": sid
        }

    # generates unique id
    for _ in range(12):
        party_id = binascii.b2a_hex(os.urandom(3)).decode()
        if party_id not in parties:
            break
    else:
        await sio.emit("host", {"successful": False, "data": {"error": "couldn't create party"}})
        return

    # creates and joins party
    parties[party_id] = party
    users[sid] = party_id
    sio.enter_room(sid, party_id)
    await sio.emit("host", {"successful": True, "data": {"party_id": party_id}})
    await sio.emit("join", {"successful": True, "data": parties[party_id] | {"party_id": party_id}})


@sio.event
async def disband(sid, data):
    print(f"(disband) {sid}: {data}")

    # checks if "party_id" is in data
    if "party_id" not in data:
        print("^- wrong disband format")
        await sio.emit("disband", {"successful": False, "data": {"error": "wrong disband format"}})
        return

    # checks if "party_id" is an int
    if not type(data["party_id"]) == str:
        print("^- wrong disband data type")
        await sio.emit("disband", {"successful": False, "data": {"error": "wrong disband data type"}})
        return

    # checks if they are in a party
    if sid not in users:
        print("^- not in a party")
        await sio.emit("disband", {"successful": False, "data": {"error": "not in a party"}})
        return

    # checks if party exists
    if not data["party_id"] in parties:
        print("^- couldn't find party")
        await sio.emit("disband", {"successful": False, "data": {"error": "couldn't find party"}})
        return

    # checks if you are the party host
    if not sid == parties[data["party_id"]]["party_host"]:
        print("^- you aren't the party host")
        await sio.emit("disband", {"successful": False, "data": {"error": "you aren't the party host"}})
        return

    parties.pop(data["party_id"])
    await sio.emit("leave", {"successful": True, "data": {}}, room=data["party_id"])
    await sio.close_room(data["party_id"])
    await sio.emit("disband", {"successful": True, "data": {}})


@sio.event
async def join(sid, data):
    print(f"(host) {sid}: {data}")

    # checks if "party_id" is in data
    if "party_id" not in data:
        print("^- wrong join format")
        await sio.emit("join", {"successful": False, "data": {"error": "wrong join format"}})
        return

    # checks if "party_id" is an int
    if not type(data["party_id"]) == str:
        print("^- wrong join data type")
        await sio.emit("join", {"successful": False, "data": {"error": "wrong join data type"}})
        return

    # checks if they are already in a party
    if sid in users:
        print("^- in a party already")
        await sio.emit("join", {"successful": False, "data": {"error": "in a party already"}})
        return

    # checks if party exists
    if not data["party_id"] in parties:
        print("^- couldn't find party")
        await sio.emit("join", {"successful": False, "data": {"error": "couldn't find party"}})
        return

    if sid in users:
        return

    # joins party
    parties[data["party_id"]]["party_count"] += 1
    users[sid] = data["party_id"]
    sio.enter_room(sid, data["party_id"])
    await sio.emit("join", {"successful": True, "data": parties[data["party_id"]]})


@sio.event
async def leave(sid, data):
    print(f"(leave) {sid}: {data}")

    # checks if "party_id" is in data
    if "party_id" not in data:
        print("^- wrong leave format")
        await sio.emit("leave", {"successful": False, "data": {"error": "wrong leave format"}})
        return

    # checks if "party_id" is an int
    if not type(data["party_id"]) == str:
        print("^- wrong leave data type")
        await sio.emit("leave", {"successful": False, "data": {"error": "wrong leave data type"}})
        return

    # checks if they are in a party
    if sid not in users:
        print("^- not in a party")
        await sio.emit("leave", {"successful": False, "data": {"error": "not in a party"}})
        return

    # checks if party exists
    if not data["party_id"] in parties:
        print("^- couldn't find party")
        await sio.emit("leave", {"successful": False, "data": {"error": "couldn't find party"}})
        return

    if sid not in users:
        return

    # leaves party
    parties[data["party_id"]]["party_count"] -= 1
    users.pop(sid)
    sio.leave_room(sid, data["party_id"])
    await sio.emit("leave", {"successful": True, "data": {}})


@sio.event
async def promote(sid, data):
    print(f"(promote) {sid}: {data}")

    # checks if "party_id" and "new_host" are in data
    if "party_id" not in data or "new_host" not in data:
        print("^- wrong promote format")
        await sio.emit("promote", {"successful": False, "data": {"error": "wrong promote format"}})
        return

    # checks if "party_id" and "new_host" are a str's
    if type(data["party_id"]) != str and type(data["new_host"]) != str:
        print("^- wrong promote data type")
        await sio.emit("promote", {"successful": False, "data": {"error": "wrong promote data type"}})
        return

    for user in users:
        if user.endswith(data["new_host"]):
            new_host = user
            break
    else:
        print("^- couldn't find user")
        await sio.emit("promote", {"successful": False, "data": {"error": "couldn't find user"}})
        return

    # checks if user is in the party
    if sid not in users:
        print("^- not in a party")
        await sio.emit("promote", {"successful": False, "data": {"error": "not in a party"}})
        return

    # checks if party exists
    if not data["party_id"] in parties:
        print("^- couldn't find party")
        await sio.emit("promote", {"successful": False, "data": {"error": "couldn't find party"}})
        return

    if users[new_host] != users[sid]:
        print("^- not in the same party")
        await sio.emit("promote", {"successful": False, "data": {"error": "not in the same party"}})
        return

    # checks if you are the party host
    if not sid == parties[data["party_id"]]["party_host"]:
        print("^- you aren't the party host")
        await sio.emit("promote", {"successful": False, "data": {"error": "you aren't the party host"}})
        return

    parties[data["party_id"]]["party_host"] = new_host
    await sio.emit("leave", {"successful": True, "data": {}})

###################
# ROOM MANAGEMENT #
###################

@sio.event
async def disbanded(sid, data):
    username = "somebody"

    if "username" in data:
        username = data["username"]

    if "party_id" not in data:
        return

    if sid not in users:
        return

    if parties[data["party_id"]]["party_host"] != sid:
        return

    await sio.emit("disbanded", {"message": f"{username} disbanded the party"}, room=users[sid])


@sio.event
async def joined(sid, data):
    username = "somebody"

    if "username" in data:
        username = data["username"]

    if sid not in users:
        return

    await sio.emit("joined", {"message": f"{username} joined the party"}, room=users[sid])


@sio.event
async def left(sid, data):
    username = "somebody"

    if "username" in data:
        username = data["username"]

    if sid not in users:
        return

    await sio.emit("left", {"message": f"{username} left the party"}, room=users[sid])


@sio.event
async def message(sid, data):
    username = "somebody"

    if "username" in data:
        username = data["username"]

    if "message" not in data:
        return

    if sid not in users:
        return

    await sio.emit("message", {"username": username,"message": data["message"]}, room=users[sid])


@sio.event
async def promoted(sid, data):
    old_host = "somebody"
    new_host = "somebody"

    if "old_host" in data and "new_host" in data:
        old_host = data["old_host"]
        new_host = data["new_host"]

    if sid not in users:
        return

    await sio.emit("promoted", {"message": f"{old_host} promoted {new_host}"}, room=users[sid])

#####

@sio.event
def connect(sid, _environ):
    print(f"(connect) {sid}")


@sio.event
def disconnect(sid):
    print(f"(disconnect) {sid}")
    
    # checks if user/party still exists and destroys them if so
    if sid in users:
        parties[users[sid]]["party_count"] -= 1

        if parties[users[sid]]["party_host"] == sid:
            sio.emit("disbanded", {"message": f"somebody disbanded the party"}, room=users[sid])
            parties.pop(users[sid])

        sio.emit("left", {"message": "somebody left the party"}, room=users[sid])
        users.pop(sid)


@sio.on("*")
async def catch_all(event, sid, data):
    if data:
        print(f"({event}) {sid}: {data}")
    else:
        print(f"({event}) {sid}")
        

if __name__ == "__main__":
    web.run_app(app)


# POSSIBLE: while leaving promote someone else instead of disbanding the party
# POSSIBLE: save crab game username so we never have the "somebody" problem