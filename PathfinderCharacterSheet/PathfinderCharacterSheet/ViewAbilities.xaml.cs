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
	public partial class ViewAbilities : ContentPage, ISheetView
	{
		public ViewAbilities ()
		{
			InitializeComponent ();
        }

        public void UpdateView()
        {
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;

            const int columns = 4;
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
            (AbilityScores.Children[abscindex++] as Label).Text = "Temp Modifier";
            var abilities = Enum.GetNames(typeof(CharacterSheet.Ability));
            var abilitiesCount = sheet.abilityScores.Length;
            for (var i = 0; i < abilitiesCount; i++)
            {
                var ab = sheet.abilityScores[i];
                abscindex = (i + 1) * columns;
                (AbilityScores.Children[abscindex++] as Label).Text = abilities[i] + ":";
                ((AbilityScores.Children[abscindex++] as Frame).Content as Label).Text = ab.score.GetTotal(sheet).ToString();
                ((AbilityScores.Children[abscindex++] as Frame).Content as Label).Text = ab.GetModifier(sheet).ToString();
                //((AbilityScores.Children[abscindex++] as Frame).Content as Label).Text = ab.tempAdjustment.ToString();
                ((AbilityScores.Children[abscindex++] as Frame).Content as Label).Text = ab.GetTempModifier(sheet).ToString();
            }

            MaxHP.Text = sheet.hp.maxHP.GetTotal(sheet).ToString();
            HP.Text = sheet.hp.hp.GetTotal(sheet).ToString();
            DamageResist.Text = sheet.hp.damageResist.GetTotal(sheet).ToString();
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
            Initiative.Text = sheet.CurrentInitiative.ToString();
            //InitiativeTotal.Text = c.CurrentInitiative.ToString();
            //InitiativeDexMod.Text = c.CurrentAbilityModifier(CharacterSheet.Ability.Dexterity).ToString();
            //InitiativeMiscMod.Text = CharacterSheet.Sum(c.initiative.miscModifiers).ToString();
            ArmorClass.Text = sheet.ACTotal.ToString();
            TouchAC.Text = sheet.ACTouch.ToString();
            FlatFootedAC.Text = sheet.ACFlatFooted.ToString();

            Fortitude.Text = sheet.GetSavingThrowTotal(CharacterSheet.Save.Fortitude).ToString();
            Reflex.Text = sheet.GetSavingThrowTotal(CharacterSheet.Save.Reflex).ToString();
            Will.Text = sheet.GetSavingThrowTotal(CharacterSheet.Save.Will).ToString();

            BaseAttackBonus.Text = sheet.baseAttackBonus.GetTotal(sheet).ToString();

            SpellResistance.Text = sheet.spellResistance.GetTotal(sheet).ToString();

            CMB.Text = sheet.CMB.ToString();
            CMD.Text = sheet.CMD.ToString();

            var baseSpeed = sheet.speed.baseSpeed.GetTotal(sheet);
            BaseSpeed.Text = baseSpeed.ToString() + " ft (" + CharacterSheet.Speed.InSquares(baseSpeed) + " sq)";
            var speedWithArmor = sheet.speed.armorSpeed.GetTotal(sheet);
            SpeedWithArmor.Text = speedWithArmor.ToString() + " ft (" + CharacterSheet.Speed.InSquares(speedWithArmor) + " sq)";
            FlySpeed.Text = sheet.speed.flySpeed.GetTotal(sheet).ToString() + " ft";
            Maneuverability.Text = sheet.speed.maneuverability.GetTotal(sheet).ToString();
            SwimSpeed.Text = sheet.speed.GetSwimSpeed(sheet).ToString() + " ft";
            ClimbSpeed.Text = sheet.speed.GetClimbSpeed(sheet).ToString() + " ft";
            BurrowSpeed.Text = sheet.speed.burrowSpeed.GetTotal(sheet).ToString() + " ft";
        }

        private void AbilityScores_DoubleTapped(object sender, EventArgs e)
        {
            Navigation.PushAsync(new EditAbilityScores());
        }

        private void MaxHP_DoubleTapped(object sender, EventArgs e)
        {
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, sheet.hp.maxHP, "Edit Max HP", "Max HP: ", true);
            Navigation.PushAsync(eivwm);
        }

        private void HP_DoubleTapped(object sender, EventArgs e)
        {
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, sheet.hp.hp, "Edit HP", "HP: ", true);
            Navigation.PushAsync(eivwm);
        }

        private void DamageResist_DoubleTapped(object sender, EventArgs e)
        {
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, sheet.hp.damageResist, "Edit Damage Resist", "Damage Resist: ", true);
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
            Navigation.PushAsync(new EditSavingThrows());
        }

        private void BaseAttackBonus_DoubleTapped(object sender, EventArgs e)
        {
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, sheet.baseAttackBonus, "Edit Base Attack Bonus", "Base Attack Bonus: ", true);
            Navigation.PushAsync(eivwm);
        }

        private void SpellResistance_DoubleTapped(object sender, EventArgs e)
        {
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet,  sheet.spellResistance, "Edit Spell Resistance", "Spell Resistance: ", true);
            Navigation.PushAsync(eivwm);
        }

        private void CombatManeuver_DoubleTapped(object sender, EventArgs e)
        {
            Navigation.PushAsync(new EditCombatManeuvers());
        }

        /*
        private void AttackBonus_DoubleTapped(object sender, EventArgs e)
        {
            var c = CharacterSheetStorage.Instance.selectedCharacter;
            if (c == null)
                return;
        }
        */

        private void Speed_DoubleTapped(object sender, EventArgs e)
        {
            var c = CharacterSheetStorage.Instance.selectedCharacter;
            if (c == null)
                return;
            Navigation.PushAsync(new EditSpeed());
        }
    }
}