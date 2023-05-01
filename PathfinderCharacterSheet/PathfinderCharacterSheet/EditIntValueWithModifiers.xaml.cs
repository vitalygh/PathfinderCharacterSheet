using PathfinderCharacterSheet.CharacterSheets.V1;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PathfinderCharacterSheet
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EditIntValueWithModifiers : ContentPage, ISheetView
	{
        private Page pushedPage = null;
        private CharacterSheet sheet = null;
        private ValueWithIntModifiers source = null;
        private IntModifiersList modifiers = null;
        private bool saveCharacter = false;
        private bool allowUseAbilities = true;

        public EditIntValueWithModifiers ()
		{
			InitializeComponent ();
		}

        public void Init(CharacterSheet sheet, ValueWithIntModifiers source, string title, string valueName, bool saveCharacter, bool allowUseAbilities = true)
        {
            Title = title;
            ValueName.Text = valueName;
            this.allowUseAbilities = allowUseAbilities;
            if (source == null)
                return;
            this.sheet = sheet;
            this.source = source;
            this.saveCharacter = saveCharacter;
            modifiers = source.Clone.Modifiers;
            if (modifiers == null)
                modifiers = new IntModifiersList();
            Value.Text = source.BaseValue.ToString();
            UpdateView();
        }

        public void UpdateView()
        {
            pushedPage = null;
            UpdateModifiersSum();
            if (modifiers == null)
                return;
            UIHelpers.FillIntMLGrid(Modifiers, sheet, modifiers, "Modifiers", EditModifier, EditModifier, ReorderModifiers, (modifiers, modifier) => UpdateModifiersSum());
        }

        private void UpdateTotal()
        {
            var total = 0;
            UIHelpers.StrToInt(Value.Text, ref total);
            if (modifiers != null)
                total += modifiers.GetValue(sheet);
            Total.Text = total.ToString();
        }

        private void UpdateModifiersSum()
        {
            if (modifiers != null)
                ModifiersSum.Text = modifiers.GetValue(sheet).ToString();
            UpdateTotal();
        }

        private void ReorderModifiers(IntModifiersList modifiers)
        {
            if (pushedPage != null)
                return;
            var ri = new ReorderIntModifiers();
            pushedPage = ri;
            var items = new IntModifiersList();
            foreach (var item in modifiers)
                items.Add(item);
            ri.Init(items, (reordered) =>
            {
                modifiers.Clear();
                foreach (var item in reordered)
                    modifiers.Add(item);
            });
            Navigation.PushAsync(pushedPage);
        }

        private void EditModifier(IntModifiersList modifiers)
        {
            EditModifier(modifiers, null);
        }

        private void EditModifier(IntModifiersList modifiers, IntModifier modifier)
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
            var baseValue = source.BaseValue;
            var hasChanged = UIHelpers.StrToInt(Value.Text, ref baseValue);
            source.BaseValue = baseValue;
            if (!source.Modifiers.Equals(modifiers))
            {
                hasChanged = true;
                source.Modifiers = modifiers;
            }
            if (hasChanged && saveCharacter)
                UIMediator.OnCharacterSheetChanged?.Invoke();
        }
    }
}