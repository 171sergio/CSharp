using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace MonitorCriptomoedas
{
    class Program
    {
        private static readonly ConcurrentDictionary<string, decimal> PrecosAtuais = new();
        private static readonly ConcurrentDictionary<string, decimal> PrecosAnteriores = new();
        private static readonly ConcurrentDictionary<string, DateTime> HorariosAtualizacao = new();

        static async Task Main(string[] args)
        {
            Console.WriteLine("Monitor de Criptomoedas");
            Console.WriteLine("Pressione ESC para sair\n");

            var criptomoedas = new List<string>
            {
                "BTC",  // Bitcoin
                "ETH",  // Ethereum
                "LTC",  // Litecoin
                "BCH",  // Bitcoin Cash
                "XRP",  // Ripple
                "ADA",  // Cardano
                "DOT",  // Polkadot
                "LINK", // Chainlink
                "XLM",  // Stellar
                "DOGE"  // Dogecoin
            };

            var cts = new CancellationTokenSource();
            
            // Inicia a tarefa para monitorar a tecla ESC
            _ = MonitorarTeclaEscAsync(cts);

            try
            {
                while (!cts.Token.IsCancellationRequested)
                {
                    Console.Clear();
                    Console.WriteLine("Monitor de Criptomoedas - Cotações em USD");
                    Console.WriteLine("Pressione ESC para sair\n");
                    Console.WriteLine("Símbolo\tPreço\t\tVariação\tÚltima Atualização");
                    Console.WriteLine("--------------------------------------------------------------");

                    // Armazena os preços atuais como anteriores antes de fazer novas requisições
                    foreach (var simbolo in criptomoedas)
                    {
                        if (PrecosAtuais.TryGetValue(simbolo, out var precoAtual))
                        {
                            PrecosAnteriores[simbolo] = precoAtual;
                        }
                    }

                    // Realiza requisições em paralelo
                    var tarefas = criptomoedas.Select(simbolo => ObterEConverterCotacaoAsync(simbolo, cts.Token));
                    try
                    {
                        await Task.WhenAll(tarefas);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao obter cotações: {ex.Message}");
                    }

                    // Exibe os resultados
                    foreach (var simbolo in criptomoedas)
                    {
                        ExibirResultadosNoConsole(simbolo);
                    }

                    // Aguarda o intervalo de atualização
                    await Task.Delay(30000, cts.Token); // 30 segundos
                }
            }
            catch (TaskCanceledException)
            {
                // Esperado quando o token é cancelado
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro não tratado: {ex.Message}");
            }
            finally
            {
                Console.Clear();
                Console.WriteLine("Monitor de Criptomoedas encerrado.");
            }
        }

        static HttpClient CriarClienteHttp()
        {
            var cliente = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(10)
            };
            cliente.DefaultRequestHeaders.Add("User-Agent", "MonitorCripto/1.0");
            cliente.DefaultRequestHeaders.Add("Accept", "application/json");
            return cliente;
        }

        static async Task ObterEConverterCotacaoAsync(string simbolo, CancellationToken token)
        {
            try
            {
                using var clienteHttp = CriarClienteHttp();
                var urlRequisicao = $"https://api.exchange.cryptomkt.com/api/3/public/price/rate?from={simbolo}&to=USDT";
                var resposta = await clienteHttp.GetAsync(urlRequisicao, token);
                resposta.EnsureSuccessStatusCode();

                var json = await resposta.Content.ReadAsStringAsync(token);

                using var documento = JsonDocument.Parse(json);
                if (documento.RootElement.TryGetProperty(simbolo, out var dadosMoeda))
                {
                    var precoString = dadosMoeda.GetProperty("price").GetString();
                    if (decimal.TryParse(precoString, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal precoAtual))
                    {
                        PrecosAtuais[simbolo] = precoAtual;
                        HorariosAtualizacao[simbolo] = DateTime.Now;
                    }
                }
            }
            catch (Exception ex) when (ex is not TaskCanceledException)
            {
                Console.WriteLine($"Erro ao processar {simbolo}: {ex.Message}");
            }
        }

        static void ExibirResultadosNoConsole(string simbolo)
        {
            if (!PrecosAtuais.TryGetValue(simbolo, out var precoAtual))
            {
                Console.WriteLine($"{simbolo}: Dados indisponíveis");
                return;
            }

            var precoAnterior = PrecosAnteriores.GetValueOrDefault(simbolo, precoAtual);
            var ultimaAtualizacao = HorariosAtualizacao.GetValueOrDefault(simbolo, DateTime.Now);
            
            var corOriginal = Console.ForegroundColor;
            var seta = precoAtual > precoAnterior ? "↑" : (precoAtual < precoAnterior ? "↓" : "=");
            Console.ForegroundColor = precoAtual > precoAnterior 
                ? ConsoleColor.Green 
                : (precoAtual < precoAnterior ? ConsoleColor.Red : corOriginal);
            
            var variacao = precoAnterior > 0 
                ? (precoAtual - precoAnterior) / precoAnterior * 100 
                : 0;
            
            Console.Write($"{simbolo}\t${precoAtual:N2}\t{seta} ");
            
            if (precoAnterior > 0 && precoAtual != precoAnterior)
            {
                Console.Write($"{Math.Abs(variacao):N2}%");
            }
            else
            {
                Console.Write("   -   ");
            }
            
            Console.Write($"\t{ultimaAtualizacao:HH:mm:ss}");
            Console.WriteLine();
            Console.ForegroundColor = corOriginal;
        }

        static async Task MonitorarTeclaEscAsync(CancellationTokenSource cts)
        {
            while (true)
            {
                if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape)
                {
                    cts.Cancel();
                    break;
                }
                await Task.Delay(100);
            }
        }
    }
}
