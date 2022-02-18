using PM02E10130.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PM02E10130.Views
{
    public class GetSite : ContentPage
    {
        private ListView listView;

        private Entry idEntry;
        private Entry descEntry;
        private Entry latEntry;
        private Entry longEntry;

        private Button mapButton;
        private Button editButton;
        private Button deleteButton;

        Sitio sitio = new Sitio();

        string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "myDbSite.db3");
        public GetSite()
        {
            this.Title = "Sitios";

            var db = new SQLiteConnection(dbPath);

            StackLayout stackLayout = new StackLayout();

            listView = new ListView();
            listView.ItemsSource = db.Table<Sitio>().OrderBy(x => x.Id).ToList();
            listView.ItemSelected += listViewItem;
            stackLayout.Children.Add(listView);

            // ID Entry
            idEntry = new Entry();
            idEntry.Placeholder = "ID";
            idEntry.IsVisible = false;
            stackLayout.Children.Add(idEntry);

            // Descripcion Entry
            descEntry = new Entry();
            descEntry.Keyboard = Keyboard.Text;
            descEntry.Placeholder = "Descripción";
            stackLayout.Children.Add(descEntry);

            // Latitud Entry
            latEntry = new Entry();
            latEntry.Placeholder = "Latitud";
            latEntry.IsEnabled = false;
            stackLayout.Children.Add(latEntry);

            // Longitud Entry
            longEntry = new Entry();
            longEntry.Placeholder = "Longitud";
            longEntry.IsEnabled = false;
            stackLayout.Children.Add(longEntry);

            //Map Btn
            mapButton = new Button();
            mapButton.Text = "Ir al Mapa";
            mapButton.Clicked += goMap_Clicked;
            stackLayout.Children.Add(mapButton);

            //Edit Btn
            editButton = new Button();
            editButton.Text = "Editar";
            editButton.Clicked += editButton_Clicked;
            stackLayout.Children.Add(editButton);
            
            //Delete Btn
            deleteButton = new Button();
            deleteButton.Text = "Eliminar";
            deleteButton.Clicked += deleteButton_Clicked;
            stackLayout.Children.Add(deleteButton);
            
            Content = stackLayout;
        }        

        private void listViewItem(object sender, SelectedItemChangedEventArgs e)
        {
            sitio = (Sitio)e.SelectedItem;

            idEntry.Text = sitio.Id.ToString();
            descEntry.Text = sitio.desc.ToString();
            latEntry.Text = sitio.latitud.ToString();
            longEntry.Text = sitio.longitud.ToString();
        }

        private async void goMap_Clicked(object sender, EventArgs e)
        {
            if (!double.TryParse(latEntry.Text, out double lat))
                return;
            if (!double.TryParse(longEntry.Text, out double lng))
                return;
            await Map.OpenAsync(lat, lng, new MapLaunchOptions
            {
                Name = descEntry.Text,
                NavigationMode = NavigationMode.None
            });
        }

        private async void editButton_Clicked(object sender, EventArgs e)
        {
            var db = new SQLiteConnection(dbPath);

            if (!string.IsNullOrEmpty(descEntry.Text))
            {
                Sitio sitio = new Sitio()
                {
                    Id = Convert.ToInt32(idEntry.Text),
                    desc = descEntry.Text,
                    latitud = latEntry.Text,
                    longitud = longEntry.Text
                };

                db.Update(sitio);
                await DisplayAlert(null, "Sitio: " + sitio.desc + " Editado!", "OK");
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Error", "Debe llenar todos los campos", "OK");
            }
        }
        
        private async void deleteButton_Clicked(object sender, EventArgs e)
        {
            var db = new SQLiteConnection(dbPath);
            db.Table<Sitio>().Delete(x => x.Id == sitio.Id);
            await DisplayAlert(null, "Sitio: " + sitio.desc + " Eliminado", "OK");
            await Navigation.PopAsync();
        }
        
        
    }
}