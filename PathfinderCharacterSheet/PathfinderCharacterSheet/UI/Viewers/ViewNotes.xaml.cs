﻿using PathfinderCharacterSheet.CharacterSheets.V1;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using EditItemType = PathfinderCharacterSheet.EditNote;
using ItemType = PathfinderCharacterSheet.CharacterSheets.V1.Note;
using ViewItemType = PathfinderCharacterSheet.ViewNote;

namespace PathfinderCharacterSheet
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ViewNotes : ContentPage, ISheetView
    {
        private Page pushedPage = null;
        private readonly ViewItemsWithDescription<ItemType> view = null;
        private List<ItemType> GetItems()
        {
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return null;
            return sheet.notes;
        }

        public ViewNotes()
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
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            Reorder.IsVisible = sheet.notes.Count > 1;
            view.UpdateItemsView();
        }

        private void EditItem(ItemType item = null)
        {
            if (pushedPage != null)
                return;
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
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
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
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
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            var ri = new ReorderItemsWithDescription();
            pushedPage = ri;
            var items = new List<ItemWithDescription>();
            foreach (var item in sheet.notes)
                items.Add(item);
            ri.Init(items, (reordered) =>
            {
                sheet.notes.Clear();
                foreach (var item in reordered)
                    sheet.notes.Add(item as ItemType);
                UIMediator.OnCharacterSheetChanged?.Invoke();
            });
            Navigation.PushAsync(pushedPage);
        }
    }
}