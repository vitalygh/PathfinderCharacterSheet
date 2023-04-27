using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using PathfinderCharacterSheet.CharacterSheets.V1;
using ItemType = PathfinderCharacterSheet.CharacterSheets.V1.Feat;

namespace PathfinderCharacterSheet
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditFeat : ContentPage
    {
        private List<ItemType> Items
        {
            get
            {
                var sheet = UIMediator.GetSelectedCharacter?.Invoke();
                if (sheet != null)
                    return sheet.feats;
                return null;
            }
        }
        private Page pushedPage = null;
        private ItemType source = null;
        private ItemType item = null;

        public EditFeat()
        {
            InitializeComponent();
        }

        public void InitEditor(ItemType item = null)
        {
            pushedPage = null;
            source = item;
            if (item == null)
                this.item = new ItemType
                {
                    uid = UIMediator.GetUID?.Invoke() ?? CharacterSheet.InvalidUID
                };
            else
                this.item = item.Clone as ItemType;
            ItemName.Text = this.item.name;
            Description.Text = this.item.description;
            Delete.IsEnabled = source != null;
        }

        private void EditToView()
        {
            if (Items == null)
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
                Items.Add(item);
                hasChanges = true;
            }
            if (hasChanges)
                UIMediator.OnCharacterSheetChanged?.Invoke();
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

        async void Delete_Clicked(object sender, EventArgs e)
        {
            if (Items == null)
                return;
            if (source == null)
                return;
            var itemName = string.Empty;
            if (!string.IsNullOrWhiteSpace(source.name))
                itemName = " \"" + source.name + "\"";
            bool allow = await DisplayAlert("Remove item" + itemName, "Are you sure?", "Yes", "No");
            if (allow)
            {
                Items.Remove(source);
                UIMediator.OnCharacterSheetChanged?.Invoke();
                await Navigation.PopAsync();
            }
        }
    }
}