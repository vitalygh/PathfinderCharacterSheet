using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Windows.Input;

namespace PathfinderCharacterSheet
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ViewAbilities : ContentPage, ISheetView
	{
        private Page pushedPage = null;

        public ViewAbilities ()
		{
			InitializeComponent ();
            CreateControls();
        }

        private void CreateControls()
        {
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
        }

        public void UpdateView()
        {
            pushedPage = null;

            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;

            const int columns = 4;
            var abilities = Enum.GetNames(typeof(CharacterSheet.Ability));
            var abilitiesCount = sheet.abilityScores.Length;
            var hasChanges = false;
            for (var i = 0; i < abilitiesCount; i++)
            {
                var ab = sheet.abilityScores[i];
                if (ab == null)
                {
                    ab = new CharacterSheet.AbilityScore();
                    sheet.abilityScores[i] = ab;
                    hasChanges = true;
                }
                var abscindex = (i + 1) * columns;
                (AbilityScores.Children[abscindex++] as Label).Text = abilities[i] + ":";
                ((AbilityScores.Children[abscindex++] as Frame).Content as Label).Text = ab.score.GetTotal(sheet).ToString();
                var modValue = ab.GetModifier(sheet);
                ((AbilityScores.Children[abscindex++] as Frame).Content as Label).Text = modValue.ToString();
                //((AbilityScores.Children[abscindex++] as Frame).Content as Label).Text = ab.tempAdjustment.ToString();
                var temp = ((AbilityScores.Children[abscindex++] as Frame).Content as Label);
                var tempValue = ab.GetTempModifier(sheet);
                temp.Text = tempValue.ToString();
                if (tempValue > modValue)
                    temp.TextColor = Color.Green;
                else if (tempValue < modValue)
                    temp.TextColor = Color.Red;
                else
                    temp.TextColor = Color.Black;
            }
            if (hasChanges)
                CharacterSheetStorage.Instance.SaveCharacter();
            MaxHP.Text = sheet.hp.maxHP.GetTotal(sheet).ToString();
            HP.Text = sheet.hp.hp.GetTotal(sheet).ToString();
            DamageResist.Text = sheet.hp.damageResist.GetTotal(sheet).ToString();

            Initiative.Text = sheet.CurrentInitiative.ToString();

            ArmorClass.Text = sheet.ACTotal.ToString();
            TouchAC.Text = sheet.ACTouch.ToString();
            FlatFootedAC.Text = sheet.ACFlatFooted.ToString();

            Fortitude.Text = sheet.GetSavingThrowTotal(CharacterSheet.Save.Fortitude).ToString();
            Reflex.Text = sheet.GetSavingThrowTotal(CharacterSheet.Save.Reflex).ToString();
            Will.Text = sheet.GetSavingThrowTotal(CharacterSheet.Save.Will).ToString();

            var attacksCount = sheet.baseAttackBonus != null ? sheet.baseAttackBonus.Count : 0;
            attacksCount = Math.Max(1, attacksCount);
            var colsCount = BaseAttackBonus.Children.Count;
            var update = Math.Min(colsCount, attacksCount);
            for (var i = 0; i < update; i++)
            {
                var bonus = sheet.GetBaseAttackBonus(i);
                //var b = bonus > 0 ? "+" + bonus : bonus.ToString();
                UpdateValue((BaseAttackBonus.Children[i] as Frame).Content as Label, bonus.ToString());
            }
            var create = colsCount < attacksCount;
            var left = create ? attacksCount - colsCount : colsCount - attacksCount;
            for (int i = 0; i < left; i++)
            {
                if (create)
                {
                    var bonus = sheet.GetBaseAttackBonus(i + update);
                    //var b = bonus > 0 ? "+" + bonus : bonus.ToString();
                    BaseAttackBonus.Children.Add(CreateFrame(bonus.ToString()));
                }
                else
                    BaseAttackBonus.Children.RemoveAt(update);
            }

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

        private Label CreateLabel(string text, TextAlignment horz = TextAlignment.Center)
        {
            return MainPage.CreateLabel(text, horz);
        }

        private Frame CreateFrame(string text)
        {
            return MainPage.CreateFrame(text);
        }

        private void UpdateValue(Label label, string text)
        {
            if (label == null)
                return;
            if (label.Text != text)
                label.Text = text;
        }

        private void AbilityScores_DoubleTapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            pushedPage = new EditAbilityScores();
            Navigation.PushAsync(pushedPage);
        }

        private void MaxHP_DoubleTapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, sheet.hp.maxHP, "Edit Max HP", "Max HP: ", true);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        private void HP_DoubleTapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, sheet.hp.hp, "Edit HP", "HP: ", true);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        private void DamageResist_DoubleTapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, sheet.hp.damageResist, "Edit Damage Resist", "Damage Resist: ", true);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        private void Initiative_DoubleTapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var c = CharacterSheetStorage.Instance.selectedCharacter;
            if (c == null)
                return;
            pushedPage = new EditInitiative();
            Navigation.PushAsync(pushedPage);
        }

        private void ArmorClass_DoubleTapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var c = CharacterSheetStorage.Instance.selectedCharacter;
            if (c == null)
                return;
            pushedPage = new EditArmorClass();
            Navigation.PushAsync(pushedPage);
        }

        private void SavingThrows_DoubleTapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var c = CharacterSheetStorage.Instance.selectedCharacter;
            if (c == null)
                return;
            pushedPage = new EditSavingThrows();
            Navigation.PushAsync(pushedPage);
        }

        private void BaseAttackBonus_DoubleTapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var ebab = new EditBaseAttackBonus();
            ebab.InitEditor();
            pushedPage = ebab;
            Navigation.PushAsync(pushedPage);
        }

        private void SpellResistance_DoubleTapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet,  sheet.spellResistance, "Edit Spell Resistance", "Spell Resistance: ", true);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        private void CombatManeuver_DoubleTapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            pushedPage = new EditCombatManeuvers();
            Navigation.PushAsync(pushedPage);
        }

        private void Speed_DoubleTapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var c = CharacterSheetStorage.Instance.selectedCharacter;
            if (c == null)
                return;
            pushedPage = new EditSpeed();
            Navigation.PushAsync(pushedPage);
        }
    }
}