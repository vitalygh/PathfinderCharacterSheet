using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using PathfinderCharacterSheet.CharacterSheets.V1;
using ItemType = PathfinderCharacterSheet.CharacterSheets.V1.LevelOfClass;

namespace PathfinderCharacterSheet
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SelectClass : ContentPage
    {
        private Action<ItemType> onSelect = null;
        private readonly List<Frame> framesPool = new List<Frame>();
        private LevelOfClass selected = null;

        public SelectClass()
        {
            InitializeComponent();
        }

        public void InitSelection(Action<ItemType> onSelect, LevelOfClass selected)
        {
            this.onSelect = onSelect;
            this.selected = selected;
            InitItems();
        }

        private void InitItems()
        {
            var sheet = MainPage.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            var items = new List<ItemType>
            {
                new ItemType()
            };
            items.AddRange(sheet.levelOfClass);
            var itemsCount = items.Count;
            var framesCount = Items.Children.Count;
            var update = Math.Min(framesCount, itemsCount);
            for (var i = 0; i < update; i++)
            {
                var item = items[i];
                var frame = Items.Children[i] as Frame;
                UpdateFrame(frame, item);
            }
            var create = framesCount < itemsCount;
            var count = create ? itemsCount - framesCount : framesCount - itemsCount;
            if (create)
                for (var i = 0; i < count; i++)
                    CreateFrame(items[update + i]);
            else
                for (var i = 0; i < count; i++)
                {
                    var frame = Items.Children[update] as Frame;
                    Items.Children.RemoveAt(update);
                    framesPool.Add(frame);
                }
        }

        private void CreateFrame(ItemType item)
        {
            Frame frame;
            if (framesPool.Count <= 0)
                frame = MainPage.CreateFrame(string.Empty);
            else
            {
                frame = framesPool[0];
                framesPool.RemoveAt(0);
            }
            UpdateFrame(frame, item);
            Items.Children.Add(frame);
        }

        private void UpdateFrame(Frame frame, ItemType item)
        {
            var sheet = MainPage.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            var label = frame.Content as Label;
            label.Text = item.ClassName ?? "Total Level";
            label.TextColor = ((selected != null) && (selected.ClassName == item.ClassName)) ? Color.Green : Color.Black;
            MainPage.SetTapHandler(frame, (s, e) => SelectItem(item));
        }

        private void SelectItem(ItemType item)
        {
            onSelect?.Invoke(item);
            Navigation.PopAsync();
        }

        private void Cancel_Clicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        private void SelectNothing_Clicked(object sender, EventArgs e)
        {
            SelectItem(null);
        }
    }
}