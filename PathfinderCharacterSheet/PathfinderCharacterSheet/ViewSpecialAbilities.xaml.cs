using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ItemType = PathfinderCharacterSheet.CharacterSheet.SpecialAbility;
using EditItemType = PathfinderCharacterSheet.EditSpecialAbility;
using ViewItemType = PathfinderCharacterSheet.ViewSpecialAbility;

namespace PathfinderCharacterSheet
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ViewSpecialAbilities : ContentPage, ISheetView
    {
        private Page pushedPage = null;
        private ViewItemsWithDecription<ItemType> view = null;
        private List<ItemType> GetItems()
        {
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return null;
            return sheet.specialAbilities;
        }

        public ViewSpecialAbilities()
        {
            InitializeComponent();
            view = new ViewItemsWithDecription<ItemType>()
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
    }
}