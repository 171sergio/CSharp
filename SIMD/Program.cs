using System;
using System.Diagnostics;
using System.Numerics;
using System.Linq;

// Estrutura de personagem original (AoS - Array of Structs)
public struct Personagem
{
	public int Ataque;
	public int Defesa;
	public int ChanceCritico; // 0-100
	public int MultCritico;   // multiplicador de crítico (ex: 200 = 2x)
	public int Vida;
	public bool Vivo;
}

// Classe de simulação original (lenta)
public class SimuladorCombate
{
	private static Random gerador = new Random(42);
   
	public static int CalcularDano(Personagem atacante, Personagem defensor)
	{
    	if (!atacante.Vivo || !defensor.Vivo)
        	return 0;
           
    	// Dano base = Ataque - Defesa (mínimo 1)
    	int danoBase = Math.Max(1, atacante.Ataque - defensor.Defesa);
       
    	// Verificar crítico
    	bool ehCritico = gerador.Next(0, 100) < atacante.ChanceCritico;
       
    	if (ehCritico)
    	{
        	danoBase = (danoBase * atacante.MultCritico) / 100;
    	}
       
    	return danoBase;
	}
   
	public static long SimularRodadaCombate(Personagem[] atacantes, Personagem[] defensores)
	{
    	long danoTotal = 0;
       
    	for (int i = 0; i < atacantes.Length && i < defensores.Length; i++)
    	{
        	danoTotal += CalcularDano(atacantes[i], defensores[i]);
    	}
       
    	return danoTotal;
	}
   
	public static Personagem[] GerarExercito(int tamanho, string tipo)
	{
    	Personagem[] exercito = new Personagem[tamanho];
    	var geradorLocal = new Random(tipo == "atacante" ? 123 : 456);

    	for (int i = 0; i < tamanho; i++)
    	{
        	if (tipo == "atacante")
        	{
            	exercito[i] = new Personagem
            	{
                	Ataque = geradorLocal.Next(80, 120),  	// 80-119 ataque
                	Defesa = geradorLocal.Next(20, 40),   	// 20-39 defesa 
                	ChanceCritico = geradorLocal.Next(15, 25), // 15-24% crítico
                	MultCritico = geradorLocal.Next(180, 220), // 1.8x-2.2x crítico
                	Vida = geradorLocal.Next(100, 150),
                	Vivo = true
            	};
        	}
        	else // defensor
        	{
            	exercito[i] = new Personagem
            	{
                	Ataque = geradorLocal.Next(60, 80),   	// menos ataque
                	Defesa = geradorLocal.Next(40, 70),   	// mais defesa
                	ChanceCritico = geradorLocal.Next(10, 20),
                	MultCritico = geradorLocal.Next(150, 200),
                	Vida = geradorLocal.Next(120, 180),   	// mais vida
                	Vivo = true
            	};
        	}
    	}
       
    	return exercito;
	}

    public static int[] GerarValoresAleatorios(int tamanho)
    {
        var valores = new int[tamanho];
        var geradorLocal = new Random(42); // Mesma semente da versão original para consistência
        for (int i = 0; i < tamanho; i++)
        {
            valores[i] = geradorLocal.Next(0, 100);
        }
        return valores;
    }
}

// Parte 1: Reestruturação de Dados para SIMD (SoA - Struct of Arrays)
public class ExercitoSIMD
{
	public int[] Ataques;
	public int[] Defesas;
	public int[] ChancesCritico;
	public int[] MultCriticos;
	public int[] Vidas;
	public int[] Vivos; // Alterado para int[] (1=vivo, 0=morto) para vetorização
   
	public ExercitoSIMD(int tamanho)
	{
    	Ataques = new int[tamanho];
    	Defesas = new int[tamanho];
    	ChancesCritico = new int[tamanho];
    	MultCriticos = new int[tamanho];
    	Vidas = new int[tamanho];
    	Vivos = new int[tamanho];
	}
   
	public void ConverterDePersonagens(Personagem[] personagens)
	{
        for (int i = 0; i < personagens.Length; i++)
        {
            Ataques[i] = personagens[i].Ataque;
            Defesas[i] = personagens[i].Defesa;
            ChancesCritico[i] = personagens[i].ChanceCritico;
            MultCriticos[i] = personagens[i].MultCritico;
            Vidas[i] = personagens[i].Vida;
            Vivos[i] = personagens[i].Vivo ? 1 : 0; // Conversão de bool para int
        }
	}
}

