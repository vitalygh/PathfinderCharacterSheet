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
	public partial class EditAbilityScores : ContentPage, ISheetView
	{
        private CharacterSheet localSheet = new CharacterSheet();

        public EditAbilityScores()
		{
			InitializeComponent();
            InitAbilityScores();
            CreateControls();
            UpdateView();
        }

        private void InitAbilityScores()
        {
            var asList = new List<CharacterSheet.AbilityScore>();
            foreach (var absc in CharacterSheetStorage.Instance.selectedCharacter.abilityScores)
                asList.Add(absc.Clone as CharacterSheet.AbilityScore);
            localSheet.abilityScores = asList.ToArray();
        }

        private void CreateControls()
        {
            if (AbilityScores.Children.Count > 0)
                return;
            var abilities = Enum.GetNames(typeof(CharacterSheet.Ability));
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
                        //if (j != 1)
                        {
                            var readOnly = j % 2 == 0;
                            child = new Frame()
                            {
                                Content = new Label()
                                {
                                    FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                                    TextColor = Color.Black,
                                    HorizontalOptions = LayoutOptions.Center,
                                    VerticalOptions = LayoutOptions.Center,
                                    TextDecorations = readOnly ? TextDecorations.None : TextDecorations.Underline,
                                },
                                BorderColor = Color.Black,
                                BackgroundColor = readOnly ? Color.LightGray : Color.White,
                                Padding = 5,
                            };
                            if (!readOnly)
                            {
                                var tgr = new TapGestureRecognizer()
                                {
                                    NumberOfTapsRequired = 1,
                                };
                                var index = i - 1;
                                var adj = j == 3;
                                tgr.Tapped += (s, e) =>
                                {
                                    var eivwm = new EditIntValueWithModifiers();
                                    var modname = (adj ? "Temp Adjustment" : "Score");
                                    var abmodname = abilities[index] + " " + modname;
                                    var labs = localSheet.abilityScores[index];
                                    var vwm = adj ? labs.tempAdjustment : labs.score;
                                    eivwm.Init(localSheet, vwm, "Edit " + abmodname, modname + ": ", false);
                                    Navigation.PushAsync(eivwm);
                                };
                                (child as Frame).GestureRecognizers.Add(tgr);
                            }
                        }
                        /*
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
                        */
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
        }

        private void UpdateModifier(int i)
        {
            var index = i * 5;
            if ((index + 5) > AbilityScores.Children.Count)
                return;
            var ab = localSheet.abilityScores[i - 1];
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            //var entry= ((AbilityScores.Children[index + 1] as Frame).Content as Entry);
            //MainPage.StrToInt(entry.Text, ref ab.score);
            //((AbilityScores.Children[index + 1] as Frame).Content as Label).Text = ab.score.Total.ToString();
            ((AbilityScores.Children[index + 2] as Frame).Content as Label).Text = ab.GetModifier(sheet).ToString();
            //((AbilityScores.Children[index + 3] as Frame).Content as Label).Text = ab.tempAdjustment.Total.ToString();
            ((AbilityScores.Children[index + 4] as Frame).Content as Label).Text = ab.GetTempModifier(sheet).ToString();
        }

        public void UpdateView()
        {
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var abilities = Enum.GetNames(typeof(CharacterSheet.Ability));
            var abilitiesCount = localSheet.abilityScores.Length;
            for (var i = 0; i < abilitiesCount; i++)
            {
                var ab = localSheet.abilityScores[i];
                var index = (i + 1) * 5;
                (AbilityScores.Children[index + 0] as Label).Text = abilities[i];
                ((AbilityScores.Children[index + 1] as Frame).Content as Label).Text = ab.score.GetTotal(sheet).ToString();
                ((AbilityScores.Children[index + 2] as Frame).Content as Label).Text = ab.GetModifier(sheet).ToString();
                ((AbilityScores.Children[index + 3] as Frame).Content as Label).Text = ab.tempAdjustment.GetTotal(sheet).ToString();
                ((AbilityScores.Children[index + 4] as Frame).Content as Label).Text = ab.GetTempModifier(sheet).ToString();
            }
        }

        private void EditToView()
        {
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var anyChanged = false;
            var abilitiesCount = localSheet.abilityScores.Length;
            for (var i = 0; i < abilitiesCount; i++)
            {
                if (sheet.abilityScores[i].Equals(localSheet.abilityScores[i]))
                    continue;
                anyChanged = true;
                sheet.abilityScores[i] = localSheet.abilityScores[i];
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