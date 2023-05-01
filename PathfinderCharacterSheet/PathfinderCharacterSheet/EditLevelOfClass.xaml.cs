using PathfinderCharacterSheet.CharacterSheets.V1;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PathfinderCharacterSheet
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EditLevelOfClass : ContentPage, ISheetView
	{
        private Page pushedPage = null;
        private LevelOfClassList levelOfClass = null;
        private LevelOfClassList currentLevelOfClass = null;

        public EditLevelOfClass ()
		{
			InitializeComponent ();
		}

        public void Init(LevelOfClassList levelOfClass)
        {
            this.levelOfClass = levelOfClass;
            currentLevelOfClass = levelOfClass?.Clone;
            UpdateView();
        }

        public void UpdateView()
        {
            pushedPage = null;
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            var grid = LevelOfClass;
            grid.Children.Clear();
            var stack = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };
            var gridTitle = UIHelpers.CreateLabel("Level Of Class", TextAlignment.Center);
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
                addButton.Clicked += (s, e) => EditLevel();
                stack.Children.Add(addButton);
            }
            grid.Children.Add(stack, 0, 2, 0, 1);
            var count = currentLevelOfClass.Count;
            if (count <= 0)
                return;
            grid.Children.Add(UIHelpers.CreateLabel("Level", TextAlignment.Center), 0, 1);
            grid.Children.Add(UIHelpers.CreateLabel("Class", TextAlignment.Center), 1, 1);
            for (var i = 0; i < count; i++)
            {
                var index = i + 2;
                var loc = currentLevelOfClass[i];
                var value = UIHelpers.CreateFrame(loc.GetValue(sheet).ToString());
                var valueTapped = new TapGestureRecognizer();
                valueTapped.Tapped += (s, e) => EditLevel(loc);
                value.GestureRecognizers.Add(valueTapped);
                grid.Children.Add(value, 0, index);
                var name = UIHelpers.CreateFrame(loc.ClassName);
                var nameTapped = new TapGestureRecognizer();
                nameTapped.Tapped += (s, e) => EditLevel(loc);
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

        private void EditLevel(LevelOfClass level = null)
        {
            if (pushedPage != null)
                return;
            var page = new EditLevel();
            page.Init(currentLevelOfClass, level);
            pushedPage = page;
            Navigation.PushAsync(pushedPage);
        }

        private void Cancel_Clicked(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            pushedPage = this;
            Navigation.PopAsync();
        }

        private void Save_Clicked(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            pushedPage = this;
            EditToView();
            Navigation.PopAsync();
        }
    }
}