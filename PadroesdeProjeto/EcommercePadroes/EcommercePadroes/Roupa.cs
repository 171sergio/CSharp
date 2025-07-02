// Produto concreto: Roupa
public class Roupa : Produto
{
    public string Tamanho { get; set; }
    public override string ObterCategoria() => "Roupas";
    public override decimal CalcularFrete() => 12.50m;
}
