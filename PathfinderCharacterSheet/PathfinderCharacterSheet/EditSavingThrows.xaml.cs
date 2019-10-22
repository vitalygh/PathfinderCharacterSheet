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
    public partial class EditSavingThrows : ContentPage, ISheetView
    {
        List<CharacterSheet.SavingThrow> savingThrows = new List<CharacterSheet.SavingThrow>();

        public EditSavingThrows()
        {
            InitializeComponent();
            InitEditor();
            UpdateView();
        }

        private void InitEditor()
        {
            var c = CharacterSheetStorage.Instance.selectedCharacter;
            if (c == null)
                return;
            savingThrows.Clear();
            var count = c.savingThrows.Length;
            for (var i = 0; i < count; i++)
                savingThrows.Add(c.GetSavingThrow((CharacterSheet.Save)i).Clone);
        }

        public void UpdateView()
        {
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            FortitudeTotal.Text = GetST(CharacterSheet.Save.Fortitude).GetTotal(sheet).ToString();
            ReflexTotal.Text = GetST(CharacterSheet.Save.Reflex).GetTotal(sheet).ToString();
            WillTotal.Text = GetST(CharacterSheet.Save.Will).GetTotal(sheet).ToString();

            FortitudeBaseSave.Text = GetST(CharacterSheet.Save.Fortitude).baseSave.GetTotal(sheet).ToString();
            ReflexBaseSave.Text = GetST(CharacterSheet.Save.Reflex).baseSave.GetTotal(sheet).ToString();
            WillBaseSave.Text = GetST(CharacterSheet.Save.Will).baseSave.GetTotal(sheet).ToString();

            FortitudeAbilityModifier.Text = GetST(CharacterSheet.Save.Fortitude).GetAbilityModifier(sheet).ToString();
            ReflexAbilityModifier.Text = GetST(CharacterSheet.Save.Reflex).GetAbilityModifier(sheet).ToString();
            WillAbilityModifier.Text = GetST(CharacterSheet.Save.Will).GetAbilityModifier(sheet).ToString();

            FortitudeMagicModifier.Text = GetST(CharacterSheet.Save.Fortitude).magicModifier.GetTotal(sheet).ToString();
            ReflexMagicModifier.Text = GetST(CharacterSheet.Save.Reflex).magicModifier.GetTotal(sheet).ToString();
            WillMagicModifier.Text = GetST(CharacterSheet.Save.Will).magicModifier.GetTotal(sheet).ToString();

            FortitudeMiscModifier.Text = GetST(CharacterSheet.Save.Fortitude).miscModifier.GetTotal(sheet).ToString();
            ReflexMiscModifier.Text = GetST(CharacterSheet.Save.Reflex).miscModifier.GetTotal(sheet).ToString();
            WillMiscModifier.Text = GetST(CharacterSheet.Save.Will).miscModifier.GetTotal(sheet).ToString();

            FortitudeTempModifier.Text = GetST(CharacterSheet.Save.Fortitude).tempModifier.GetTotal(sheet).ToString();
            ReflexTempModifier.Text = GetST(CharacterSheet.Save.Reflex).tempModifier.GetTotal(sheet).ToString();
            WillTempModifier.Text = GetST(CharacterSheet.Save.Will).tempModifier.GetTotal(sheet).ToString();
        }

        private void EditToView()
        {
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var hasChanges = false;
            var count = savingThrows.Count;
            for (var i = 0; i < count; i++)
                if (!savingThrows[i].Equals(sheet.savingThrows[i]))
                {
                    sheet.savingThrows = savingThrows.ToArray();
                    hasChanges = true;
                    break;
                }
            if (hasChanges)
                CharacterSheetStorage.Instance.SaveCharacter(CharacterSheetStorage.Instance.selectedCharacter);
        }

        private CharacterSheet.SavingThrow GetST(CharacterSheet.Save st)
        {
            if (savingThrows == null)
                return null;
            var index = (int)st;
            if ((index < 0) || (index >= savingThrows.Count))
                return null;
            return savingThrows[index];
        }

        private void FortitudeBaseSave_Tapped(object sender, EventArgs e)
        {
            var st = GetST(CharacterSheet.Save.Fortitude);
            if (st == null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, st.baseSave, "Edit Fortitude Base Save", "Base Save: ", false);
            Navigation.PushAsync(eivwm);
        }

        private void ReflexBaseSave_Tapped(object sender, EventArgs e)
        {
            var st = GetST(CharacterSheet.Save.Reflex);
            if (st == null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, st.baseSave, "Edit Reflex Base Save", "Base Save: ", false);
            Navigation.PushAsync(eivwm);
        }

        private void WillBaseSave_Tapped(object sender, EventArgs e)
        {
            var st = GetST(CharacterSheet.Save.Will);
            if (st == null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, st.baseSave, "Edit Will Base Save", "Base Save: ", false);
            Navigation.PushAsync(eivwm);
        }

        private void FortitudeMagicModifier_Tapped(object sender, EventArgs e)
        {
            var st = GetST(CharacterSheet.Save.Fortitude);
            if (st == null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, st.magicModifier, "Edit Fortitude Magic Modifier", "Magic Modifier: ", false);
            Navigation.PushAsync(eivwm);
        }

        private void ReflexMagicModifier_Tapped(object sender, EventArgs e)
        {
            var st = GetST(CharacterSheet.Save.Reflex);
            if (st == null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, st.magicModifier, "Edit Reflex Magic Modifier", "Magic Modifier: ", false);
            Navigation.PushAsync(eivwm);
        }

        private void WillMagicModifier_Tapped(object sender, EventArgs e)
        {
            var st = GetST(CharacterSheet.Save.Will);
            if (st == null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, st.magicModifier, "Edit Will Magic Modifier", "Magic Modifier: ", false);
            Navigation.PushAsync(eivwm);
        }

        private void FortitudeMiscModifier_Tapped(object sender, EventArgs e)
        {
            var st = GetST(CharacterSheet.Save.Fortitude);
            if (st == null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, st.miscModifier, "Edit Fortitude Misc Modifier", "Misc Modifier: ", false);
            Navigation.PushAsync(eivwm);
        }

        private void ReflexMiscModifier_Tapped(object sender, EventArgs e)
        {
            var st = GetST(CharacterSheet.Save.Reflex);
            if (st == null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, st.miscModifier, "Edit Reflex Misc Modifier", "Misc Modifier: ", false);
            Navigation.PushAsync(eivwm);
        }

        private void WillMiscModifier_Tapped(object sender, EventArgs e)
        {
            var st = GetST(CharacterSheet.Save.Will);
            if (st == null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, st.miscModifier, "Edit Will Misc Modifier", "Misc Modifier: ", false);
            Navigation.PushAsync(eivwm);
        }

        private void FortitudeTempModifier_Tapped(object sender, EventArgs e)
        {
            var st = GetST(CharacterSheet.Save.Fortitude);
            if (st == null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, st.tempModifier, "Edit Fortitude Temp Modifier", "Temp Modifier: ", false);
            Navigation.PushAsync(eivwm);
        }

        private void ReflexTempModifier_Tapped(object sender, EventArgs e)
        {
            var st = GetST(CharacterSheet.Save.Reflex);
            if (st == null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, st.tempModifier, "Edit Reflex Temp Modifier", "Temp Modifier: ", false);
            Navigation.PushAsync(eivwm);
        }

        private void WillTempModifier_Tapped(object sender, EventArgs e)
        {
            var st = GetST(CharacterSheet.Save.Will);
            if (st == null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, st.tempModifier, "Edit Will Temp Modifier", "Temp Modifier: ", false);
            Navigation.PushAsync(eivwm);
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