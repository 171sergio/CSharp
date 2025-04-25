using System;
using System.Threading;

class Program
{
    static void Main(string[] args)
    {
        int numFilosofos = 5;
        Filosofo[] filosofos = new Filosofo[numFilosofos];
        SemaphoreSlim[] garfos = new SemaphoreSlim[numFilosofos];

        // Inicializa os garfos (um SemaphoreSlim para cada garfo)
        for (int i = 0; i < numFilosofos; i++)
        {
            garfos[i] = new SemaphoreSlim(1, 1);
        }

        // Cria e inicia os filósofos
        for (int i = 0; i < numFilosofos; i++)
        {
            filosofos[i] = new Filosofo(i, garfos[i], garfos[(i + 1) % numFilosofos]);
            new Thread(filosofos[i].Viver).Start();
        }

        Console.ReadLine(); // Mantém o programa em execução
    }
}

class Filosofo
{
    private int id;
    private SemaphoreSlim garfoEsquerdo;
    private SemaphoreSlim garfoDireito;
    private Random random;

    public Filosofo(int id, SemaphoreSlim garfoEsquerdo, SemaphoreSlim garfoDireito)
    {
        this.id = id;
        this.garfoEsquerdo = garfoEsquerdo;
        this.garfoDireito = garfoDireito;
        this.random = new Random();
    }

    public void Viver()
    {
        while (true)
        {
            Pensar();
            Comer();
        }
    }

    private void Pensar()
    {
        Console.WriteLine($"Filósofo {id} está pensando.");
        Thread.Sleep(random.Next(1000, 3000)); // Simula o tempo pensando
    }

    private void Comer()
    {
        Console.WriteLine($"Filósofo {id} está tentando pegar os garfos.");

        // Estratégia para evitar deadlock: pegar os garfos em ordem
        SemaphoreSlim primeiroGarfo = id % 2 == 0 ? garfoEsquerdo : garfoDireito;
        SemaphoreSlim segundoGarfo = id % 2 == 0 ? garfoDireito : garfoEsquerdo;

        // Tenta pegar o primeiro garfo
        primeiroGarfo.Wait();
        Console.WriteLine($"Filósofo {id} pegou o primeiro garfo.");

        // Tenta pegar o segundo garfo
        if (segundoGarfo.Wait(1000)) // Timeout para evitar deadlock
        {
            Console.WriteLine($"Filósofo {id} pegou o segundo garfo e está comendo.");
            Thread.Sleep(random.Next(1000, 2000)); // Simula o tempo comendo
            segundoGarfo.Release();
            Console.WriteLine($"Filósofo {id} liberou o segundo garfo.");
        }
        else
        {
            Console.WriteLine($"Filósofo {id} não conseguiu pegar o segundo garfo e desistiu.");
        }

        // Libera o primeiro garfo
        primeiroGarfo.Release();
        Console.WriteLine($"Filósofo {id} liberou o primeiro garfo.");
    }
}