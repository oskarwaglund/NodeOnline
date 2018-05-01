var nextPlayerId = 0;
var players = {};
var numberOfPlayers = 0;
var inputs = [];

module.exports.addPlayer = function (NAME){
    var ID = nextPlayerId++;

    var player = {
        name:NAME,
        x:100,
        y:100
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
        if((mask & 4) > 0 && players[id].y < 800)
            players[id].y++;
        if((mask & 8) > 0 && players[id].x < 640)
            players[id].x++;
    }
    inputs = [];
}

module.exports.getState = function(){
    var bytesPerPlayer = 5;
    var state = new Uint8Array(numberOfPlayers*bytesPerPlayer);
    var i = 0;
    for(var prop in players){
        if(players.hasOwnProperty(prop)){
            state[i*bytesPerPlayer + 0] = prop;
            state[i*bytesPerPlayer + 1] = (players[prop].x & 0xFF00) >> 8;
            state[i*bytesPerPlayer + 2] = players[prop].x & 0xFF;
            state[i*bytesPerPlayer + 3] = (players[prop].y & 0xFF00) >> 8;
            state[i*bytesPerPlayer + 4] = players[prop].y & 0xFF;

            i++;
        }
        
    }
    
    return state;
}