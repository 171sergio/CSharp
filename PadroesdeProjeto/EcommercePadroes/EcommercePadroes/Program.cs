using System;

class SistemaECommerce
{
    public static void Main()
    {
        // 1. Configurar sistema usando Singleton
        var configuracao = GerenciadorConfiguracao.Instancia;
        Console.WriteLine($"Conexão: {configuracao.ConexaoBancoDados}, Taxa Imposto: {configuracao.TaxaImposto}");

        // 2. Criar produtos usando Factory
        var fabricaEletronicos = new FabricaEletronicos();
        var smartphone = fabricaEletronicos.CriarProduto("iPhone", 999.99m);
        var fabricaRoupa = new FabricaRoupa();
        var camiseta = fabricaRoupa.CriarProduto("Camiseta Polo", 79.90m);
        ((Roupa)camiseta).Tamanho = "M";
        var fabricaLivro = new FabricaLivro();
        var livro = fabricaLivro.CriarProduto("Clean Code", 120.00m);
        ((Livro)livro).Autor = "Robert C. Martin";
        ((Livro)livro).NumeroPaginas = 350;

        // 3. Aplicar decoradores
        var smartphoneComGarantia = new DecoradorGarantia(smartphone, 12);
        var produtoFinal = new DecoradorFreteExpresso(smartphoneComGarantia);
        var presente = new DecoradorEmbalagemPresente(livro);

        // 4. Criar pedido e adicionar observadores
        var pedido = new Pedido();
        pedido.Inscrever(new NotificadorEmail());
        pedido.Inscrever(new NotificadorSMS());
        pedido.Status = "Aguardando Pagamento";

        // 5. Processar pagamento usando Strategy
        var contextoPagamento = new ContextoPagamento();
        var pagamentoCartao = new PagamentoCartaoCredito { NumeroCartao = "1234567890123456", NomeTitular = "João Silva" };
        contextoPagamento.DefinirEstrategiaPagamento(pagamentoCartao);
        bool pago = contextoPagamento.ExecutarPagamento(produtoFinal.Preco);
        Console.WriteLine($"Pagamento: {pagamentoCartao.ObterDetalhespagamento()} - Sucesso: {pago}");
        pedido.Status = pago ? "Pago" : "Pagamento Recusado";

        // Testando outros métodos de pagamento
        var pagamentoPayPal = new PagamentoPayPal { EmailPayPal = "cliente@email.com" };
        contextoPagamento.DefinirEstrategiaPagamento(pagamentoPayPal);
        contextoPagamento.ExecutarPagamento(camiseta.Preco);

        var pagamentoPix = new PagamentoPix { ChavePix = "chave-pix-123" };
        contextoPagamento.DefinirEstrategiaPagamento(pagamentoPix);
        contextoPagamento.ExecutarPagamento(presente.Preco);

        // Exibir detalhes dos produtos decorados
        Console.WriteLine($"Produto: {produtoFinal.Nome}, Categoria: {produtoFinal.ObterCategoria()}, Preço: {produtoFinal.Preco:C}, Frete: {produtoFinal.CalcularFrete():C}");
        Console.WriteLine($"Produto: {presente.Nome}, Categoria: {presente.ObterCategoria()}, Preço: {presente.Preco:C}, Frete: {presente.CalcularFrete():C}");
    }
}
