var nextPlayerId = 0;
var players = {};
var inputs = [];

module.exports.addPlayer = function (NAME){
    var ID = nextPlayerId++;

    var player = {
        name:NAME,
        x:100,
        y:100
    };

    players.ID = player;

    return ID;
}

module.exports.addInput = function(INPUT){
    inputs.push(INPUT);
}

module.exports.updateGame = function(){
    for(input in inputs){
        let id = input[0];
        let mask = input[1];

        if(mask & 1)
            players.id.y--;
        if(mask & 2)
            players.id.x--;
        if(mask & 4)
            players.id.y++;
        if(mask & 8)
            players.id.x++;
    }
    inputs = [];
}

module.exports.getState = function(){
    var bytesPerPlayer = 3;
    var state = new Uint8Array(players.length*bytesPerPlayer);
    for(let i = 0; i < players.length; i++){
        state[i*bytesPerPlayer + 0] = players[i].id;
        state[i*bytesPerPlayer + 1] = players[i].x;
        state[i*bytesPerPlayer + 2] = players[i].y;
    }

    return state;
}