using System;

// Pagamento com cartão de crédito
public class PagamentoCartaoCredito : IEstrategiaPagamento
{
    public string NumeroCartao { get; set; }
    public string NomeTitular { get; set; }

    public bool ProcessarPagamento(decimal valor)
    {
        if (valor > 0 && valor < 5000)
        {
            Console.WriteLine("Pagamento com cartão aprovado.");
            return true;
        }
        Console.WriteLine("Pagamento com cartão recusado.");
        return false;
    }

    public string ObterDetalhespagamento()
    {
        string ultimos4 = NumeroCartao?.Length >= 4 ? NumeroCartao.Substring(NumeroCartao.Length - 4) : "XXXX";
        return $"Cartão de Crédito (****{ultimos4})";
    }
}
