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
    public partial class CharacterSheetTabs : TabbedPage, ISheetView
    {
        public CharacterSheetTabs()
        {
            InitializeComponent();
        }

        public void UpdateView()
        {
            var c = CharacterSheetStorage.Instance.selectedCharacter;
            CharacterName.Text = c.Name;
            Alignment.Text = c.alignment.ToString();
            Experience.Text = c.experience.ToString();
            NextLevel.Text = c.nextLevelExperience.ToString();
            var level = string.Empty;
            if (c.levelOfClass != null)
                foreach (var loc in c.levelOfClass)
                {
                    if (level.Length > 0)
                        level += ", ";
                    level += string.Format("({0}) {1}", loc.level, loc.className);
                }
            Level.Text = level;
            Deity.Text = c.deity;
            Homeland.Text = c.homeland;
            Race.Text = c.Race;
            Size.Text = c.size;
            Gender.Text = c.gender;
            Age.Text = c.age.ToString();
            CharacterHeight.Text = c.height.ToString();
            Weight.Text = c.weight.ToString();
            Hair.Text = c.hair;
            Eyes.Text = c.eyes;
            Biography.Text = c.biography;

            const int columns = 3;
            if (AbilityScores.Children.Count <= 0)
            {
                for (var i = 0; i < (int)CharacterSheet.Ability.Total + 1; i++)
                    for (var j = 0; j < columns; j++)
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
                            child = new Frame()
                            {
                                Content = new Label()
                                {
                                    FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                                    TextColor = Color.Black,
                                    VerticalTextAlignment = TextAlignment.Center,
                                    HorizontalTextAlignment = TextAlignment.Center,
                                },
                                BorderColor = Color.Black,
                                Padding = 5,
                            };
                        }
                        AbilityScores.Children.Add(child, j, i);
                    }
            }
            var abscindex = 0;
            (AbilityScores.Children[abscindex++] as Label).Text = "Ability Name";
            (AbilityScores.Children[abscindex++] as Label).Text = "Ability Score";
            (AbilityScores.Children[abscindex++] as Label).Text = "Ability Modifier";
            //(AbilityScores.Children[abscindex++] as Label).Text = "Temp Adjustment";
            //(AbilityScores.Children[abscindex++] as Label).Text = "Temp Modifier";
            var abilities = Enum.GetNames(typeof(CharacterSheet.Ability));
            var abilitiesCount = c.abilityScores.Length;
            for (var i = 0; i < abilitiesCount; i++)
            {
                var ab = c.abilityScores[i];
                abscindex = (i + 1) * columns;
                (AbilityScores.Children[abscindex++] as Label).Text = abilities[i];
                ((AbilityScores.Children[abscindex++] as Frame).Content as Label).Text = ab.score.ToString();
                ((AbilityScores.Children[abscindex++] as Frame).Content as Label).Text = ab.Modifier.ToString();
                //((AbilityScores.Children[abscindex++] as Frame).Content as Label).Text = ab.tempAdjustment.ToString();
                //((AbilityScores.Children[abscindex++] as Frame).Content as Label).Text = ab.tempModifier.ToString();
            }

            //MaxHP.Text = c.hp.maxHP.ToString();
            HP.Text = c.hp.CurrentHP.ToString() + "/" + c.hp.maxHP.ToString();
            DamageResist.Text = c.hp.damageResist.ToString();
            /*
            while (TempHPModifiers.Children.Count / 3 < c.hp.tempHP.Count)
            {
                var row = TempHPModifiers.Children.Count / 3;
                TempHPModifiers.Children.Add(new CheckBox()
                {
                    VerticalOptions = LayoutOptions.Center,
                }, 0, row);
                TempHPModifiers.Children.Add(new Frame()
                {
                    Content = new Label()
                    {
                        FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                        TextColor = Color.Black,
                        HorizontalTextAlignment = TextAlignment.Center,
                        VerticalOptions = LayoutOptions.Center,
                    },
                    BorderColor = Color.Black,
                    Padding = 5,
                }, 1, row);
                TempHPModifiers.Children.Add(new Label()
                {
                    FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                    TextColor = Color.Black,
                    HorizontalTextAlignment = TextAlignment.Start,
                    VerticalOptions = LayoutOptions.Center,
                }, 2, row);
            }
            for (var i = 0; i < c.hp.tempHP.Count; i++)
            {
                var im = c.hp.tempHP[i];
                var index = i * 3;
                (TempHPModifiers.Children[index + 0] as CheckBox).IsChecked = im.IsActive;
                ((TempHPModifiers.Children[index + 1] as Frame).Content as Label).Text = im.Value.ToString();
                (TempHPModifiers.Children[index + 2] as Label).Text = im.Name;
            }
            */
            Initiative.Text = c.CurrentInitiative.ToString();
            //InitiativeTotal.Text = c.CurrentInitiative.ToString();
            //InitiativeDexMod.Text = c.CurrentAbilityModifier(CharacterSheet.Ability.Dexterity).ToString();
            //InitiativeMiscMod.Text = CharacterSheet.Sum(c.initiative.miscModifiers).ToString();

            ACTotal.Text = c.ACTotal.ToString();
            ACArmorBonus.Text = c.ACArmorBonus.ToString();
            ACShieldBonus.Text = c.ACShieldBonus.ToString();
            ACDexModifier.Text = c.CurrentAbilityModifier(CharacterSheet.Ability.Dexterity).ToString();
            ACSizeModifier.Text = c.armorClass.sizeModifier.ToString();
            ACNaturalArmor.Text = c.armorClass.naturalArmor.ToString();
            ACDeflectionModifier.Text = c.armorClass.deflectionModifier.ToString();
            ACMiscModifier.Text = c.ACMiscModifier.ToString();
        }

        private void AbilityScores_DoubleTapped(object sender, EventArgs e)
        {
            Navigation.PushAsync(new EditAbilityScores());
        }

        private void HP_DoubleTapped(object sender, EventArgs e)
        {
            Navigation.PushAsync(new EditHP());
        }

        private void Initiative_DoubleTapped(object sender, EventArgs e)
        {

        }
    }
}