﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ItemType = PathfinderCharacterSheet.CharacterSheet.GearItem;

namespace PathfinderCharacterSheet
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SelectGearItem : ContentPage
    {
        private Action<ItemType> onSelect = null;
        private List<Frame> framesPool = new List<Frame>();
        private ItemType selected = null;

        public SelectGearItem()
        {
            InitializeComponent();
        }

        public void InitSelection(Action<ItemType> onSelect, ItemType selected)
        {
            this.onSelect = onSelect;
            this.selected = selected;
            InitItems();
        }

        private void InitItems()
        {
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var items = sheet.GetAllGearItems();
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
            Frame frame = null;
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
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var label = frame.Content as Label;
            label.Text = item.AsString(sheet);
            label.FontAttributes = item.active ? FontAttributes.Bold : FontAttributes.None;
            label.TextColor = selected == item ? Color.Green : Color.Black;
            MainPage.SetTapHandler(frame, (s, e) => SelectItem(item));
        }

        private void SelectItem(ItemType item)
        {
            if (onSelect != null)
                onSelect(item);
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