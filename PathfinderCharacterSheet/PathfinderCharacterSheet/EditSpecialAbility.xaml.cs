using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ItemType = PathfinderCharacterSheet.CharacterSheet.SpecialAbility;

namespace PathfinderCharacterSheet
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditSpecialAbility : ContentPage
    {
        private int itemIndex = -1;
        private List<ItemType> items
        {
            get
            {
                var sheet = CharacterSheetStorage.Instance.selectedCharacter;
                if (sheet != null)
                    return sheet.specialAbilities;
                return null;
            }
        }
        private ItemType item = null;

        public EditSpecialAbility()
        {
            InitializeComponent();
        }

        public void InitEditor(ItemType item = null, int index = -1)
        {
            if (item == null)
            {
                this.item = new ItemType();
                index = -1;
            }
            else
                this.item = item.Clone as ItemType;
            itemIndex = index;
            ItemName.Text = this.item.name;
            Description.Text = this.item.description;
            Delete.IsEnabled = index >= 0;
        }

        private void EditToView()
        {
            if (items == null)
                return;
            item.name = ItemName.Text;
            item.description = Description.Text;
            var hasChanges = false;
            if (itemIndex >= 0)
            {
                var sourceItem = items[itemIndex];
                if (!item.Equals(sourceItem))
                {
                    sourceItem.Fill(item);
                    hasChanges = true;
                }
            }
            else
            {
                items.Add(item);
                hasChanges = true;
            }
            if (hasChanges)
                CharacterSheetStorage.Instance.SaveCharacter();
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

        async void Delete_Clicked(object sender, EventArgs e)
        {
            if (items == null)
                return;
            if (itemIndex < 0)
                return;
            if (itemIndex >= items.Count)
                return;
            var item = items[itemIndex];
            var itemName = string.Empty;
            if ((item != null) && !string.IsNullOrWhiteSpace(item.name))
                itemName = " \"" + item.name + "\"";
            bool allow = await DisplayAlert("Remove item" + itemName, "Are you sure?", "Yes", "No");
            if (allow)
            {
                items.RemoveAt(itemIndex);
                CharacterSheetStorage.Instance.SaveCharacter();
                await Navigation.PopAsync();
            }
        }
    }
}