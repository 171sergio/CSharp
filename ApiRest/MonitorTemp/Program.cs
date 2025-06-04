using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    private static readonly HttpClient client = new HttpClient();
    private static double? lastTemperature = null;
    private static readonly object consoleLock = new object();

    static async Task Main(string[] args)
    {
        Console.WriteLine("Monitor de Temperatura");
        Console.WriteLine("=====================");

        // Solicitar unidade de temperatura
        string unidade;
        do
        {
            Console.Write("Digite a unidade desejada (celsius, kelvin ou fahrenheit): ");
            unidade = Console.ReadLine()?.ToLower() ?? "";
        } while (unidade != "celsius" && unidade != "kelvin" && unidade != "fahrenheit");

        // Solicitar intervalo
        int intervalo;
        do
        {
            Console.Write("Digite o intervalo em segundos entre as leituras: ");
        } while (!int.TryParse(Console.ReadLine(), out intervalo) || intervalo <= 0);

        Console.WriteLine("\nIniciando monitoramento... (Pressione Ctrl+C para encerrar)\n");

        // Configurar tratamento de Ctrl+C
        Console.CancelKeyPress += (sender, e) =>
        {
            e.Cancel = true;
            Console.WriteLine("\nEncerrando monitoramento de temperatura.");
            Environment.Exit(0);
        };

        while (true)
        {
            try
            {
                await LerTemperatura(unidade);
                await Task.Delay(intervalo * 1000);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Erro ao obter leitura: {ex.Message}");
                Console.ResetColor();
                await Task.Delay(intervalo * 1000);
            }
        }
    }

    static async Task LerTemperatura(string unidade)
    {
        var response = await client.GetAsync($"http://localhost:5204/temperatura/{unidade}");
        
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"HTTP Error: {response.StatusCode}");
        }

        var content = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        
        try
        {
            var result = JsonSerializer.Deserialize<TemperaturaResponse>(content, options);
            if (result == null)
            {
                throw new Exception("Resposta inválida");
            }

            var horaAtual = DateTime.Now.ToString("HH:mm:ss");
            var variacao = CompararTemperatura(result.Valor);

            lock (consoleLock)
            {
                Console.Write($"[{horaAtual}] Temperatura: {result.Valor:F2} °{unidade.ToUpper()[0]} → ");
                
                switch (variacao)
                {
                    case Variacao.Subiu:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("SUBIU");
                        break;
                    case Variacao.Desceu:
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.Write("DESCEU");
                        break;
                    default:
                        Console.Write("SEM ALTERAÇÃO");
                        break;
                }
                
                Console.ResetColor();
                Console.WriteLine();
            }

            lastTemperature = result.Valor;
        }
        catch (JsonException)
        {
            throw new Exception("Resposta inválida");
        }
    }

    static Variacao CompararTemperatura(double temperaturaAtual)
    {
        if (!lastTemperature.HasValue)
            return Variacao.SemAlteracao;

        if (temperaturaAtual > lastTemperature.Value)
            return Variacao.Subiu;
        if (temperaturaAtual < lastTemperature.Value)
            return Variacao.Desceu;
        
        return Variacao.SemAlteracao;
    }
}

public class TemperaturaResponse
{
    public string Unidade { get; set; } = "";
    public double Valor { get; set; }
}

public enum Variacao
{
    Subiu,
    Desceu,
    SemAlteracao
}
