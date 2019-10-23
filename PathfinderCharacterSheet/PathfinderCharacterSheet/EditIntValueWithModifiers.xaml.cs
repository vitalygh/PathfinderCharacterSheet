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
	public partial class EditIntValueWithModifiers : ContentPage, ISheetView
	{
        private CharacterSheet sheet = null;
        private CharacterSheet.ValueWithModifiers<int, CharacterSheet.IntSum> source = null;
        private CharacterSheet.ModifiersList<int, CharacterSheet.IntSum> modifiers = null;
        private bool saveCharacter = false;

        public EditIntValueWithModifiers ()
		{
			InitializeComponent ();
		}

        public void Init(CharacterSheet sheet, CharacterSheet.ValueWithModifiers<int, CharacterSheet.IntSum> source, string title, string valueName, bool saveCharacter)
        {
            Title = title;
            ValueName.Text = valueName;
            if (source == null)
                return;
            this.sheet = sheet;
            this.source = source;
            this.saveCharacter = saveCharacter;
            modifiers = source.modifiers.Clone as CharacterSheet.ModifiersList<int, CharacterSheet.IntSum>;
            Value.Text = source.baseValue.ToString();
            UpdateView();
        }

        public void UpdateView()
        {
            UpdateModifiersSum();
            if (modifiers == null)
                return;
            MainPage.FillIntMLGrid(Modifiers, sheet, modifiers, "Modifiers:", EditModifier, EditModifier, (modifiers, modifier) => UpdateModifiersSum());
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

        private void EditModifier(CharacterSheet.ModifiersList<int, CharacterSheet.IntSum> modifiers)
        {
            EditModifier(modifiers, null);
        }

        private void EditModifier(CharacterSheet.ModifiersList<int, CharacterSheet.IntSum> modifiers, CharacterSheet.Modifier<int> modifier)
        {
            var page = new EditIntModifier();
            page.Init(sheet, modifiers, modifier);
            Navigation.PushAsync(page);
        }

        private void Value_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateTotal();
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
                CharacterSheetStorage.Instance.SaveCharacter(CharacterSheetStorage.Instance.selectedCharacter);
        }
    }
}