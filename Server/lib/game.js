var idCounter = 0;
var players = {};
var numberOfPlayers = 0;
var inputs = [];
var bullets = [];

const STATE         = 0x10;
const PLAYER_DATA   = 0x11;

module.exports.addPlayer = function (_name){
    var id = idCounter++;

    var player = {
        name:_name,
        x:100,
        y:100,
        color:{
            r:0,
            g:0,
            b:100
        },
        health:100
    };

    players[id] = player;
    numberOfPlayers++;

    return id;
}

module.exports.addInput = function(_input){
    inputs.push(_input);
}

module.exports.updateColor = function(_msg){
    var id = _msg[1];
    players[id].color.r = _msg[2];
    players[id].color.g = _msg[3];
    players[id].color.b = _msg[4];
}

module.exports.updateGame = function(){
    for(var i = 0; i < inputs.length; i++){
        let id = inputs[i][1];
        let mask = inputs[i][2];
        
        if((mask & 1) > 0 && players[id].y > 0)
            players[id].y--;
        if((mask & 2) > 0 && players[id].x > 0)
            players[id].x--;
        if((mask & 4) > 0 && players[id].y < 640)
            players[id].y++;
        if((mask & 8) > 0 && players[id].x < 800)
            players[id].x++;

        if(inputs[i].length > 3){
            var X = player[id].x;
            var Y = player[id].y;
            
            var clickX = (input[3] << 8) + input[4];
            var clickY = (input[5] << 8) + input[6]; 
            
            var dX = clickX - X;
            var dY = clickY - Y;
            
            var length = Math.sqrt(dX*dX + dY*dY)

            dx /= length;
            dY /= length;

            var bullet = {
                id: idCounter++,
                playerId: id,
                x: X,
                y: Y,
                dx: dX,
                dy: dY
            }

            bullets.push(bullet);
        }
    }
    inputs = [];
}

module.exports.getState = function(){
    var bytesPerPlayer = 5;
    var bytesPerBullet = 5;

    var stateLength = 1+numberOfPlayers*bytesPerPlayer + (bullets.length > 0 ? 2 + bullets.length*bytesPerBullet : 0)

    var state = new Uint8Array(stateLength);
    var i = 1;

    state[0] = STATE;

    for(var playerId in players){
        if(players.hasOwnProperty(playerId)){
            state[i + 0] = playerId;
            state[i + 1] = players[playerId].x >> 8;
            state[i + 2] = players[playerId].x;
            state[i + 3] = players[playerId].y >> 8;
            state[i + 4] = players[playerId].y;

            i += bytesPerPlayer;
        }        
    }

    if(bullets.length > 0){
        state[i+0] = 0xFF;
        state[i+1] = 0xFF;

        i += 2;

        for(var bullet in bullets){
            state[i+0] = bullet.id;
            state[i+1] = bullet.x >> 8;
            state[i+2] = bullet.x;
            state[i+3] = bullet.y >> 8;
            state[i+4] = bullet.y;

            i += bytesPerBullet;
        }
    }

    return state;
}

module.exports.getPlayerData = function(){
    //ID + Name + color + health (21)
    var bytesPerPlayer = 1 + 16 + 3 + 1;

    var data = new Uint8Array(1+numberOfPlayers*bytesPerPlayer);
    var i = 0;

    data[0] = PLAYER_DATA;

    for(var prop in players){
        if(players.hasOwnProperty(prop)){
            data[1 + i*bytesPerPlayer] = prop;
            
            for(var j = 0; j < Math.min(players[prop].name.length, 16); j++){
                data[1 + i*bytesPerPlayer + 1 + j] = players[prop].name.charCodeAt(j);
            }
            for(var j = Math.min(players[prop].name.length, 16); j < 16; j++){
                data[1 + i*bytesPerPlayer + 1 + j] = 0;
            }
            
            data[1 + i*bytesPerPlayer + 17] = players[prop].color.r;
            data[1 + i*bytesPerPlayer + 18] = players[prop].color.g;
            data[1 + i*bytesPerPlayer + 19] = players[prop].color.b;

            data[1 + i*bytesPerPlayer + 20] = players[prop].health;

            i++;
        }
    }
    return data;
}