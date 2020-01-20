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
    public partial class ReorderItemsWithDescription : ContentPage
    {
        class Controls
        {
            public View up = null;
            public Label text = null;
            public View down = null;
        }

        private Page pushedPage = null;
        private List<CharacterSheet.ItemWithDescription> initItems = null;
        private List<CharacterSheet.ItemWithDescription> items = null;
        private Action<List<CharacterSheet.ItemWithDescription>> reorder = null;
        private List<Controls> controls = new List<Controls>();

        public ReorderItemsWithDescription()
        {
            InitializeComponent();
        }

        private void MoveItem(CharacterSheet.ItemWithDescription item, int dir, bool onePosition)
        {
            if (items == null)
                return;
            var index = items.IndexOf(item);
            if (index < 0)
                return;
            var newIndex = onePosition ? index + dir : (dir < 0 ? 0 : items.Count - 1);
            newIndex = Math.Min(Math.Max(0, newIndex), items.Count - 1);
            if (newIndex == index)
                return;
            items.Remove(item);
            items.Insert(newIndex, item);
            UpdateLabels();
        }

        private void UpdateItem(Controls controls, CharacterSheet.ItemWithDescription item)
        {
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            MainPage.SetTapHandler(controls.up, () => MoveItem(item, -1, true));
            MainPage.AddTapHandler(controls.up, () => MoveItem(item, -1, false), 2);
            controls.text.Text = item.AsString(sheet);
            MainPage.SetTapHandler(controls.down, () => MoveItem(item, 1, true));
            MainPage.AddTapHandler(controls.down, () => MoveItem(item, 1, false), 2);
        }

        private View AddButton(string text)
        {
            var label = MainPage.CreateLabel(text);
            var sl = new StackLayout();
            sl.Children.Add(label);
            sl.BackgroundColor = Color.LightGray;
            return sl;
        }

        private void AddItem(CharacterSheet.ItemWithDescription item)
        {
            if (item == null)
                return;
            var sl = new StackLayout() { Orientation = StackOrientation.Horizontal, };
            var up = AddButton("Up");
            var down = AddButton("Down");
            var frame = MainPage.CreateFrame(string.Empty);
            sl.Children.Add(up);
            sl.Children.Add(frame);
            sl.Children.Add(down);
            Items.Children.Add(sl);
            var cntrls = new Controls()
            {
                up = up,
                text = frame.Content as Label,
                down = down,
            };
            controls.Add(cntrls);
            UpdateItem(cntrls, item);
        }

        private void UpdateLabels()
        {
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            for (var i = 0; i < items.Count; i++)
            {
                if (items.Count <= i)
                    break;
                UpdateItem(controls[i], items[i]);
            }
        }

        public void Init(List<CharacterSheet.ItemWithDescription> items, Action<List<CharacterSheet.ItemWithDescription>> reorder)
        {
            this.items = items;
            initItems = new List<CharacterSheet.ItemWithDescription>(items);
            this.reorder = reorder;
            pushedPage = null;
            Items.Children.Clear();
            controls.Clear();
            foreach (var item in items)
                AddItem(item);
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
            if (!CharacterSheet.IsEqual(initItems, items))
                reorder?.Invoke(items);
            Navigation.PopAsync();
        }
    }
}