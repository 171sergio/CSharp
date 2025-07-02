using System;

// Singleton para gerenciar configurações globais do sistema
public sealed class GerenciadorConfiguracao
{
    private static GerenciadorConfiguracao _instancia;
    private static readonly object _bloqueio = new object();

    private GerenciadorConfiguracao() { }

    public static GerenciadorConfiguracao Instancia
    {
        get
        {
            if (_instancia == null)
            {
                lock (_bloqueio)
                {
                    if (_instancia == null)
                        _instancia = new GerenciadorConfiguracao();
                }
            }
            return _instancia;
        }
    }

    public string ConexaoBancoDados { get; set; } = "ConexaoPadrao";
    public decimal TaxaImposto { get; set; } = 0.08m;
}
