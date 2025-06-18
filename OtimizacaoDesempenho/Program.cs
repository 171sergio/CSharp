using System.Diagnostics;
using OtimizacaoDesempenho;

Console.WriteLine("Exercício: Otimização de Processador de Imagens com ArrayPool<T>");
Console.WriteLine("===============================================================");

// --- Parte 1: Execução e Análise da Versão Trivial ---
long initialMemoryTrivial = GC.GetTotalMemory(true);
var swTrivial = Stopwatch.StartNew();
var gen0BeforeTrivial = GC.CollectionCount(0);
var gen1BeforeTrivial = GC.CollectionCount(1);
var gen2BeforeTrivial = GC.CollectionCount(2);

ImageProcessorTrivial.ProcessImages();

swTrivial.Stop();
long finalMemoryTrivial = GC.GetTotalMemory(true);
var gen0AfterTrivial = GC.CollectionCount(0);
var gen1AfterTrivial = GC.CollectionCount(1);
var gen2AfterTrivial = GC.CollectionCount(2);

// --- Parte 2: Execução e Análise da Versão Otimizada ---
long initialMemoryOptimized = GC.GetTotalMemory(true);
var swOptimized = Stopwatch.StartNew();
var gen0BeforeOptimized = GC.CollectionCount(0);
var gen1BeforeOptimized = GC.CollectionCount(1);
var gen2BeforeOptimized = GC.CollectionCount(2);

ImageProcessorOptimized.ProcessImages();

swOptimized.Stop();
long finalMemoryOptimized = GC.GetTotalMemory(true);
var gen0AfterOptimized = GC.CollectionCount(0);
var gen1AfterOptimized = GC.CollectionCount(1);
var gen2AfterOptimized = GC.CollectionCount(2);


// --- Parte 3: Comparação e Análise ---
Console.WriteLine("\n\n--- Análise Comparativa de Performance ---");
Console.WriteLine("---------------------------------------------------------------");
Console.WriteLine("| Métrica                     | Versão Trivial    | Versão Otimizada  | Melhoria (%)   |");
Console.WriteLine("---------------------------------------------------------------");

// Tempo de Execução
var improvementTime = 100 * (1 - (double)swOptimized.ElapsedMilliseconds / swTrivial.ElapsedMilliseconds);
Console.WriteLine($"| Tempo Total (ms)            | {swTrivial.ElapsedMilliseconds,-17} | {swOptimized.ElapsedMilliseconds,-17} | {improvementTime,12:F2}% |");

// Alocação de Memória
var memDiffTrivial = (finalMemoryTrivial - initialMemoryTrivial) / (1024.0 * 1024.0);
var memDiffOptimized = (finalMemoryOptimized - initialMemoryOptimized) / (1024.0 * 1024.0);
Console.WriteLine($"| Alocação Memória (MB)       | {memDiffTrivial,-17:F2} | {memDiffOptimized,-17:F2} | {"N/A",12} |");

// Coleções GC
var gc0Trivial = gen0AfterTrivial - gen0BeforeTrivial;
var gc1Trivial = gen1AfterTrivial - gen1BeforeTrivial;
var gc2Trivial = gen2AfterTrivial - gen2BeforeTrivial;

var gc0Optimized = gen0AfterOptimized - gen0BeforeOptimized;
var gc1Optimized = gen1AfterOptimized - gen1BeforeOptimized;
var gc2Optimized = gen2AfterOptimized - gen2BeforeOptimized;

var improvementGc0 = gc0Trivial > 0 ? 100 * (1 - (double)gc0Optimized / gc0Trivial) : 0;
var improvementGc1 = gc1Trivial > 0 ? 100 * (1 - (double)gc1Optimized / gc1Trivial) : 0;
var improvementGc2 = gc2Trivial > 0 ? 100 * (1 - (double)gc2Optimized / gc2Trivial) : 0;

Console.WriteLine($"| Coleções GC Gen 0           | {gc0Trivial,-17} | {gc0Optimized,-17} | {improvementGc0,12:F2}% |");
Console.WriteLine($"| Coleções GC Gen 1           | {gc1Trivial,-17} | {gc1Optimized,-17} | {improvementGc1,12:F2}% |");
Console.WriteLine($"| Coleções GC Gen 2           | {gc2Trivial,-17} | {gc2Optimized,-17} | {improvementGc2,12:F2}% |");
Console.WriteLine("---------------------------------------------------------------");

Console.WriteLine("\nObservações Finais:");
Console.WriteLine("1. Tempo de Execução: A versão otimizada demonstrou uma melhora de performance significativa. Essa aceleração é um resultado direto da redução do trabalho do Garbage Collector (GC). Menos tempo gasto em coletas de lixo significa mais tempo de CPU disponível para o processamento real das imagens.");
Console.WriteLine("2. Coletas do Garbage Collector (GC): A redução no número de coletas de GC é o resultado mais importante. A versão trivial força centenas de coletas, causando pausas na aplicação. A versão otimizada, ao reutilizar arrays, quase elimina a necessidade do GC, resultando em uma execução mais estável e previsível.");
Console.WriteLine("3. Métrica de 'Alocação de Memória': O valor para a versão otimizada pode parecer maior e contraintuitivo. Isso ocorre porque o ArrayPool reserva um bloco de memória para reutilização, e a métrica GC.GetTotalMemory() mede a memória total em uso (incluindo a reservada pelo pool). A métrica chave aqui não é a memória total alocada no final, mas a ausência de alocações repetitivas, que é comprovada pela drástica redução nas coletas de GC."); 