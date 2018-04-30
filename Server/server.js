const dgram = require('dgram');
const socket = dgram.createSocket('udp4');
const gameloop = require('./lib/gameloop');
const game = require('./lib/game');

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

socket.on('error', (err) => {
  console.log(`server error:\n${err.stack}`);
  server.close();
});

socket.on('message', (msg, rinfo) => {
  console.log(`server got: ${msg} from ${rinfo.address}:${rinfo.port}` + msg[0]);
  switch(msg[0]){
    case CONNECT: 
      let id = game.addPlayer(msg.slice(1));
      console.log("Player " + msg.slice(1) + " connected with id " + id + "!");
      let reply = new Uint8Array([CONNECT, id]);
      socket.send(reply, rinfo.port, rinfo.address);
      break;
    case INPUT:
      console.log("Player input");
      break;
  }
    
});

socket.on('listening', () => {
  const address = socket.address();
  console.log(`server listening ${address.address}:${address.port}`);
});

const id = gameloop.setGameLoop(function(delta) {
	// `delta` is the delta time from the last frame
  frameCount++;
  game.updateGame();
  mcSocket.send(game.getState(), 6000, mcIP);

}, 25);

socket.bind({
    address: 'localhost',
    port: 12345,
    exclusive: true
  });
// server listening 127.0.0.1:12345