using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ItemType = PathfinderCharacterSheet.CharacterSheet.Feat;

namespace PathfinderCharacterSheet
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditFeat : ContentPage
    {
        private List<ItemType> items
        {
            get
            {
                var sheet = CharacterSheetStorage.Instance.selectedCharacter;
                if (sheet != null)
                    return sheet.feats;
                return null;
            }
        }
        private ItemType source = null;
        private ItemType item = null;

        public EditFeat()
        {
            InitializeComponent();
        }

        public void InitEditor(ItemType item = null)
        {
            source = item;
            if (item == null)
                this.item = new ItemType();
            else
                this.item = item.Clone as ItemType;
            ItemName.Text = this.item.name;
            Description.Text = this.item.description;
            Delete.IsEnabled = source != null;
        }

        private void EditToView()
        {
            if (items == null)
                return;
            item.name = ItemName.Text;
            item.description = Description.Text;
            var hasChanges = false;
            if (source != null)
            {
                if (!item.Equals(source))
                {
                    source.Fill(item);
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
            if (source == null)
                return;
            var itemName = string.Empty;
            if (!string.IsNullOrWhiteSpace(source.name))
                itemName = " \"" + source.name + "\"";
            bool allow = await DisplayAlert("Remove item" + itemName, "Are you sure?", "Yes", "No");
            if (allow)
            {
                items.Remove(source);
                CharacterSheetStorage.Instance.SaveCharacter();
                await Navigation.PopAsync();
            }
        }
    }
}