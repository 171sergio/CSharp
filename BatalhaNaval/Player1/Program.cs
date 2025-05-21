// See https://aka.ms/new-console-template for more information
using BattleshipLib;

var game = new Game(isServer: true);
game.Start(port: 5000);
