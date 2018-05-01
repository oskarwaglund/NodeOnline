const dgram = require('dgram');
const gameloop = require('./lib/gameloop');
const game = require('./lib/game');

const localSocket = dgram.createSocket('udp4');
const remoteSocket = dgram.createSocket('udp4');

var mcIP = '224.1.2.3';
var mcPort = 6000;
var mcSocket = dgram.createSocket("udp4"); 
mcSocket.bind(mcPort, function(){
  mcSocket.setBroadcast(true)
  mcSocket.setMulticastTTL(10);
  mcSocket.addMembership(mcIP);
});

var frameCount = 0;

const CONNECT = 0;
const LEAVE = 1;
const INPUT = 2;

localSocket.on('error', (err) => {
  console.log(`server error:\n${err.stack}`);
  server.close();
});

var onSocketMessage = function(msg, rinfo, socket){
  switch(msg[0]){
    case CONNECT: 
      let id = game.addPlayer(msg.slice(1));
      console.log("Player " + msg.slice(1) + " connected with id " + id + "!");
      let reply = new Uint8Array([CONNECT, id]);
      socket.send(reply, rinfo.port, rinfo.address);
      break;
    case INPUT:
      game.addInput(msg);
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

gameloop.setGameLoop(function(delta) {
	// `delta` is the delta time from the last frame
  frameCount++;
  game.updateGame();

  var state = game.getState();
  if(state.length > 0)
    mcSocket.send(state, 6000, mcIP);

}, 25);
// server listening 127.0.0.1:12345