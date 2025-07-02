// Produto concreto: Eletrônico
public class Eletronico : Produto
{
    public override string ObterCategoria() => "Eletrônicos";
    public override decimal CalcularFrete() => Preco * 0.05m;
}
