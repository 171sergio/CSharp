// Classe base abstrata para produtos
public abstract class Produto
{
    public string Nome { get; set; }
    public decimal Preco { get; set; }
    public abstract string ObterCategoria();
    public abstract decimal CalcularFrete();
}
