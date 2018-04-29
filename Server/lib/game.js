var nextPlayerId = 0;
var players = [];
var input = [];
module.exports.addPlayer = function (NAME){
    var ID = nextPlayerId++;

    var player = {
        id:ID,
        name:NAME,
        x:100,
        y:100
    };

    players.push(player);

    return ID;
}

module.exports.addInput = function(INPUT){
    input.push(INPUT);
}

module.exports.updateGame = function(){

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