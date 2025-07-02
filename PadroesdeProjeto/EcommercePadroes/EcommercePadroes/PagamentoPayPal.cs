using System;

// Pagamento com PayPal
public class PagamentoPayPal : IEstrategiaPagamento
{
    public string EmailPayPal { get; set; }

    public bool ProcessarPagamento(decimal valor)
    {
        if (valor > 0 && !string.IsNullOrWhiteSpace(EmailPayPal))
        {
            Console.WriteLine("Pagamento via PayPal aprovado.");
            return true;
        }
        Console.WriteLine("Pagamento via PayPal recusado.");
        return false;
    }

    public string ObterDetalhespagamento()
    {
        return $"PayPal ({EmailPayPal})";
    }
}