// Parte 2: Cálculo de Dano com SIMD
public class SimuladorCombateSIMD
{
	public static long CalcularDanoVetorizado(ExercitoSIMD atacantes, ExercitoSIMD defensores, int[] valoresAleatorios)
	{
    	long danoTotalAcumulado = 0;
    	int tamanhoVetor = Vector<int>.Count;
    	int i = 0;

        Vector<int> vetorUm = Vector<int>.One;
        Vector<int> vetorZero = Vector<int>.Zero;
        Vector<int> vetorCem = new Vector<int>(100);
       
    	// Loop principal processando em blocos do tamanho do vetor SIMD
    	for (i = 0; i <= atacantes.Ataques.Length - tamanhoVetor; i += tamanhoVetor)
    	{
        	// DESAFIO 4: Criar máscara para personagens vivos (atacantes E defensores)
            var vetorVivosAtacante = new Vector<int>(atacantes.Vivos, i);
            var vetorVivosDefensor = new Vector<int>(defensores.Vivos, i);
            var mascaraVivos = Vector.BitwiseAnd(
                Vector.Equals(vetorVivosAtacante, vetorUm),
                Vector.Equals(vetorVivosDefensor, vetorUm)
            );

        	// DESAFIO 1: Processar dano base (Ataque - Defesa, mínimo 1)
            var vetorAtaques = new Vector<int>(atacantes.Ataques, i);
            var vetorDefesas = new Vector<int>(defensores.Defesas, i);
            var danoBase = Vector.Subtract(vetorAtaques, vetorDefesas);
            danoBase = Vector.Max(vetorUm, danoBase);
       
        	// DESAFIO 2: Implementar sistema de crítico vetorizado
            var vetorChanceCritico = new Vector<int>(atacantes.ChancesCritico, i);
            var vetorAleatorio = new Vector<int>(valoresAleatorios, i);
            var mascaraCritico = Vector.LessThan(vetorAleatorio, vetorChanceCritico);
       
        	// DESAFIO 3: Aplicar multiplicadores de crítico
            var vetorMultCritico = new Vector<int>(atacantes.MultCriticos, i);
            var danoCriticoNumerador = Vector.Multiply(danoBase, vetorMultCritico);
            var danoCritico = Vector.Divide(danoCriticoNumerador, vetorCem);

            var danoComCritico = Vector.ConditionalSelect(mascaraCritico, danoCritico, danoBase);
       
            // Aplicar máscara de vivos: zerar dano de personagens mortos
            var danoFinal = Vector.ConditionalSelect(mascaraVivos, danoComCritico, vetorZero);
            
            // Acumular o dano do vetor
            danoTotalAcumulado += Vector.Dot(danoFinal, Vector<int>.One);
    	}

        // Processar o restante dos elementos que não preenchem um vetor completo
        for (; i < atacantes.Ataques.Length; i++)
        {
            if (atacantes.Vivos[i] == 1 && defensores.Vivos[i] == 1)
            {
                int danoBase = Math.Max(1, atacantes.Ataques[i] - defensores.Defesas[i]);
                bool ehCritico = valoresAleatorios[i] < atacantes.ChancesCritico[i];
                if (ehCritico)
                {
                    danoBase = (danoBase * atacantes.MultCriticos[i]) / 100;
                }
                danoTotalAcumulado += danoBase;
            }
        }
       
    	return danoTotalAcumulado;
	}
}


class Program
{
    static void Main()
	{
		TestarPerformanceCompleta();
	}
    
    // Sistema de Benchmark
    public static void TestarPerformanceCompleta()
    {
        int[] tamanhosExercito = { 10_000, 50_000, 100_000, 500_000, 1_000_000, 5_000_000 };
    
        Console.WriteLine("=== BENCHMARK DE SISTEMA DE COMBATE ===");
        Console.WriteLine($"Suporte a SIMD por Hardware: {Vector.IsHardwareAccelerated}");
        Console.WriteLine($"Tamanho do Vetor (elementos de 32 bits): {Vector<int>.Count}");
        Console.WriteLine(new string('=', 78));
        Console.WriteLine($"| {"Tamanho Exército",-20} | {"Método",-10} | {"Tempo (ms)",-12} | {"Dano Total",-15} | {"Speedup",-9} |");
        Console.WriteLine(new string('=', 78));
    
        foreach (int tamanho in tamanhosExercito)
        {
            // Geração de dados
            Personagem[] atacantes = SimuladorCombate.GerarExercito(tamanho, "atacante");
            Personagem[] defensores = SimuladorCombate.GerarExercito(tamanho, "defensor");
            int[] valoresAleatorios = SimuladorCombate.GerarValoresAleatorios(tamanho);

            // Teste da versão original (sem SIMD)
            Stopwatch cronometroOriginal = Stopwatch.StartNew();
            long danoTotalOriginal = SimuladorCombate.SimularRodadaCombate(atacantes, defensores);
            cronometroOriginal.Stop();
            long tempoOriginalMs = Math.Max(1, cronometroOriginal.ElapsedMilliseconds);
            Console.WriteLine($"| {tamanho,-20:N0} | {"Original",-10} | {tempoOriginalMs,-12:N0} | {danoTotalOriginal,-15:N0} | {"-",-9} |");

            // Preparação para o teste SIMD
            ExercitoSIMD atacantesSIMD = new ExercitoSIMD(tamanho);
            ExercitoSIMD defensoresSIMD = new ExercitoSIMD(tamanho);
            atacantesSIMD.ConverterDePersonagens(atacantes);
            defensoresSIMD.ConverterDePersonagens(defensores);

            // Teste da versão otimizada (com SIMD)
            Stopwatch cronometroSIMD = Stopwatch.StartNew();
            long danoTotalSIMD = SimuladorCombateSIMD.CalcularDanoVetorizado(atacantesSIMD, defensoresSIMD, valoresAleatorios);
            cronometroSIMD.Stop();
            long tempoSIMDMs = Math.Max(1, cronometroSIMD.ElapsedMilliseconds);
            
            // Calcular e exibir speedup
            double speedup = (double)tempoOriginalMs / tempoSIMDMs;
            string speedupText = $"{speedup:F2}x";
            Console.WriteLine($"| {" ",-20} | {"SIMD",-10} | {tempoSIMDMs,-12:N0} | {danoTotalSIMD,-15:N0} | {speedupText,-9} |");

            if (tamanho != tamanhosExercito.Last())
            {
                Console.WriteLine(new string('-', 78));
            }
        }
        Console.WriteLine(new string('=', 78));
    }
}
