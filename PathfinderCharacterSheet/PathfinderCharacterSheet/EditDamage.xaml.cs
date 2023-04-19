using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using PathfinderCharacterSheet.CharacterSheets.V1;
using ItemType = PathfinderCharacterSheet.CharacterSheets.V1.DiceRoll;
using EditItemType = PathfinderCharacterSheet.EditDiceRoll;

namespace PathfinderCharacterSheet
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditDamage : ContentPage, ISheetView
    {
        private Page pushedPage = null;
        private ViewDiceRolls view = null;
        private CharacterSheet sheet = null;
        private List<ItemType> source = null;
        private List<ItemType> rolls = null;
        private List<ItemType> GetItems()
        {
            if (rolls == null)
                return null;
            return rolls;
        }

        public EditDamage()
        {
            InitializeComponent();
            view = new ViewDiceRolls()
            {
                actEditItem = EditItem,
                items = GetItems,
                layout = Items,
            };
        }

        public void Init(CharacterSheet sheet, List<ItemType> damageRolls)
        {
            if (sheet == null)
                return;
            if (damageRolls == null)
                return;
            this.sheet = sheet;
            source = damageRolls;
            rolls = new List<ItemType>();
            foreach (var roll in damageRolls)
                rolls.Add(roll.Clone as ItemType);
            UpdateView();
        }

        public void UpdateView()
        {
            pushedPage = null;
            Reorder.IsVisible = rolls.Count > 1;
            view.UpdateItemsView();
        }

        private void EditItem(ItemType item = null)
        {
            if (pushedPage != null)
                return;
            var eit = new EditItemType();
            eit.Init(sheet, item, rolls, false);
            pushedPage = eit;
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
            var rdr = new ReorderDiceRolls();
            pushedPage = rdr;
            var items = new List<ItemType>();
            foreach (var item in rolls)
                items.Add(item);
            rdr.Init(items, (reordered) =>
            {
                rolls.Clear();
                foreach (var item in reordered)
                    rolls.Add(item as ItemType);
            });
            Navigation.PushAsync(pushedPage);
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
            if (!CharacterSheet.IsEqual(source, rolls))
            {
                source.Clear();
                foreach (var roll in rolls)
                    source.Add(roll);
            }
            Navigation.PopAsync();
        }
    }
}