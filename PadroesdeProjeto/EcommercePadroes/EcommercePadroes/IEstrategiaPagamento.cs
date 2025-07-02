// Interface Strategy para métodos de pagamento
public interface IEstrategiaPagamento
{
    bool ProcessarPagamento(decimal valor);
    string ObterDetalhespagamento();
}
