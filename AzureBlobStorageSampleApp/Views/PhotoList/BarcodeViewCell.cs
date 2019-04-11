using Xamarin.Forms;

using AzureBlobStorageSampleApp.Shared;

namespace AzureBlobStorageSampleApp
{
    public class BarcodeViewCell : ViewCell
    {
        public BarcodeViewCell()
        {
            var photo = new Image
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Margin = new Thickness(0, 5),
                Aspect = Aspect.AspectFit,
                BackgroundColor = ColorConstants.PageBackgroundColor
            };
            photo.SetBinding(Image.SourceProperty, nameof(PhotoModel.Url), BindingMode.Default, new CheckAndModifyLocalFiles());


            var title = new Label { VerticalTextAlignment = TextAlignment.Center };
            title.SetBinding(Label.TextProperty, nameof(PhotoModel.BarcodeString),  BindingMode.Default, new AddBarcodeAndNoBarcodeWordConverter());


            var grid = new Grid
            {
                Margin = new Thickness(10, 0),
                ColumnSpacing = 10,
                RowDefinitions = {
                    new RowDefinition { Height = new GridLength(200, GridUnitType.Absolute) },},

                ColumnDefinitions = {
                    new ColumnDefinition { Width = new GridLength(150, GridUnitType.Absolute) },

                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },}
            };
            grid.Children.Add(photo, 0, 0);
            grid.Children.Add(title, 1, 0);

            View = grid;
        }
    }

    public class IsBarcodeOrNotBarcodeViewCell : ViewCell
    {
        public IsBarcodeOrNotBarcodeViewCell()
        {
            var photo = new Image
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Margin = new Thickness(0, 5),
                Aspect = Aspect.AspectFit,
                BackgroundColor = ColorConstants.PageBackgroundColor
            };
            photo.SetBinding(Image.SourceProperty, nameof(PhotoModel.Url), BindingMode.Default, new CheckAndModifyLocalFiles());


            var title = new Label { VerticalTextAlignment = TextAlignment.Center };
            title.SetBinding(Label.TextProperty, nameof(PhotoModel.BarcodeString),  BindingMode.Default, new IsBarcodeAndNoBarcodeWordConverter());


            var grid = new Grid
            {
                Margin = new Thickness(10, 0),
                ColumnSpacing = 10,
                RowDefinitions = {
                    new RowDefinition { Height = new GridLength(200, GridUnitType.Absolute) },},

                ColumnDefinitions = {
                    new ColumnDefinition { Width = new GridLength(150, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },}
            };
            grid.Children.Add(photo, 0, 0);
            grid.Children.Add(title, 1, 0);

            View = grid;
        }
    }

}

