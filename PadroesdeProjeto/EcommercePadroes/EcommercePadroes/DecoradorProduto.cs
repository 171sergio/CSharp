// Decorator base para produtos
public abstract class DecoradorProduto : Produto
{
    protected Produto _produto;

    public DecoradorProduto(Produto produto)
    {
        _produto = produto;
    }

    public override string ObterCategoria() => _produto.ObterCategoria();
    public override decimal CalcularFrete() => _produto.CalcularFrete();
}
