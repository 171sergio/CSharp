using System;

// Notificador por SMS
public class NotificadorSMS : IObservadorPedido
{
    public void AoMudarStatusPedido(Pedido pedido, string novoStatus)
    {
        Console.WriteLine($"[SMS] Status do pedido alterado para: {novoStatus}");
    }
}
