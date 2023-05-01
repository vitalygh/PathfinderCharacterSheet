using PathfinderCharacterSheet.CharacterSheets.V1;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PathfinderCharacterSheet
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditSavingThrows : ContentPage, ISheetView
    {
        private Page pushedPage = null;
        private readonly List<SavingThrow> savingThrows = new List<SavingThrow>();

        public EditSavingThrows()
        {
            InitializeComponent();
            InitEditor();
            UpdateView();
        }

        private void InitEditor()
        {
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            savingThrows.Clear();
            var count = sheet.savingThrows.Length;
            for (var i = 0; i < count; i++)
                savingThrows.Add(sheet.GetSavingThrow((Save)i).Clone);
        }

        public void UpdateView()
        {
            pushedPage = null;
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            FortitudeTotal.Text = GetST(Save.Fortitude).GetTotal(sheet).ToString();
            ReflexTotal.Text = GetST(Save.Reflex).GetTotal(sheet).ToString();
            WillTotal.Text = GetST(Save.Will).GetTotal(sheet).ToString();

            FortitudeBaseSave.Text = GetST(Save.Fortitude).baseSave.GetValue(sheet).ToString();
            ReflexBaseSave.Text = GetST(Save.Reflex).baseSave.GetValue(sheet).ToString();
            WillBaseSave.Text = GetST(Save.Will).baseSave.GetValue(sheet).ToString();

            FortitudeAbilityModifier.Text = GetST(Save.Fortitude).GetAbilityModifier(sheet).ToString();
            ReflexAbilityModifier.Text = GetST(Save.Reflex).GetAbilityModifier(sheet).ToString();
            WillAbilityModifier.Text = GetST(Save.Will).GetAbilityModifier(sheet).ToString();

            FortitudeMagicModifier.Text = GetST(Save.Fortitude).magicModifier.GetValue(sheet).ToString();
            ReflexMagicModifier.Text = GetST(Save.Reflex).magicModifier.GetValue(sheet).ToString();
            WillMagicModifier.Text = GetST(Save.Will).magicModifier.GetValue(sheet).ToString();

            FortitudeMiscModifier.Text = GetST(Save.Fortitude).miscModifier.GetValue(sheet).ToString();
            ReflexMiscModifier.Text = GetST(Save.Reflex).miscModifier.GetValue(sheet).ToString();
            WillMiscModifier.Text = GetST(Save.Will).miscModifier.GetValue(sheet).ToString();

            FortitudeTempModifier.Text = GetST(Save.Fortitude).tempModifier.GetValue(sheet).ToString();
            ReflexTempModifier.Text = GetST(Save.Reflex).tempModifier.GetValue(sheet).ToString();
            WillTempModifier.Text = GetST(Save.Will).tempModifier.GetValue(sheet).ToString();
        }

        private void EditToView()
        {
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
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
                UIMediator.OnCharacterSheetChanged?.Invoke();
        }

        private SavingThrow GetST(Save st)
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
            if (pushedPage != null)
                return;
            var st = GetST(Save.Fortitude);
            if (st == null)
                return;
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, st.baseSave, "Edit Fortitude Base Save", "Base Save", false);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        private void ReflexBaseSave_Tapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var st = GetST(Save.Reflex);
            if (st == null)
                return;
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, st.baseSave, "Edit Reflex Base Save", "Base Save", false);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        private void WillBaseSave_Tapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var st = GetST(Save.Will);
            if (st == null)
                return;
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, st.baseSave, "Edit Will Base Save", "Base Save", false);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        private void FortitudeMagicModifier_Tapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var st = GetST(Save.Fortitude);
            if (st == null)
                return;
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, st.magicModifier, "Edit Fortitude Magic Modifier", "Magic Modifier", false);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        private void ReflexMagicModifier_Tapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var st = GetST(Save.Reflex);
            if (st == null)
                return;
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, st.magicModifier, "Edit Reflex Magic Modifier", "Magic Modifier", false);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        private void WillMagicModifier_Tapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var st = GetST(Save.Will);
            if (st == null)
                return;
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, st.magicModifier, "Edit Will Magic Modifier", "Magic Modifier", false);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        private void FortitudeMiscModifier_Tapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var st = GetST(Save.Fortitude);
            if (st == null)
                return;
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, st.miscModifier, "Edit Fortitude Misc Modifier", "Misc Modifier", false);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        private void ReflexMiscModifier_Tapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var st = GetST(Save.Reflex);
            if (st == null)
                return;
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, st.miscModifier, "Edit Reflex Misc Modifier", "Misc Modifier", false);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        private void WillMiscModifier_Tapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var st = GetST(Save.Will);
            if (st == null)
                return;
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, st.miscModifier, "Edit Will Misc Modifier", "Misc Modifier", false);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        private void FortitudeTempModifier_Tapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var st = GetST(Save.Fortitude);
            if (st == null)
                return;
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, st.tempModifier, "Edit Fortitude Temp Modifier", "Temp Modifier", false);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        private void ReflexTempModifier_Tapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var st = GetST(Save.Reflex);
            if (st == null)
                return;
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, st.tempModifier, "Edit Reflex Temp Modifier", "Temp Modifier", false);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        private void WillTempModifier_Tapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var st = GetST(Save.Will);
            if (st == null)
                return;
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, st.tempModifier, "Edit Will Temp Modifier", "Temp Modifier", false);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
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