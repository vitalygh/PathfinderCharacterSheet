using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ItemType = PathfinderCharacterSheet.CharacterSheets.V1.Note;
using EditItemType = PathfinderCharacterSheet.EditNote;

namespace PathfinderCharacterSheet
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ViewNote : ContentPage, ISheetView
    {
        private ItemType item = null;
        private List<ItemType> Items
        {
            get
            {
                var sheet = MainPage.GetSelectedCharacter?.Invoke();
                if (sheet != null)
                    return sheet.notes;
                return null;
            }
        }
        private Page pushedPage = null;

        public ViewNote()
        {
            InitializeComponent();
            MainPage.AddTapHandler(NoteGrid, Edit_Clicked, 2);
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
            if ((Items != null) && !Items.Contains(item))
            {
                Navigation.PopAsync();
                return;
            }
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
            eit.InitEditor(item);
            pushedPage = eit;
            Navigation.PushAsync(pushedPage);
        }

        async void Delete_Clicked(object sender, EventArgs e)
        {
            if (Items == null)
                return;
            if (Items == null)
                return;
            var itemName = string.Empty;
            if (!string.IsNullOrWhiteSpace(item.name))
                itemName = " \"" + item.name + "\"";
            bool allow = await DisplayAlert("Remove item" + itemName, "Are you sure?", "Yes", "No");
            if (allow)
            {
                Items.Remove(item);
                MainPage.SaveSelectedCharacter?.Invoke();
                await Navigation.PopAsync();
            }
        }
    }
}