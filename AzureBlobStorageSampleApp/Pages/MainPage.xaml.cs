using System;
using System.Collections.Generic;

using Xamarin.Forms;

using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using AzureBlobStorageSampleApp.Shared;

namespace AzureBlobStorageSampleApp.Pages
{
   [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : MasterDetailPage
    {
        Dictionary<int, NavigationPage> MenuPages = new Dictionary<int, NavigationPage>();
        public MainPage()
        {
            InitializeComponent();

            MasterBehavior = MasterBehavior.Popover;

            MenuPages.Add((int)MenuItemType.Browse, (NavigationPage)Detail);
        }

        public async Task NavigateFromMenu(int id)
        {
            if (!MenuPages.ContainsKey(id))
            {
                switch (id)
                {
                    case (int)MenuItemType.Browse:
                        MenuPages.Add(id, new NavigationPage(new PhotoListPage()));
                        break;
                    case (int)MenuItemType.GeographyListPage:
                        MenuPages.Add(id, new NavigationPage(new GeographyListPage()){ BarBackgroundColor = ColorConstants.NavigationBarBackgroundColor, BarTextColor = ColorConstants.NavigationBarTextColor });
                        break;
                    case (int)MenuItemType.DateTimeListPage:
                        MenuPages.Add(id, new NavigationPage(new DateTimeListPage()){ BarBackgroundColor = ColorConstants.NavigationBarBackgroundColor, BarTextColor = ColorConstants.NavigationBarTextColor });
                        break;
                    case (int)MenuItemType.BarcodeListPage:
                        MenuPages.Add(id, new NavigationPage(new BarcodeListPage()){ BarBackgroundColor = ColorConstants.NavigationBarBackgroundColor, BarTextColor = ColorConstants.NavigationBarTextColor });
                        break;
                    case (int)MenuItemType.About:
                        MenuPages.Add(id, new NavigationPage(new AboutPage()){ BarBackgroundColor = ColorConstants.NavigationBarBackgroundColor, BarTextColor = ColorConstants.NavigationBarTextColor });
                        break;
                }
            }

            var newPage = MenuPages[id];

            if (newPage != null && Detail != newPage)
            {
                Detail = newPage;

                if (Device.RuntimePlatform == Device.Android)
                    await Task.Delay(100);

                IsPresented = false;
            }
        }
    }
}