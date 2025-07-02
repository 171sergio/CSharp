// Decorator: Embalagem para presente
public class DecoradorEmbalagemPresente : DecoradorProduto
{
    public DecoradorEmbalagemPresente(Produto produto) : base(produto)
    {
        Nome = produto.Nome;
        Preco = produto.Preco + 5;
    }

    public override string ObterCategoria() => _produto.ObterCategoria() + " + Embalagem Presente";
    public override decimal CalcularFrete() => _produto.CalcularFrete();
}
