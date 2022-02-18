using Plugin.Media;
using Plugin.Media.Abstractions;
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
    public class AddSite : ContentPage
    {
        byte[] cam_image;

        private Entry descEntry;
        private Entry latEntry;
        private Entry longEntry;

        private Image imagePreview;

        private Button cam_imgButton;
        private Button saveButton;

        string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "myDbSite.db3");

        public AddSite()
        {
            this.Title = "Añadir Sitio";
            StackLayout stackLayout = new StackLayout();

            //Camera Btn
            cam_imgButton = new Button();
            cam_imgButton.Text = "Tomar Foto";
            cam_imgButton.Clicked += cam_imgButton_Clicked;
            stackLayout.Children.Add(cam_imgButton);

            //Image Preview
            imagePreview = new Image();
            stackLayout.Children.Add(imagePreview);

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

            //Save Btn
            saveButton = new Button();
            saveButton.Text = "Guardar";
            saveButton.Clicked += saveButton_Clicked;
            stackLayout.Children.Add(saveButton);

            Content = stackLayout;

            getLocation();
        }

        private async void cam_imgButton_Clicked(object sender, EventArgs e)
        {
            var result = await MediaPicker.CapturePhotoAsync();
            var stream = await result.OpenReadAsync();
            imagePreview.Source = ImageSource.FromStream(() => stream);

            using (MemoryStream memory = new MemoryStream())
            {
                stream.CopyTo(memory);
                cam_image = memory.ToArray();
            }
        }

        public async void getLocation()
        {
            Xamarin.Essentials.Location location = await Geolocation.GetLastKnownLocationAsync();

            if (location == null)
            {
                location = await Geolocation.GetLocationAsync(new GeolocationRequest
                {
                    DesiredAccuracy = GeolocationAccuracy.High,
                    Timeout = TimeSpan.FromSeconds(30)
                });
            }

            latEntry.Text = location.Latitude.ToString();
            longEntry.Text= location.Longitude.ToString();
        }

        private async void saveButton_Clicked(object sender, EventArgs e)
        {
            var db = new SQLiteConnection(dbPath);
            db.CreateTable<Sitio>();

            var maxPK = db.Table<Sitio>().OrderByDescending(c => c.Id).FirstOrDefault();

            if (!string.IsNullOrEmpty(descEntry.Text))
            {
                Sitio sitio = new Sitio()
                {
                    Id = (maxPK == null ? 1 : maxPK.Id + 1),
                    desc = descEntry.Text,
                    latitud = latEntry.Text,
                    longitud = longEntry.Text,
                    save_image = cam_image
                };

                db.Insert(sitio);
                await DisplayAlert(null, "Sitio: " + sitio.desc + " Guardado!", "OK");
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Error", "Debe llenar todos los campos", "OK");
            }
        }
    }
}