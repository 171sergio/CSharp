using System;

// Notificador por e-mail
public class NotificadorEmail : IObservadorPedido
{
    public void AoMudarStatusPedido(Pedido pedido, string novoStatus)
    {
        Console.WriteLine($"[E-mail] Status do pedido alterado para: {novoStatus}");
    }
}
