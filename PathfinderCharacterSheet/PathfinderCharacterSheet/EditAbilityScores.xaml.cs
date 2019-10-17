﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PathfinderCharacterSheet
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EditAbilityScores : ContentPage
	{
        private List<CharacterSheet.IntModifier>[] tempAdjustments = new List<CharacterSheet.IntModifier>[(int)CharacterSheet.Ability.Total];
        private List<CharacterSheet.IntModifier>[] tempModifiers = new List<CharacterSheet.IntModifier>[(int)CharacterSheet.Ability.Total];
        private Grid[] modifierGrids = null;

        public EditAbilityScores()
		{
			InitializeComponent();
            CreateControls();
            UpdateView();
        }

        private void CreateControls()
        {
            if (AbilityScores.Children.Count > 0)
                return;
            var modifierFrames = new List<Frame>();
            for (var i = 0; i < (int)CharacterSheet.Ability.Total + 1; i++)
                for (var j = 0; j < 5; j++)
                {
                    View child = null;
                    if ((i <= 0) || (j <= 0))
                    {
                        child = new Label()
                        {
                            FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                            TextColor = Color.Black,
                            HorizontalTextAlignment = TextAlignment.Center,
                            VerticalTextAlignment = TextAlignment.Center,
                        };
                    }
                    else
                    {
                        if (j == 2)
                        {
                            child = new Frame()
                            {
                                Content = new Label()
                                {
                                    FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                                    TextColor = Color.Black,
                                    HorizontalOptions = LayoutOptions.Center,
                                    VerticalOptions = LayoutOptions.Center,
                                },
                                BorderColor = Color.Black,
                                BackgroundColor = Color.LightGray,
                                Padding = 5,
                            };
                            modifierFrames.Add(child as Frame);
                        }
                        else
                        {
                            child = new Frame()
                            {
                                Content = new Entry()
                                {
                                    FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                                    TextColor = Color.Black,
                                    Keyboard = Keyboard.Numeric,
                                    HorizontalTextAlignment = TextAlignment.Center,
                                    HorizontalOptions = LayoutOptions.FillAndExpand,
                                },
                                BorderColor = Color.Black,
                                Padding = 5,
                            };
                            var index = i;
                            ((child as Frame).Content as Entry).TextChanged += (s, e) => UpdateModifier(index);
                        }
                    }
                    if (i == 0)
                        AbilityScores.Children.Add(child, j, i);
                    else
                        AbilityScores.Children.Add(child, j, i * 2 - 1);
                }
            //(AbilityScores.Children[0] as Label).Text = "Ability Name";
            (AbilityScores.Children[1] as Label).Text = "Ability Score";
            (AbilityScores.Children[2] as Label).Text = "Ability Modifier";
            (AbilityScores.Children[3] as Label).Text = "Temp Adjustment";
            (AbilityScores.Children[4] as Label).Text = "Temp Modifier";
            var grids = new List<Grid>();
            for (var i = 0; i < (int)CharacterSheet.Ability.Total; i++)
            {
                var grid = new Grid()
                {
                    ColumnSpacing = 5,
                    RowSpacing = 5,
                    BackgroundColor = Color.LightGray,
                    IsVisible = false,
                };
                var index = (i + 1) * 2;
                AbilityScores.Children.Add(grid, 0, 5, index, index + 1);
                grids.Add(grid);
                var tgr = new TapGestureRecognizer();
                var frame = modifierFrames[i];
                var label = (frame.Content as Label);
                tgr.Tapped += (s, e) => ModifiersLabel_Tapped(grid, label, frame);
                label.GestureRecognizers.Add(tgr);
            }
            modifierGrids = grids.ToArray();
        }

        private void ModifiersLabel_Tapped(Grid g, Label l, Frame f)
        {
            var visible = !g.IsVisible;
            g.IsVisible = visible;
            if (g.IsVisible)
                MainPage.UpdateParentGrid(g);
            f.BackgroundColor = visible ? Color.LightGray : Color.White;
            l.TextDecorations = visible ? TextDecorations.None : TextDecorations.Underline;
        }

        private void UpdateModifier(int i)
        {
            var index = i * 5;
            if ((index + 5) > AbilityScores.Children.Count)
                return;
            var ab = new CharacterSheet.AbilityScore();
            var entry= ((AbilityScores.Children[index + 1] as Frame).Content as Entry);
            MainPage.StrToInt(entry.Text, ref ab.score);
            entry = ((AbilityScores.Children[index + 3] as Frame).Content as Entry);
            MainPage.StrToInt(entry.Text, ref ab.tempAdjustment);
            entry = ((AbilityScores.Children[index + 4] as Frame).Content as Entry);
            MainPage.StrToInt(entry.Text, ref ab.tempModifier);
            ((AbilityScores.Children[index + 2] as Frame).Content as Label).Text = ab.Modifier.ToString();
        }

        public void UpdateView()
        {
            var c = CharacterSheetStorage.Instance.selectedCharacter;
            if (c == null)
                return;
            var abilities = Enum.GetNames(typeof(CharacterSheet.Ability));
            var abilitiesCount = c.abilityScores.Length;
            for (var i = 0; i < abilitiesCount; i++)
            {
                var ab = c.abilityScores[i];
                var index = (i + 1) * 5;
                (AbilityScores.Children[index + 0] as Label).Text = abilities[i];
                ((AbilityScores.Children[index + 1] as Frame).Content as Entry).Text = ab.score.ToString();
                ((AbilityScores.Children[index + 2] as Frame).Content as Label).Text = ab.Modifier.ToString();
                ((AbilityScores.Children[index + 3] as Frame).Content as Entry).Text = ab.tempAdjustment.ToString();
                ((AbilityScores.Children[index + 4] as Frame).Content as Entry).Text = ab.tempModifier.ToString();
                MainPage.FillIntModifierGrid(modifierGrids[i], ab.tempAdjustments, "Temp Ajustments:", EditIntModifier, EditIntModifier, (mods, mod) => UpdateModifier(i + 1));
            }
        }

        private void EditIntModifier(List<CharacterSheet.IntModifier> modifiers)
        {
            EditIntModifier(modifiers, null);
        }

        private void EditIntModifier(List<CharacterSheet.IntModifier> modifiers, CharacterSheet.IntModifier mod)
        {
            var page = new EditIntModifier();
            page.Init(modifiers, mod);
            Navigation.PushAsync(page);
        }

        private void EditToView()
        {
            var c = CharacterSheetStorage.Instance.selectedCharacter;
            if (c == null)
                return;
            var anyChanged = false;
            var abilitiesCount = c.abilityScores.Length;
            for (var i = 0; i < abilitiesCount; i++)
            {
                var ab = c.abilityScores[i];
                var index = (i + 1) * 5;
                anyChanged |= MainPage.StrToInt(((AbilityScores.Children[index + 1] as Frame).Content as Entry).Text, ref ab.score);
                anyChanged |= MainPage.StrToInt(((AbilityScores.Children[index + 3] as Frame).Content as Entry).Text, ref ab.tempAdjustment);
                anyChanged |= MainPage.StrToInt(((AbilityScores.Children[index + 4] as Frame).Content as Entry).Text, ref ab.tempModifier);
            }
            if (anyChanged)
                CharacterSheetStorage.Instance.SaveCharacter(CharacterSheetStorage.Instance.selectedCharacter);
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