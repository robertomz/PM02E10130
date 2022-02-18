using PM02E10130.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PM02E10130
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage (new HomePage());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
