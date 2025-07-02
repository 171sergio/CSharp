// Interface Observer para notificações de pedido
public interface IObservadorPedido
{
    void AoMudarStatusPedido(Pedido pedido, string novoStatus);
}
