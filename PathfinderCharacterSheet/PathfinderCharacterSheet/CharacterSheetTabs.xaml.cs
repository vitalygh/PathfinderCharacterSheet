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
            if (c == null)
                return;
            CharacterName.Text = c.Name;
            Alignment.Text = c.alignment.ToString();
            Experience.Text = c.experience.ToString();
            NextLevel.Text = c.nextLevelExperience.ToString();
            Level.Text = c.LevelAsString;
            Deity.Text = c.deity;
            Homeland.Text = c.homeland;
            Race.Text = c.Race;
            Size.Text = c.size;
            Gender.Text = c.gender;
            Age.Text = c.age;
            CharacterHeight.Text = c.height;
            Weight.Text = c.weight;
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
                                HorizontalTextAlignment = (j == 0) ? TextAlignment.Start : TextAlignment.Center,
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
            var abscindex = 1;
            //(AbilityScores.Children[abscindex++] as Label).Text = "Ability Name";
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
                (AbilityScores.Children[abscindex++] as Label).Text = abilities[i] + ":";
                ((AbilityScores.Children[abscindex++] as Frame).Content as Label).Text = ab.score.ToString();
                ((AbilityScores.Children[abscindex++] as Frame).Content as Label).Text = ab.Modifier.ToString();
                //((AbilityScores.Children[abscindex++] as Frame).Content as Label).Text = ab.tempAdjustment.ToString();
                //((AbilityScores.Children[abscindex++] as Frame).Content as Label).Text = ab.tempModifier.ToString();
            }

            MaxHP.Text = c.hp.maxHP.Total.ToString();
            HP.Text = c.hp.hp.Total.ToString();
            DamageResist.Text = c.hp.damageResist.Total.ToString();
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
            ArmorClass.Text = c.ACTotal.ToString();
            TouchAC.Text = c.ACTouch.ToString();
            FlatFootedAC.Text = c.ACFlatFooted.ToString();

            Fortitude.Text = c.GetSavingThrowTotal(CharacterSheet.Save.Fortitude).ToString();
            Reflex.Text = c.GetSavingThrowTotal(CharacterSheet.Save.Reflex).ToString();
            Will.Text = c.GetSavingThrowTotal(CharacterSheet.Save.Will).ToString();

            BaseAttackBonus.Text = c.baseAttackBonus.Total.ToString();

            SpellResistance.Text = c.spellResistance.Total.ToString();

            CMB.Text = c.CMB.ToString();
            CMD.Text = c.CMD.ToString();

            MeleeAttackBonus.Text = c.MeleeAttackBonus.ToString();
            MeleeDamageBonus.Text = c.MeleeDamageBonus.ToString();
            RangeAttackBonus.Text = c.RangeAttackBonus.ToString();
        }

        private void Biography_DoubleTapped(object sender, EventArgs e)
        {
            Navigation.PushAsync(new EditBackground());
        }

        private void AbilityScores_DoubleTapped(object sender, EventArgs e)
        {
            Navigation.PushAsync(new EditAbilityScores());
        }

        private void MaxHP_DoubleTapped(object sender, EventArgs e)
        {
            var c = CharacterSheetStorage.Instance.selectedCharacter;
            if (c == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(c.hp.maxHP, "Edit Max HP", "Max HP: ", true);
            Navigation.PushAsync(eivwm);
        }

        private void HP_DoubleTapped(object sender, EventArgs e)
        {
            var c = CharacterSheetStorage.Instance.selectedCharacter;
            if (c == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(c.hp.hp, "Edit HP", "HP: ", true);
            Navigation.PushAsync(eivwm);
        }

        private void DamageResist_DoubleTapped(object sender, EventArgs e)
        {
            var c = CharacterSheetStorage.Instance.selectedCharacter;
            if (c == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(c.hp.damageResist, "Edit Damage Resist", "Damage Resist: ", true);
            Navigation.PushAsync(eivwm);
        }

        private void Initiative_DoubleTapped(object sender, EventArgs e)
        {
            var c = CharacterSheetStorage.Instance.selectedCharacter;
            if (c == null)
                return;
            Navigation.PushAsync(new EditInitiative());
        }

        private void ArmorClass_DoubleTapped(object sender, EventArgs e)
        {
            var c = CharacterSheetStorage.Instance.selectedCharacter;
            if (c == null)
                return;
            Navigation.PushAsync(new EditArmorClass());
        }

        private void SavingThrows_DoubleTapped(object sender, EventArgs e)
        {
            var c = CharacterSheetStorage.Instance.selectedCharacter;
            if (c == null)
                return;

        }

        private void BaseAttackBonus_DoubleTapped(object sender, EventArgs e)
        {
            var c = CharacterSheetStorage.Instance.selectedCharacter;
            if (c == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(c.baseAttackBonus, "Edit Base Attack Bonus", "Base Attack Bonus: ", true);
            Navigation.PushAsync(eivwm);
        }

        private void SpellResistance_DoubleTapped(object sender, EventArgs e)
        {
            var c = CharacterSheetStorage.Instance.selectedCharacter;
            if (c == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(c.spellResistance, "Edit Spell Resistance", "Spell Resistance: ", true);
            Navigation.PushAsync(eivwm);
        }

        private void CombatManeuver_DoubleTapped(object sender, EventArgs e)
        {
            var c = CharacterSheetStorage.Instance.selectedCharacter;
            if (c == null)
                return;

        }
    }
}