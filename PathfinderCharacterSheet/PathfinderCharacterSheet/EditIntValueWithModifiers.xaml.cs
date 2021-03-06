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
	public partial class EditIntValueWithModifiers : ContentPage, ISheetView
	{
        private Page pushedPage = null;
        private CharacterSheet sheet = null;
        private CharacterSheet.ValueWithIntModifiers source = null;
        private CharacterSheet.ModifiersList<CharacterSheet.IntModifier, int, CharacterSheet.IntSum> modifiers = null;
        private bool saveCharacter = false;
        private bool allowUseAbilities = true;

        public EditIntValueWithModifiers ()
		{
			InitializeComponent ();
		}

        public void Init(CharacterSheet sheet, CharacterSheet.ValueWithIntModifiers source, string title, string valueName, bool saveCharacter, bool allowUseAbilities = true)
        {
            Title = title;
            ValueName.Text = valueName;
            this.allowUseAbilities = allowUseAbilities;
            if (source == null)
                return;
            this.sheet = sheet;
            this.source = source;
            this.saveCharacter = saveCharacter;
            modifiers = source.modifiers.Clone as CharacterSheet.ModifiersList<CharacterSheet.IntModifier, int, CharacterSheet.IntSum>;
            Value.Text = source.baseValue.ToString();
            UpdateView();
        }

        public void UpdateView()
        {
            pushedPage = null;
            UpdateModifiersSum();
            if (modifiers == null)
                return;
            MainPage.FillIntMLGrid(Modifiers, sheet, modifiers, "Modifiers", EditModifier, EditModifier, ReorderModifiers, (modifiers, modifier) => UpdateModifiersSum());
        }

        private void UpdateTotal()
        {
            var total = 0;
            MainPage.StrToInt(Value.Text, ref total);
            if (modifiers != null)
                total += modifiers.GetTotal(sheet);
            Total.Text = total.ToString();
        }

        private void UpdateModifiersSum()
        {
            if (modifiers != null)
                ModifiersSum.Text = modifiers.GetTotal(sheet).ToString();
            UpdateTotal();
        }

        private void ReorderModifiers(CharacterSheet.ModifiersList<CharacterSheet.IntModifier, int, CharacterSheet.IntSum> modifiers)
        {
            if (pushedPage != null)
                return;
            var ri = new ReorderIntModifiers();
            pushedPage = ri;
            var items = new CharacterSheet.ModifiersList<CharacterSheet.IntModifier, int, CharacterSheet.IntSum>();
            foreach (var item in modifiers)
                items.Add(item);
            ri.Init(items, (reordered) =>
            {
                modifiers.Clear();
                foreach (var item in reordered)
                    modifiers.Add(item as CharacterSheet.IntModifier);
            });
            Navigation.PushAsync(pushedPage);
        }

        private void EditModifier(CharacterSheet.ModifiersList<CharacterSheet.IntModifier, int, CharacterSheet.IntSum> modifiers)
        {
            EditModifier(modifiers, null);
        }

        private void EditModifier(CharacterSheet.ModifiersList<CharacterSheet.IntModifier, int, CharacterSheet.IntSum> modifiers, CharacterSheet.IntModifier modifier)
        {
            if (pushedPage != null)
                return;
            var page = new EditIntModifier();
            page.Init(sheet, modifiers, modifier, allowUseAbilities);
            pushedPage = page;
            Navigation.PushAsync(page);
        }

        private void Value_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateTotal();
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

        private void EditToView()
        {
            if (source == null)
                return;
            var hasChanged = MainPage.StrToInt(Value.Text, ref source.baseValue);
            if (!source.modifiers.Equals(modifiers))
            {
                hasChanged = true;
                source.modifiers = modifiers;
            }
            if (hasChanged && saveCharacter)
                CharacterSheetStorage.Instance.SaveCharacter();
        }
    }
}