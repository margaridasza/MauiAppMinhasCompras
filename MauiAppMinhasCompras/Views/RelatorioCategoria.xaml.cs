using MauiAppMinhasCompras.Models;//
namespace MauiAppMinhasCompras.Views;

public partial class RelatorioCategoria : ContentPage
{
    public RelatorioCategoria()
    {
        InitializeComponent();
        CarregarRelatorio();
    }

    private async void CarregarRelatorio()
    {
        var produtos = await App.Db.GetAll();

        var relatorio = produtos
            .GroupBy(p => string.IsNullOrWhiteSpace(p.Categoria) ? "Sem categoria" : p.Categoria)
            .Select(g => new { Categoria = g.Key, Total = g.Sum(p => p.Total) })
            .OrderByDescending(x => x.Total)
            .ToList();

        RelatorioCollectionView.ItemsSource = relatorio;
    }
}
