using System;

namespace BattleshipLib
{
    public class Game
    {
        private readonly Board board;
        private readonly NetworkManager network;
        private readonly bool isServer;
        private const int NUM_SHIPS = 10;

        public Game(bool isServer)
        {
            this.isServer = isServer;
            board = new Board();
            network = new NetworkManager();
        }

        public void Start(string host = "127.0.0.1", int port = 5000)
        {
            try
            {
                if (isServer)
                {
                    SetupServer(port);
                    PlayAsServer();
                }
                else
                {
                    SetupClient(host, port);
                    PlayAsClient();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro: {ex.Message}");
            }
            finally
            {
                network.Dispose();
            }
        }

        private void SetupServer(int port)
        {
            Console.WriteLine("=== BATALHA NAVAL - SERVIDOR (Player 1) ===\n");
            Console.WriteLine("Escolha o modo de posicionamento dos navios:");
            Console.WriteLine("1. Posicionamento Aleatório");
            Console.WriteLine("2. Posicionamento Manual");

            while (true)
            {
                if (int.TryParse(Console.ReadLine(), out int choice))
                {
                    if (choice == 1)
                    {
                        board.PlaceShipsRandomly(NUM_SHIPS);
                        break;
                    }
                    else if (choice == 2)
                    {
                        PlaceShipsManually();
                        break;
                    }
                }
                Console.WriteLine("Opção inválida. Digite 1 ou 2.");
            }

            Console.WriteLine("\nTabuleiro inicial:");
            board.Print(true);
            network.StartServer(port);
        }

        private void SetupClient(string host, int port)
        {
            Console.WriteLine("=== BATALHA NAVAL - CLIENTE (Player 2) ===\n");
            network.ConnectToServer(host, port);
        }

        private void PlaceShipsManually()
        {
            Console.WriteLine($"\nDigite {NUM_SHIPS} coordenadas para posicionar os navios (ex: A5, J0):");
            int placed = 0;
            while (placed < NUM_SHIPS)
            {
                Console.Write($"Navio {placed + 1}/{NUM_SHIPS}: ");
                string? input = Console.ReadLine();
                if (string.IsNullOrEmpty(input))
                    continue;

                if (board.PlaceShipManually(input))
                {
                    placed++;
                    board.Print(true);
                }
                else
                {
                    Console.WriteLine("Posição inválida ou já ocupada. Tente novamente.");
                }
            }
        }

        private void PlayAsServer()
        {
            while (true)
            {
                string attack = network.Receive();
                Console.WriteLine($"\nRecebido ataque em: {attack}");

                try
                {
                    var (row, col) = Board.ParseCoordinate(attack);
                    string response;

                    if (board.IsShip(row, col))
                    {
                        board.MarkHit(row, col);
                        response = board.AreAllShipsSunk() ? "WIN" : "HIT";
                    }
                    else
                    {
                        board.MarkMiss(row, col);
                        response = "MISS";
                    }

                    Console.WriteLine("Tabuleiro atual:");
                    board.Print(true);
                    network.Send(response);

                    if (response == "WIN")
                    {
                        Console.WriteLine("\nTodos os navios foram afundados! Fim de jogo.");
                        break;
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Coordenada inválida recebida.");
                }
            }
        }

        private void PlayAsClient()
        {
            while (true)
            {
                board.Print(true);
                Console.Write("\nDigite uma coordenada de ataque (ex: A5): ");
                string? attack = Console.ReadLine();

                if (string.IsNullOrEmpty(attack))
                    continue;

                try
                {
                    Board.ParseCoordinate(attack);
                    network.Send(attack);

                    string response = network.Receive();
                    var (row, col) = Board.ParseCoordinate(attack);

                    switch (response)
                    {
                        case "HIT":
                            Console.WriteLine("ACERTOU!");
                            board.MarkHit(row, col);
                            break;
                        case "MISS":
                            Console.WriteLine("ÁGUA!");
                            board.MarkMiss(row, col);
                            break;
                        case "WIN":
                            Console.WriteLine("PARABÉNS! Você venceu!");
                            board.MarkHit(row, col);
                            board.Print(true);
                            return;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro: {ex.Message}");
                }
            }
        }
    }
} 