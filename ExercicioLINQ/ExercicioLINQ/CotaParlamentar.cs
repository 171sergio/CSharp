using System.Globalization;

class CotaParlamentar
{
    public int? Legislatura { get; set; }
    public DateTime? DataEmissao { get; set; }
    public int? DocumentoId { get; set; }
    public string CadastroId { get; set; }
    public int? TipoDocumento { get; set; }
    public string CarteiraParlamentar { get; set; }
    public int? DeputadoId { get; set; }
    public int? NumeroLegislatura { get; set; }
    public int? Ano { get; set; }
    public int? EspecificacaoSubCota { get; set; }
    public int? Lote { get; set; }
    public int? Mes { get; set; }
    public int? Parcela { get; set; }
    public string? Ressarcimento { get; set; }
    //public int? SubCota { get; set; }
    public string? Partido { get; set; }
    public string? UF { get; set; }
    public string? NomeParlamentar { get; set; }
    //public string CNPJCPF { get; set; }
    public string? Descricao { get; set; }
    //public string DescricaoEspecificacao { get; set; }
    public string? Fornecedor { get; set; }
    //public string Numero { get; set; }
    //public string Passageiro { get; set; }
    //public string Trecho { get; set; }
    //public decimal? ValorDocumento { get; set; }
    //public decimal? ValorGlosa { get; set; }
    public decimal? ValorLiquido { get; set; }
    //public decimal? ValorRestituicao { get; set; }

    public static IEnumerable<CotaParlamentar> LerCotasParlamentares(string caminhoArquivo)
    {
        var linhas = File.ReadAllLines(caminhoArquivo).Skip(1);
        var lista = new List<CotaParlamentar>();

        foreach (var linha in linhas)
        {
            var campos = linha.Split(',');

            var cota = new CotaParlamentar
            {
                Legislatura = int.TryParse(campos[0], out var legislatura) ? legislatura : (int?)null,
                DataEmissao = DateTime.TryParse(campos[1], out var dataEmissao) ? dataEmissao : (DateTime?)null,
                DocumentoId = int.TryParse(campos[2], out var documentoId) ? documentoId : (int?)null,
                //CadastroId = string.IsNullOrEmpty(campos[3]) ? null : campos[3],
                //TipoDocumento = int.TryParse(campos[4], out var tipoDocumento) ? tipoDocumento : (int?)null,
                //CarteiraParlamentar = string.IsNullOrEmpty(campos[5]) ? null : campos[5],
                //DeputadoId = int.TryParse(campos[6], out var deputadoId) ? deputadoId : (int?)null,
                //NumeroLegislatura = int.TryParse(campos[7], out var numeroLegislatura) ? numeroLegislatura : (int?)null,
                Ano = int.TryParse(campos[8], out var ano) ? ano : (int?)null,
                //EspecificacaoSubCota = int.TryParse(campos[9], out var especificacaoSubCota) ? especificacaoSubCota : (int?)null,
                //Lote = int.TryParse(campos[10], out var lote) ? lote : (int?)null,
                //Mes = int.TryParse(campos[11], out var mes) ? mes : (int?)null,
                //Parcela = int.TryParse(campos[12], out var parcela) ? parcela : (int?)null,
                Ressarcimento = string.IsNullOrEmpty(campos[13]) ? null : campos[13],
                //SubCota = int.TryParse(campos[14], out var subCota) ? subCota : (int?)null,
                Partido = campos[15],
                UF = campos[16],
                NomeParlamentar = campos[17],
                //CNPJCPF = campos[18],
                Descricao = campos[19],
                //DescricaoEspecificacao = campos[20],
                Fornecedor = campos[21],
                //Numero = campos[22],
                //Passageiro = campos[23],
                //Trecho = campos[24],
                //ValorDocumento = decimal.TryParse(campos[25], CultureInfo.InvariantCulture, out var valorDocumento) ? valorDocumento : (decimal?)null,
                //ValorGlosa = decimal.TryParse(campos[26], CultureInfo.InvariantCulture, out var valorGlosa) ? valorGlosa : (decimal?)null,
                ValorLiquido = decimal.TryParse(campos[27], CultureInfo.InvariantCulture, out var valorLiquido) ? valorLiquido : (decimal?)null,
                //ValorRestituicao = decimal.TryParse(campos[28], CultureInfo.InvariantCulture, out var valorRestituicao) ? valorRestituicao : (decimal?)null
           };

            yield return cota;
        }
    }
}