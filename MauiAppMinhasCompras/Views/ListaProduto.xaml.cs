using MauiAppMinhasCompras.Models;
using System.Collections.ObjectModel;

namespace MauiAppMinhasCompras.Views;

public partial class ListaProduto : ContentPage
{
    ObservableCollection<Produto> lista = new();            // base
    ObservableCollection<Produto> listaFiltrada = new();    // ligada à UI

    public ListaProduto()
    {
        InitializeComponent();
        lst_produtos.ItemsSource = listaFiltrada; // sempre aponta para a filtrada
    }

    protected async override void OnAppearing()
    {
        try
        {
            lista.Clear();

            var tmp = await App.Db.GetAll();
            tmp.ForEach(i => lista.Add(i));

            ReaplicarFiltros();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
    }

    private void ReaplicarFiltros()
    {
        var termo = txt_search?.Text?.Trim() ?? "";
        var categoria = pickerCategoria?.SelectedItem?.ToString() ?? "Todos";

        IEnumerable<Produto> query = lista;

        if (categoria != "Todos" && !string.IsNullOrWhiteSpace(categoria))
            query = query.Where(p => string.Equals(p.Categoria, categoria, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(termo))
            query = query.Where(p => p.Descricao?.Contains(termo, StringComparison.OrdinalIgnoreCase) == true);

        listaFiltrada.Clear();
        foreach (var p in query) listaFiltrada.Add(p);
    }

    private void ToolbarItem_Clicked(object sender, EventArgs e)
    {
        try
        {
            Navigation.PushAsync(new Views.NovoProduto());
        }
        catch (Exception ex)
        {
            DisplayAlert("Ops", ex.Message, "OK");
        }
    }

    private async void txt_search_TextChanged(object sender, TextChangedEventArgs e)
    {
        try
        {
            lst_produtos.IsRefreshing = true;

            // opção B: busca no banco
            lista.Clear();
            var tmp = await App.Db.Search(e.NewTextValue);
            tmp.ForEach(i => lista.Add(i));

            ReaplicarFiltros();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
        finally
        {
            lst_produtos.IsRefreshing = false;
        }
    }

    private void ToolbarItem_Clicked_1(object sender, EventArgs e)
    {
        var soma = listaFiltrada.Sum(i => i.Total); // soma apenas os filtrados
        DisplayAlert("Total dos Produtos", $"O total é {soma:C}", "OK");
    }

    private async void MenuItem_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (sender is MenuItem selecionado && selecionado.BindingContext is Produto p)
            {
                bool confirm = await DisplayAlert("Tem Certeza?", $"Remover {p.Descricao}?", "Sim", "Não");

                if (confirm)
                {
                    await App.Db.Delete(p.Id);
                    lista.Remove(p);
                    listaFiltrada.Remove(p);
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
    }

    private void lst_produtos_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        try
        {
            if (e.SelectedItem is Produto p)
            {
                Navigation.PushAsync(new Views.EditarProduto
                {
                    BindingContext = p,
                });
            }
        }
        catch (Exception ex)
        {
            DisplayAlert("Ops", ex.Message, "OK");
        }
    }

    private async void lst_produtos_Refreshing(object sender, EventArgs e)
    {
        try
        {
            lista.Clear();

            var tmp = await App.Db.GetAll();
            tmp.ForEach(i => lista.Add(i));

            ReaplicarFiltros();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
        finally
        {
            lst_produtos.IsRefreshing = false;
        }
    }

    private async void ToolbarItem_Clicked_Relatorio(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RelatorioCategoria());
    }

    private void PickerCategoria_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            ReaplicarFiltros();

            var categoria = pickerCategoria.SelectedItem?.ToString() ?? "Todos";
            var soma = listaFiltrada.Sum(p => p.Total);

            DisplayAlert("Total da Categoria", $"O total de {categoria} é {soma:C}", "OK");
        }
        catch (Exception ex)
        {
            DisplayAlert("Ops", ex.Message, "OK");
        }
    }
}
