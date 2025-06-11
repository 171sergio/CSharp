using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace PrizeSolver
{
    class Program
    {
        static void Main(string[] args)
        {
            // Atividade 1: Validador de Senha
            SolvePasswordValidator();

            Console.WriteLine("\n" + new string('-', 50) + "\n");

            // Atividade 2: Encontrar Ganhadores do Nobel de Economia
            SolveNobelPrizeFinder();
        }

        /// <summary>
        /// 1) Crie uma aplicação de console em C# que solicite ao usuário a digitação de uma senha.
        /// O programa deve verificar se a senha inserida é considerada "forte".
        /// </summary>
        public static void SolvePasswordValidator()
        {
            Console.WriteLine("--- Atividade 1: Validador de Senha Forte ---");

            // Regex para validar a senha forte, conforme solicitado.
            const string strongPasswordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()+=_\-{}\[\]:;""'?<>,.]).{7,16}$";
            var regex = new Regex(strongPasswordPattern);

            while (true)
            {
                Console.Write("Por favor, digite uma senha para validação: ");
                string? password = Console.ReadLine();

                if (password != null && regex.IsMatch(password))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("✓ Sucesso! A senha é forte.");
                    Console.ResetColor();
                    break;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("✗ Falha! A senha não é forte. Tente novamente.");
                    Console.WriteLine("Lembre-se das regras: 7-16 caracteres, 1 maiúscula, 1 minúscula, 1 número e 1 caractere especial.");
                    Console.ResetColor();
                }
            }

            // Explicação da expressão regular
            Console.WriteLine("\n--- Explicação da Expressão Regular (Regex) ---");
            Console.WriteLine($"Padrão: {strongPasswordPattern}\n");
            Console.WriteLine(
                " • ^ e $                        : São âncoras. '^' marca o início da string e '$' marca o fim. Isso garante que a senha inteira deve corresponder ao padrão, não apenas uma parte dela.\n" +
                " • (?=.*[a-z])                  : Este é um 'lookahead positivo'. Ele verifica se existe pelo menos uma letra minúscula (a-z) em qualquer lugar da senha, mas não consome caracteres.\n" +
                " • (?=.*[A-Z])                  : Similar ao anterior, mas para uma letra maiúscula (A-Z).\n" +
                " • (?=.*\\d)                     : Verifica a existência de pelo menos um dígito (0-9). '\\d' é um atalho para [0-9].\n" +
                " • (?=.*[!@#$%^&*()+=_\\-{}\\[\\]:;\"'?<>,.]) : Verifica a existência de pelo menos um caractere especial do conjunto especificado. Caracteres como '[', ']', '{', '}' e '-' são escapados com '\\' para serem tratados como literais.\n" +
                " • .{7,16}                      : Esta parte verifica o comprimento da senha. O '.' corresponde a qualquer caractere, e '{7,16}' especifica que o comprimento deve ser de no mínimo 7 e no máximo 16 caracteres."
            );
        }
        
        /// <summary>
        /// 2) Considere o arquivo JSON com os Prêmios Nobel, encontre o primeiro nome (firstname)
        /// dos ganhadores do prêmio de economia ("category": "economics")
        /// </summary>
        public static void SolveNobelPrizeFinder()
        {
            Console.WriteLine("--- Atividade 2: Ganhadores do Prêmio Nobel de Economia ---");
            
            try
            {
                // Embora regex possa ser usado para extrair dados de strings, para dados estruturados
                // como JSON, o uso de um parser (como System.Text.Json) é muito mais robusto,
                // seguro e recomendado. Regex pode falhar com pequenas variações na formatação do JSON.
                string fileName = "prize.json";
                if (!File.Exists(fileName))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Erro: O arquivo '{fileName}' não foi encontrado no diretório de execução.");
                    Console.ResetColor();
                    return;
                }

                string jsonString = File.ReadAllText(fileName);
                var root = JsonSerializer.Deserialize<RootObject>(jsonString);

                if (root?.Prizes == null)
                {
                     Console.WriteLine("Não foi possível ler os prêmios do arquivo JSON.");
                     return;
                }

                var economicsPrizes = root.Prizes
                    .Where(p => p.Category != null && p.Category.Equals("economics", StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (economicsPrizes.Any())
                {
                    Console.WriteLine("\nPrimeiro nome dos ganhadores do Prêmio Nobel de Economia:\n");
                    foreach (var prize in economicsPrizes)
                    {
                        if (prize.Laureates != null)
                        {
                            foreach (var laureate in prize.Laureates)
                            {
                                // A propriedade 'firstname' pode conter nomes de organizações.
                                // A propriedade 'surname' pode não existir para organizações.
                                if(!string.IsNullOrWhiteSpace(laureate.Firstname))
                                {
                                    Console.ForegroundColor = ConsoleColor.Cyan;
                                    Console.Write($" • Ano: {prize.Year}");
                                    Console.ResetColor();
                                    Console.WriteLine($" - Ganhador: {laureate.Firstname}");
                                }
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Nenhum ganhador do prêmio de economia encontrado no arquivo.");
                }
            }
            catch (JsonException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Erro ao processar o arquivo JSON: {ex.Message}");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Ocorreu um erro inesperado: {ex.Message}");
                Console.ResetColor();
            }
        }
    }

    // Classes para desserializar o JSON prize.json

    public class RootObject
    {
        [JsonPropertyName("prizes")]
        public List<Prize>? Prizes { get; set; }
    }

    public class Prize
    {
        [JsonPropertyName("year")]
        public string? Year { get; set; }

        [JsonPropertyName("category")]
        public string? Category { get; set; }

        [JsonPropertyName("laureates")]
        public List<Laureate>? Laureates { get; set; }
    }

    public class Laureate
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("firstname")]
        public string? Firstname { get; set; }

        [JsonPropertyName("surname")]
        public string? Surname { get; set; }

        [JsonPropertyName("motivation")]
        public string? Motivation { get; set; }

        [JsonPropertyName("share")]
        public string? Share { get; set; }
    }
}
