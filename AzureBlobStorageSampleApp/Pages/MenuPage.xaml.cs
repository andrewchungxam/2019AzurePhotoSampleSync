﻿using System;
using System.Collections.Generic;

using Xamarin.Forms;

using System.Collections.Generic;

using Xamarin.Forms.Xaml;
using AzureBlobStorageSampleApp.Shared;

namespace AzureBlobStorageSampleApp.Pages
{
        [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MenuPage : ContentPage
    {
        MainPage RootPage { get => Application.Current.MainPage as MainPage; }
        List<HomeMenuItem> menuItems;
        public MenuPage()
        {
            InitializeComponent();

            menuItems = new List<HomeMenuItem>
            {
                new HomeMenuItem {Id = MenuItemType.Browse, Title="Browse" },
                new HomeMenuItem {Id = MenuItemType.GeographyListPage, Title="Location"},
                new HomeMenuItem {Id = MenuItemType.DateTimeListPage, Title="Date"},
                new HomeMenuItem {Id = MenuItemType.BarcodeListPage, Title="Barcode"},
                new HomeMenuItem {Id = MenuItemType.About, Title="About" },
            };

            ListViewMenu.ItemsSource = menuItems;

            ListViewMenu.SelectedItem = menuItems[0];
            ListViewMenu.ItemSelected += async (sender, e) =>
            {
                if (e.SelectedItem == null)
                    return;

                var id = (int)((HomeMenuItem)e.SelectedItem).Id;
                await RootPage.NavigateFromMenu(id);
            };
        }
    }
}
