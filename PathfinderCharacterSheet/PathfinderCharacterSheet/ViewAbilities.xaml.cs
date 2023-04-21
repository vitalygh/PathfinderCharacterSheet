using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Windows.Input;
using PathfinderCharacterSheet.CharacterSheets.V1;

namespace PathfinderCharacterSheet
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ViewAbilities : ContentPage, ISheetView
	{
        private Page pushedPage = null;
        private const int abilityColumns = 5;

        public ViewAbilities ()
		{
			InitializeComponent ();
            CreateControls();
        }

        private void CreateControls()
        {
            if (AbilityScores.Children.Count <= 0)
            {
                for (var i = 0; i < (int)Ability.Total + 1; i++)
                    for (var j = 0; j < abilityColumns; j++)
                    {
                        View child = null;
                        if ((i <= 0) || (j <= 0))
                        {
                            child = CreateLabel(string.Empty, (j == 0) ? TextAlignment.Start : TextAlignment.Center);
                        }
                        else
                        {
                            child = CreateFrame(string.Empty);
                        }
                        AbilityScores.Children.Add(child, j, i);
                    }
            }
            var abscindex = 1;
            //(AbilityScores.Children[abscindex++] as Label).Text = "Ability Name";
            (AbilityScores.Children[abscindex++] as Label).Text = "Ability Score";
            (AbilityScores.Children[abscindex++] as Label).Text = "Ability Modifier";
            (AbilityScores.Children[abscindex++] as Label).Text = "Temp Score";
            (AbilityScores.Children[abscindex++] as Label).Text = "Temp Modifier";

            MainPage.AddTapHandler(AbilityScores, AbilityScores_DoubleTapped, 2);
            MainPage.AddTapHandler(MaxHPTitle, MaxHP_DoubleTapped, 2);
            MainPage.AddTapHandler(MaxHPFrame, MaxHP_DoubleTapped, 2);
            MainPage.AddTapHandler(HPTitle, HP_DoubleTapped, 2);
            MainPage.AddTapHandler(HPFrame, HP_DoubleTapped, 2);
            MainPage.AddTapHandler(DamageResistTitle, DamageResist_DoubleTapped, 2);
            MainPage.AddTapHandler(DamageResistFrame, DamageResist_DoubleTapped, 2);
            MainPage.AddTapHandler(InitiativeGrid, Initiative_DoubleTapped, 2);
            MainPage.AddTapHandler(ArmorClassGrid, ArmorClass_DoubleTapped, 2);
            MainPage.AddTapHandler(SavingThrowsGrid, SavingThrows_DoubleTapped, 2);
            MainPage.AddTapHandler(BaseAttackBonusGrid, BaseAttackBonus_DoubleTapped, 2);
            MainPage.AddTapHandler(SpellResistanceGrid, SpellResistance_DoubleTapped, 2);
            MainPage.AddTapHandler(CombatManeuverGrid, CombatManeuver_DoubleTapped, 2);
            MainPage.AddTapHandler(SpeedGrid, Speed_DoubleTapped, 2);
        }

        public void UpdateView()
        {
            pushedPage = null;

            var sheet = MainPage.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;

            var abilities = Enum.GetNames(typeof(Ability));
            var abilitiesCount = sheet.abilityScores.Length;
            var hasChanges = false;
            for (var i = 0; i < abilitiesCount; i++)
            {
                var ab = sheet.abilityScores[i];
                if (ab == null)
                {
                    ab = new AbilityScore();
                    sheet.abilityScores[i] = ab;
                    hasChanges = true;
                }
                var abscindex = (i + 1) * abilityColumns;
                (AbilityScores.Children[abscindex++] as Label).Text = abilities[i] + ":";
                var score = ab.score.GetValue(sheet);
                ((AbilityScores.Children[abscindex++] as Frame).Content as Label).Text = score.ToString();
                var modValue = ab.GetModifier(sheet);
                ((AbilityScores.Children[abscindex++] as Frame).Content as Label).Text = modValue.ToString();
                var tempAbValue = score + ab.tempAdjustment.GetValue(sheet);
                var tempAb = (AbilityScores.Children[abscindex++] as Frame).Content as Label;
                tempAb.Text = tempAbValue.ToString();
                var tempMod = (AbilityScores.Children[abscindex++] as Frame).Content as Label;
                var tempModValue = ab.GetTempModifier(sheet);
                tempMod.Text = tempModValue.ToString();
                if (tempModValue > modValue)
                {
                    tempAb.TextColor = Color.Green;
                    tempMod.TextColor = Color.Green;
                }
                else if (tempModValue < modValue)
                {
                    tempAb.TextColor = Color.Red;
                    tempMod.TextColor = Color.Red;
                }
                else
                {
                    tempAb.TextColor = Color.Black;
                    tempMod.TextColor = Color.Black;
                }
            }
            if (hasChanges)
                MainPage.SaveSelectedCharacter?.Invoke();
            var maxHP = sheet.hp.maxHP.GetValue(sheet);
            MaxHP.Text = maxHP.ToString();
            var hp = sheet.hp.hp.GetValue(sheet);
            HP.Text = hp.ToString();
            if (hp > (2 * maxHP / 3))
                HP.TextColor = Color.Green;
            else if (hp > (maxHP / 3))
                HP.TextColor = Color.Orange;
            else
                HP.TextColor = Color.Red;
            DamageResist.Text = sheet.hp.damageResist.GetValue(sheet).ToString();

            Initiative.Text = sheet.CurrentInitiative.ToString();

            ArmorClass.Text = sheet.ACTotal.ToString();
            TouchAC.Text = sheet.ACTouch.ToString();
            FlatFootedAC.Text = sheet.ACFlatFooted.ToString();

            Fortitude.Text = sheet.GetSavingThrowTotal(Save.Fortitude).ToString();
            Reflex.Text = sheet.GetSavingThrowTotal(Save.Reflex).ToString();
            Will.Text = sheet.GetSavingThrowTotal(Save.Will).ToString();

            var attacksCount = sheet.baseAttackBonus != null ? sheet.AttacksCount : 0;
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

            SpellResistance.Text = sheet.spellResistance.GetValue(sheet).ToString();

            CMB.Text = sheet.CMB.ToString();
            CMD.Text = sheet.CMD.ToString();

            var baseSpeed = sheet.speed.baseSpeed.GetValue(sheet);
            BaseSpeed.Text = baseSpeed.ToString() + " ft (" + Speed.InSquares(baseSpeed) + " sq)";
            var speedWithArmor = sheet.speed.armorSpeed.GetValue(sheet);
            SpeedWithArmor.Text = speedWithArmor.ToString() + " ft (" + Speed.InSquares(speedWithArmor) + " sq)";
            FlySpeed.Text = sheet.speed.flySpeed.GetValue(sheet).ToString() + " ft";
            Maneuverability.Text = sheet.speed.maneuverability.GetValue(sheet).ToString();
            SwimSpeed.Text = sheet.speed.GetSwimSpeed(sheet).ToString() + " ft";
            ClimbSpeed.Text = sheet.speed.GetClimbSpeed(sheet).ToString() + " ft";
            BurrowSpeed.Text = sheet.speed.burrowSpeed.GetValue(sheet).ToString() + " ft";
        }

        private Label CreateLabel(string text, TextAlignment horz = TextAlignment.Start)
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
            var sheet = MainPage.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, sheet.hp.maxHP, "Edit Max HP", "Max HP", true);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        private void HP_DoubleTapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var sheet = MainPage.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, sheet.hp.hp, "Edit HP", "HP", true);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        private void DamageResist_DoubleTapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var sheet = MainPage.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, sheet.hp.damageResist, "Edit Damage Resist", "Damage Resist", true);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        private void Initiative_DoubleTapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            pushedPage = new EditInitiative();
            Navigation.PushAsync(pushedPage);
        }

        private void ArmorClass_DoubleTapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            pushedPage = new EditArmorClass();
            Navigation.PushAsync(pushedPage);
        }

        private void SavingThrows_DoubleTapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            pushedPage = new EditSavingThrows();
            Navigation.PushAsync(pushedPage);
        }

        private void BaseAttackBonus_DoubleTapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
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
            var sheet = MainPage.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet,  sheet.spellResistance, "Edit Spell Resistance", "Spell Resistance", true);
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
            pushedPage = new EditSpeed();
            Navigation.PushAsync(pushedPage);
        }
    }
}