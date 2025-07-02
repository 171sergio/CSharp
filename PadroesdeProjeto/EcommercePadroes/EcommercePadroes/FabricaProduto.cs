// Factory Method para criação de produtos
public abstract class FabricaProduto
{
    public abstract Produto CriarProduto(string nome, decimal preco);
}

public class FabricaEletronicos : FabricaProduto
{
    public override Produto CriarProduto(string nome, decimal preco)
    {
        return new Eletronico { Nome = nome, Preco = preco };
    }
}

public class FabricaRoupa : FabricaProduto
{
    public override Produto CriarProduto(string nome, decimal preco)
    {
        return new Roupa { Nome = nome, Preco = preco };
    }
}

public class FabricaLivro : FabricaProduto
{
    public override Produto CriarProduto(string nome, decimal preco)
    {
        return new Livro { Nome = nome, Preco = preco };
    }
}
