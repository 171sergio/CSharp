using BattleshipLib;

var game = new Game(isServer: false);
game.Start(host: "127.0.0.1", port: 5000);
