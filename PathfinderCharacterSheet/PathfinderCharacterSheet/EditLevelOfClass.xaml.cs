using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PathfinderCharacterSheet
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EditLevelOfClass : ContentPage, ISheetView
	{
        private List<CharacterSheet.LevelOfClass> levelOfClass = null;
        private List<CharacterSheet.LevelOfClass> currentLevelOfClass = new List<CharacterSheet.LevelOfClass>();

        public EditLevelOfClass ()
		{
			InitializeComponent ();
		}

        public void Init(List<CharacterSheet.LevelOfClass> levelOfClass)
        {
            this.levelOfClass = levelOfClass;
            currentLevelOfClass = CharacterSheet.LevelOfClass.CreateClone(levelOfClass);
            UpdateView();
        }

        public void UpdateView()
        {
            var grid = LevelOfClass;
            grid.Children.Clear();
            var stack = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };
            var gridTitle = new Label()
            {
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                TextColor = Color.Black,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Start,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Text = "Level Of Class:",
            };
            stack.Children.Add(gridTitle);
            if (gridTitle != null)
            {
                var addButton = new Button()
                {
                    FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                    TextColor = Color.Black,
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    HorizontalOptions = LayoutOptions.End,
                    Text = "Add",
                };
                addButton.Clicked += (s, e) => EditLevel(currentLevelOfClass);
                stack.Children.Add(addButton);
            }
            grid.Children.Add(stack, 0, 2, 0, 1);
            var count = currentLevelOfClass.Count;
            if (count <= 0)
                return;
            grid.Children.Add(new Label()
            {
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                TextColor = Color.Black,
                BackgroundColor = Color.LightGray,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                Text = "Level",
            }, 0, 1);
            grid.Children.Add(new Label()
            {
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                TextColor = Color.Black,
                BackgroundColor = Color.LightGray,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                Text = "Class",
            }, 1, 1);
            for (var i = 0; i < count; i++)
            {
                var index = i + 2;
                var loc = currentLevelOfClass[i];
                var value = new Frame()
                {
                    Content = new Label()
                    {
                        FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                        TextColor = Color.Black,
                        VerticalTextAlignment = TextAlignment.Center,
                        HorizontalTextAlignment = TextAlignment.Center,
                        Text = loc.Level.ToString(),
                    },
                    BorderColor = Color.Black,
                    Padding = 5,
                };
                var valueTapped = new TapGestureRecognizer();
                valueTapped.Tapped += (s, e) => EditLevel(currentLevelOfClass, loc);
                value.GestureRecognizers.Add(valueTapped);
                grid.Children.Add(value, 0, index);
                var name = new Frame()
                {
                    Content = new Label()
                    {
                        FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                        TextColor = Color.Black,
                        VerticalTextAlignment = TextAlignment.Center,
                        HorizontalTextAlignment = TextAlignment.Center,
                        Text = loc.ClassName,
                    },
                    BorderColor = Color.Black,
                    Padding = 5,
                };
                var nameTapped = new TapGestureRecognizer();
                nameTapped.Tapped += (s, e) => EditLevel(currentLevelOfClass, loc);
                name.GestureRecognizers.Add(nameTapped);
                grid.Children.Add(name, 1, index);
            }
        }

        private void EditToView()
        {
            levelOfClass.Clear();
            foreach (var l in currentLevelOfClass)
                levelOfClass.Add(l);
        }

        private void EditLevel(List<CharacterSheet.LevelOfClass> levelOfClass)
        {
            EditLevel(currentLevelOfClass, null);
        }

        private void EditLevel(List<CharacterSheet.LevelOfClass> levelOfClass, CharacterSheet.LevelOfClass level)
        {
            var page = new EditLevel();
            page.Init(levelOfClass, level);
            Navigation.PushAsync(page);
        }

        private void Cancel_Clicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        private void Save_Clicked(object sender, EventArgs e)
        {
            EditToView();
            Navigation.PopAsync();
        }
    }
}