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
	public partial class EditHP : ContentPage
	{
        private List<CharacterSheet.IntModifier> tempHP = null;

		public EditHP ()
		{
			InitializeComponent ();
            ViewToEdit();
        }

        private void ViewToEdit()
        {
            var c = CharacterSheetStorage.Instance.selectedCharacter;
            tempHP = new List<CharacterSheet.IntModifier>();
            foreach (var mod in c.hp.tempHP)
                tempHP.Add(new CharacterSheet.IntModifier()
                {
                    active = mod.IsActive,
                    value = mod.Value,
                    name = mod.Name,
                });
            MaxHP.Text = c.hp.maxHP.ToString();
            HP.Text = c.hp.hp.ToString();
            DamageResist.Text = c.hp.damageResist.ToString();
            ModifiersToEdit();
        }

        public void ModifiersToEdit()
        {
            TempHPModifiers.Children.Clear();
            var stack = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };
            var tempHPTitle = new Label()
            {
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                TextColor = Color.Black,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Start,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Text = "Temp HP Modifiers:",
            };
            stack.Children.Add(tempHPTitle);
            var addTempHPModifierButton = new Button()
            {
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                TextColor = Color.Black,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.End,
                Text = "Add",
            };
            addTempHPModifierButton.Clicked += TempHPModifierAddButton_Clicked;
            stack.Children.Add(addTempHPModifierButton);
            TempHPModifiers.Children.Add(stack, 0, 3, 0, 1);
            var count = tempHP.Count;
            if (count <= 0)
                return;
            TempHPModifiers.Children.Add(new Label()
            {
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                TextColor = Color.Black,
                BackgroundColor = Color.LightGray,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                Text = "Active",
            }, 0, 1);
            TempHPModifiers.Children.Add(new Label()
            {
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                TextColor = Color.Black,
                BackgroundColor = Color.LightGray,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                Text = "Value",
            }, 1, 1);
            TempHPModifiers.Children.Add(new Label()
            {
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                TextColor = Color.Black,
                BackgroundColor = Color.LightGray,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                Text = "Name",
            }, 2, 1);
            for (var i = 0; i < count; i++)
            {
                var index = i + 2;
                var t = tempHP[i];
                var active = new CheckBox()
                {
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                    IsChecked = t.IsActive,
                };
                active.CheckedChanged += (s, e) => t.active = active.IsChecked;
                TempHPModifiers.Children.Add(active, 0, index);
                var value = new Frame()
                {
                    Content = new Label()
                    {
                        FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                        TextColor = Color.Black,
                        VerticalTextAlignment = TextAlignment.Center,
                        HorizontalTextAlignment = TextAlignment.Center,
                        Text = t.Value.ToString(),
                    },
                    BorderColor = Color.Black,
                    Padding = 5,
                };
                var valueTapped = new TapGestureRecognizer();
                valueTapped.Tapped += (s, e) => EditIntModifier(t);
                value.GestureRecognizers.Add(valueTapped);
                TempHPModifiers.Children.Add(value, 1, index);
                var name = new Frame()
                {
                    Content = new Label()
                    {
                        FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                        TextColor = Color.Black,
                        VerticalTextAlignment = TextAlignment.Center,
                        HorizontalTextAlignment = TextAlignment.Center,
                        Text = t.Name.ToString(),
                    },
                    BorderColor = Color.Black,
                    Padding = 5,
                };
                var nameTapped = new TapGestureRecognizer();
                nameTapped.Tapped += (s, e) => EditIntModifier(t);
                name.GestureRecognizers.Add(nameTapped);
                TempHPModifiers.Children.Add(name, 2, index);
            }
        }

        private void EditToView()
        {
            var c = CharacterSheetStorage.Instance.selectedCharacter;
            var anyChanged = false;
            anyChanged |= MainPage.StrToInt(MaxHP.Text, ref c.hp.maxHP);
            anyChanged |= MainPage.StrToInt(HP.Text, ref c.hp.hp);
            anyChanged |= MainPage.StrToInt(DamageResist.Text, ref c.hp.damageResist);
            if (!CharacterSheet.IsEqual(c.hp.tempHP, tempHP))
            {
                anyChanged = true;
                c.hp.tempHP = tempHP;
            }
            if (anyChanged)
                CharacterSheetStorage.Instance.SaveCharacter(CharacterSheetStorage.Instance.selectedCharacter);
        }

        private void TempHPModifierAddButton_Clicked(object sender, EventArgs e)
        {
            EditIntModifier(null);
        }

        private void EditIntModifier(CharacterSheet.IntModifier mod)
        {
            var page = new EditIntModifier();
            page.Init(tempHP, mod);
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