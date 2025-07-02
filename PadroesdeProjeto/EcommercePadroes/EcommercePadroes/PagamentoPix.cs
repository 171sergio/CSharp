using System;

// Pagamento com Pix
public class PagamentoPix : IEstrategiaPagamento
{
    public string ChavePix { get; set; }

    public bool ProcessarPagamento(decimal valor)
    {
        if (valor > 0)
        {
            Console.WriteLine("Pagamento via Pix aprovado.");
            return true;
        }
        Console.WriteLine("Pagamento via Pix recusado.");
        return false;
    }

    public string ObterDetalhespagamento()
    {
        return $"PIX ({ChavePix})";
    }
}
