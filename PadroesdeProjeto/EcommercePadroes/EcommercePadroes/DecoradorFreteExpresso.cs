// Decorator: Frete expresso
public class DecoradorFreteExpresso : DecoradorProduto
{
    public DecoradorFreteExpresso(Produto produto) : base(produto)
    {
        Nome = produto.Nome;
        Preco = produto.Preco + 15;
    }

    public override string ObterCategoria() => _produto.ObterCategoria() + " + Frete Expresso";
    public override decimal CalcularFrete() => _produto.CalcularFrete() + 15;
}
