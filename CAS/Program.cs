using System;

namespace CAS
{
    class Program
    {
        static void Main(string[] args)
        {
            // Operações com números complexos
            Console.WriteLine("Operações com números complexos:");
            Expressao complexo1 = new Complexo(2, 3);
            Expressao complexo2 = new Complexo(1, 4);
            Expressao somaComplexo = complexo1 + complexo2;
            Expressao subtracaoComplexo = complexo1 - complexo2;
            Expressao multiplicacaoComplexo = complexo1 * complexo2;
            Expressao divisaoComplexo = complexo1 / complexo2;

            Console.WriteLine($"Soma de complexos: {somaComplexo}"); // Saída: 3 + 7i
            Console.WriteLine($"Subtração de complexos: {subtracaoComplexo}"); // Saída: 1 - 1i
            Console.WriteLine($"Multiplicação de complexos: {multiplicacaoComplexo}"); // Saída: -10 + 11i
            Console.WriteLine($"Divisão de complexos: {divisaoComplexo}"); // Saída: 0.8235294117647058 + 0.29411764705882354i

            // Substituição de um símbolo por um elemento
            Console.WriteLine("\nSubstituição de um símbolo por um elemento:");
            Expressao expr = new Soma(new Simbolo("x"), new Numero(2));
            Console.WriteLine($"Expressão original: {expr}"); // Saída: (x + 2)
            Expressao substituida = expr.Substituir(new Simbolo("x"), new Numero(5));
            Console.WriteLine($"Expressão substituída: {substituida}"); // Saída: (5 + 2)

            // Simplificação de operações com zero
            Console.WriteLine("\nSimplificação de operações com zero:");

            // Soma com zero
            Expressao somaZero = new Soma(new Simbolo("x"), new Numero(0));
            Console.WriteLine($"Expressão original (soma): {somaZero}"); // Saída: (x + 0)
            Expressao simplificadaSoma = somaZero.Simplificar();
            Console.WriteLine($"Expressão simplificada (soma): {simplificadaSoma}"); // Saída: x

            // Subtracao com zero
            Expressao subtracaoZero = new Subtracao(new Simbolo("y"), new Numero(0));
            Console.WriteLine($"Expressão original (subtração): {subtracaoZero}"); // Saída: (y - 0)
            Expressao simplificadaSubtracao = subtracaoZero.Simplificar();
            Console.WriteLine($"Expressão simplificada (subtração): {simplificadaSubtracao}"); // Saída: y

            // Multiplicacao com zero
            Expressao multiplicacaoZero = new Multiplicacao(new Simbolo("z"), new Numero(0));
            Console.WriteLine($"Expressão original (multiplicação): {multiplicacaoZero}"); // Saída: (z * 0)
            Expressao simplificadaMultiplicacao = multiplicacaoZero.Simplificar();
            Console.WriteLine($"Expressão simplificada (multiplicação): {simplificadaMultiplicacao}"); // Saída: 0

            // Divisao com zero no numerador
            Expressao divisaoZeroNumerador = new Divisao(new Numero(0), new Simbolo("w"));
            Console.WriteLine($"Expressão original (divisão): {divisaoZeroNumerador}"); // Saída: (0 / w)
            Expressao simplificadaDivisaoNumerador = divisaoZeroNumerador.Simplificar();
            Console.WriteLine($"Expressão simplificada (divisão): {simplificadaDivisaoNumerador}"); // Saída: 0

            // Divisao com zero no denominador (deve lançar exceção)
            try
            {
                Expressao divisaoZeroDenominador = new Divisao(new Simbolo("w"), new Numero(0));
                Console.WriteLine($"Expressão original (divisão): {divisaoZeroDenominador}"); // Saída: (w / 0)
                Expressao simplificadaDivisaoDenominador = divisaoZeroDenominador.Simplificar();
                Console.WriteLine($"Expressão simplificada (divisão): {simplificadaDivisaoDenominador}");
            }
            catch (DivideByZeroException ex)
            {
                Console.WriteLine($"Exceção capturada: {ex.Message}"); // Saída: Divisão por zero não é permitida.
            }
        }
    }
}