using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ItemType = PathfinderCharacterSheet.CharacterSheet.Feat;
using EditItemType = PathfinderCharacterSheet.EditFeat;

namespace PathfinderCharacterSheet
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ViewFeat : ContentPage, ISheetView
    {
        private ItemType item = null;
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
        private Page pushedPage = null;

        public ViewFeat()
        {
            InitializeComponent();
        }

        public void InitView(ItemType item)
        {
            this.item = item;
            UpdateView();
        }

        public void UpdateView()
        {
            pushedPage = null;
            if (item == null)
                return;
            ItemName.Text = item.name;
            Description.Text = item.description;
        }

        private void Back_Clicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        private void Edit_Clicked(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var eit = new EditItemType();
            var itemIndex = items.IndexOf(item);
            eit.InitEditor(item, itemIndex);
            pushedPage = eit;
            Navigation.PushAsync(pushedPage);
        }

        async void Delete_Clicked(object sender, EventArgs e)
        {
            if (items == null)
                return;
            var itemIndex = items.IndexOf(this.item);
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