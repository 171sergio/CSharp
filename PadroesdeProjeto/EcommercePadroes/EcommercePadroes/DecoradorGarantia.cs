// Decorator: Garantia estendida
public class DecoradorGarantia : DecoradorProduto
{
    private int _mesesGarantia;

    public DecoradorGarantia(Produto produto, int mesesGarantia) : base(produto)
    {
        _mesesGarantia = mesesGarantia;
        Nome = produto.Nome;
        Preco = produto.Preco + (mesesGarantia * 10); // R$10 por mês
    }

    public override string ObterCategoria() => _produto.ObterCategoria() + " + Garantia";
    public override decimal CalcularFrete() => _produto.CalcularFrete();
}
