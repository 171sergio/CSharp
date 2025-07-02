// Produto concreto: Livro
public class Livro : Produto
{
    public string Autor { get; set; }
    public int NumeroPaginas { get; set; }
    public override string ObterCategoria() => "Livros";
    public override decimal CalcularFrete() => NumeroPaginas > 300 ? 8.00m : 5.00m;
}
