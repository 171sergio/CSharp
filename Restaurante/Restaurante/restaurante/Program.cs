using System;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Text;

BlockingCollection<(int pedido, int prato)> pedidos = new BlockingCollection<(int pedido, int prato)>();
object lockConsole = new();
object lockEstoque = new();

int estoqueArroz = 0, estoqueCarne = 0, estoqueMacarrao = 0, estoqueMolho = 0;

void ConsoleLock(string msg, ConsoleColor color)
{
    lock (lockConsole)
    {
        var aux = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.WriteLine(msg);
        Console.ForegroundColor = aux;
    }
}

int pedido = 0;
void Garcom()
{
    var rnd = new Random();
    var id = Thread.CurrentThread.ManagedThreadId;
    ConsoleLock($"[Garçom {id}] Estou pronto!!!", ConsoleColor.Blue);
    while (true)
    {
        int tempo = rnd.Next(1000, 10000);
        int prato = rnd.Next(1, 4);
        int p = Interlocked.Increment(ref pedido);

        Thread.Sleep(tempo);

        ConsoleLock($"[Garçom {id}] Envio de Pedido {p}: Prato {prato}", ConsoleColor.Blue);
        pedidos.Add((p, prato));
    }
}

void ProduzirIngrediente(string ingrediente, ref int estoque, int quantidade)
{
    lock (lockEstoque)
    {
        ConsoleLock($"[Chef] Iniciando produção de {ingrediente}", ConsoleColor.Green);
        Thread.Sleep(2000); // 2 segundos por produção
        estoque += quantidade;
        ConsoleLock($"[Chef] Finalizou produção de {ingrediente}. Estoque atualizado: {estoque} unidades", ConsoleColor.Green);
    }
}

void Chef()
{
    ConsoleLock("[Chef] Estou pronto!!!", ConsoleColor.Red);
    foreach (var item in pedidos.GetConsumingEnumerable())
    {
        var (pedido, prato) = item;
        ConsoleLock($"[Chef] Inicio da Preparação do Pedido {pedido}", ConsoleColor.Red);

        // Verificar e produzir ingredientes necessários
        if (prato == 1) // Prato Executivo: 1 arroz, 1 carne
        {
            if (estoqueArroz < 1) ProduzirIngrediente("Arroz", ref estoqueArroz, 3);
            if (estoqueCarne < 1) ProduzirIngrediente("Carne", ref estoqueCarne, 2);

            lock (lockEstoque)
            {
                estoqueArroz--;
                estoqueCarne--;
            }
        }
        else if (prato == 2) // Prato Italiano: 1 macarrão, 1 molho
        {
            if (estoqueMacarrao < 1) ProduzirIngrediente("Macarrão", ref estoqueMacarrao, 4);
            if (estoqueMolho < 1) ProduzirIngrediente("Molho", ref estoqueMolho, 2);

            lock (lockEstoque)
            {
                estoqueMacarrao--;
                estoqueMolho--;
            }
        }
        else if (prato == 3) // Prato Especial: 1 arroz, 1 carne, 1 molho
        {
            if (estoqueArroz < 1) ProduzirIngrediente("Arroz", ref estoqueArroz, 3);
            if (estoqueCarne < 1) ProduzirIngrediente("Carne", ref estoqueCarne, 2);
            if (estoqueMolho < 1) ProduzirIngrediente("Molho", ref estoqueMolho, 2);

            lock (lockEstoque)
            {
                estoqueArroz--;
                estoqueCarne--;
                estoqueMolho--;
            }
        }

        // Montagem do prato
        Thread.Sleep(1000); // 1 segundo por porção usada
        ConsoleLock($"[Chef] Fim da Preparação do Pedido {pedido}", ConsoleColor.Red);
    }
}

// Iniciar threads
var garcomTask = Task.Run(() => Garcom());
var chefTask = Task.Run(() => Chef());

Task.WaitAll(garcomTask, chefTask);