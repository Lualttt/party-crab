import socketio
from aiohttp import web

sio = socketio.AsyncServer(cors_allowed_origins="*")
app = web.Application()
sio.attach(app)


@sio.event
def connect(sid, _environ):
    print(f"(connect) {sid}")
    

@sio.event
def disconnect(sid):
    print(f"(disconnect) {sid}")
    

@sio.on("*")
async def catch_all(event, sid, data):
    if data:
        print(f"({event}) {sid}: {data}")
    else:
        print(f"({event}) {sid}")
        

if __name__ == "__main__":
    web.run_app(app)
