var nextPlayerId = 0;
var players = {};
var numberOfPlayers = 0;
var inputs = [];

const STATE         = 0x10;
const PLAYER_DATA   = 0x11;

module.exports.addPlayer = function (NAME){
    var ID = nextPlayerId++;

    var player = {
        name:NAME,
        x:100,
        y:100,
        color:{
            r:0,
            g:0,
            b:100
        },
        health:100
    };

    players[ID] = player;
    numberOfPlayers++;

    return ID;
}

module.exports.addInput = function(INPUT){
    inputs.push(INPUT);
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
    }
    inputs = [];
}

module.exports.getState = function(){
    var bytesPerPlayer = 5;
    var state = new Uint8Array(1+numberOfPlayers*bytesPerPlayer);
    var i = 0;

    state[0] = STATE;

    for(var prop in players){
        if(players.hasOwnProperty(prop)){
            state[1 + (i*bytesPerPlayer) + 0] = prop;
            state[1 + (i*bytesPerPlayer) + 1] = (players[prop].x & 0xFF00) >> 8;
            state[1 + (i*bytesPerPlayer) + 2] = players[prop].x & 0xFF;
            state[1 + (i*bytesPerPlayer) + 3] = (players[prop].y & 0xFF00) >> 8;
            state[1 + (i*bytesPerPlayer) + 4] = players[prop].y & 0xFF;

            i++;
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