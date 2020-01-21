using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ItemType = PathfinderCharacterSheet.CharacterSheet.Feat;
using EditItemType = PathfinderCharacterSheet.EditFeat;
using ViewItemType = PathfinderCharacterSheet.ViewFeat;

namespace PathfinderCharacterSheet
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ViewFeats : ContentPage, ISheetView
    {
        private Page pushedPage = null;
        private ViewItemsWithDescription<ItemType> view = null;
        private List<ItemType> GetItems()
        {
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return null;
            return sheet.feats;
        }

        public ViewFeats()
        {
            InitializeComponent();
            view = new ViewItemsWithDescription<ItemType>()
            {
                actEditItem = EditItem,
                actViewItem = ViewItem,
                items = GetItems,
                layout = Items,
            };
            UpdateView();
        }

        public void UpdateView()
        {
            pushedPage = null;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            Reorder.IsVisible = sheet.feats.Count > 1;
            view.UpdateItemsView();
        }

        private void EditItem(ItemType item = null)
        {
            if (pushedPage != null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var eit = new EditItemType();
            eit.InitEditor(item);
            pushedPage = eit;
            Navigation.PushAsync(pushedPage);
        }

        private void ViewItem(ItemType item = null)
        {
            if (pushedPage != null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var vit = new ViewItemType();
            vit.InitView(item);
            pushedPage = vit;
            Navigation.PushAsync(pushedPage);
        }

        private void AddItem_Clicked(object sender, EventArgs e)
        {
            EditItem();
        }

        private void Reorder_Clicked(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var ri = new ReorderItemsWithDescription();
            pushedPage = ri;
            var items = new List<CharacterSheet.ItemWithDescription>();
            foreach (var item in sheet.feats)
                items.Add(item);
            ri.Init(items, (reordered) =>
            {
                sheet.feats.Clear();
                foreach (var item in reordered)
                    sheet.feats.Add(item as ItemType);
                CharacterSheetStorage.Instance.SaveCharacter();
            });
            Navigation.PushAsync(pushedPage);
        }
    }
}