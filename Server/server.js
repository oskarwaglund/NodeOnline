const dgram = require('dgram');
const gameloop = require('./lib/gameloop');
const game = require('./lib/game');

const localSocket = dgram.createSocket('udp4');
const remoteSocket = dgram.createSocket('udp4');

var frameCount = 0;

const CONNECT = 0;
const LEAVE = 1;
const INPUT = 2;

const UPDATE_COLOR = 5;

var connections = {};

localSocket.on('error', (err) => {
  console.log(`server error:\n${err.stack}`);
  server.close();
});

function bin2String(array) {
  var result = "";
  for (var i = 0; i < array.length; i++) {
    result += String.fromCharCode(array[i], 2);
  }
  return result;
}

var onSocketMessage = function(msg, rinfo, socket){
  switch(msg[0]){
    case CONNECT: 
      let id = game.addPlayer(bin2String(msg.slice(1)));
      console.log("Player " + msg.slice(1) + " connected with id " + id + "!");
      let reply = new Uint8Array([CONNECT, id]);
      socket.send(reply, rinfo.port, rinfo.address);
      connections[id] = {
        addr: rinfo.address,
        port: rinfo.port
      }
      break;
    case INPUT:
      game.addInput(msg);
      break;
    case UPDATE_COLOR:
      game.updateColor(msg);
      break;
  }
}

var onListening = function(socket){
  const address = socket.address();
  console.log(`server listening ${address.address}:${address.port}`);
}

localSocket.on('message', (msg, rinfo) => {
  onSocketMessage(msg, rinfo, localSocket);
});

remoteSocket.on('message', (msg, rinfo) => {
  onSocketMessage(msg, rinfo, remoteSocket);
});

localSocket.on('listening', () => {
  onListening(localSocket);
});

remoteSocket.on('listening', () => {
  onListening(remoteSocket);
});

localSocket.bind({
    address: 'localhost',
    port: 12345,
    exclusive: true
});

remoteSocket.bind({
  address: '192.168.0.7',
  port: 12345,
  exclusive: true
});

function broadCastGameState(){
  var state = game.getState();
  if(state.length > 1)
    for(var id in connections)
    {
      if(connections.hasOwnProperty(id))
        remoteSocket.send(state, connections[id].port, connections[id].addr);
    } 
}

function broadCastPlayerData(){
  var playerData = game.getPlayerData();
  for(var id in connections)
  {
    if(connections.hasOwnProperty(id))
      remoteSocket.send(playerData, connections[id].port, connections[id].addr);
  }
}

gameloop.setGameLoop(function(delta) {
	// `delta` is the delta time from the last frame
  frameCount++;
  game.updateGame();
  broadCastGameState();
  if(frameCount % 40 == 0){
    broadCastPlayerData();
  }

}, 25);