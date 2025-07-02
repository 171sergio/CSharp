using System.Collections.Generic;

// Observable para pedidos
public class Pedido
{
    private List<IObservadorPedido> _observadores = new List<IObservadorPedido>();
    private string _status;

    public string Status
    {
        get => _status;
        set
        {
            _status = value;
            NotificarObservadores();
        }
    }

    public void Inscrever(IObservadorPedido observador)
    {
        _observadores.Add(observador);
    }

    private void NotificarObservadores()
    {
        foreach (var observador in _observadores)
        {
            observador.AoMudarStatusPedido(this, _status);
        }
    }
}
