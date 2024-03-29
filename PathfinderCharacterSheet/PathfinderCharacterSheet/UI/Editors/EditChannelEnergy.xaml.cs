﻿using PathfinderCharacterSheet.CharacterSheets.V1;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PathfinderCharacterSheet
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EditChannelEnergy : ContentPage, ISheetView
	{
        private Page pushedPage = null;
        private ChannelEnergy channelEnergy = null;

		public EditChannelEnergy ()
		{
			InitializeComponent ();
		}

        public void InitEditor()
        {
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            channelEnergy = sheet.channelEnergy.Clone;
            UpdateView();
        }

        public void UpdateView()
        {
            pushedPage = null;
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            if (channelEnergy == null)
                return;
            UIHelpers.UpdateValue(ChannelsLeft, channelEnergy.left.GetValue(sheet).ToString());
            UIHelpers.UpdateValue(ChannelsTotal, channelEnergy.total.GetValue(sheet).ToString());
            UIHelpers.UpdateValue(ChannelPoints, channelEnergy.points.AsString(sheet));
        }

        private void ChannelsLeft_Tapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, channelEnergy.left, "Edit Channels Energy", "Channels Left", false);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        private void ChannelsTotal_Tapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, channelEnergy.total, "Edit Channels Energy", "Channels Total", false);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        private void ChannelPoints_Tapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            var ed = new EditDiceRoll();
            ed.Init(sheet, channelEnergy.points, null, true);
            pushedPage = ed;
            Navigation.PushAsync(ed);
        }

        private void EditToView()
        {
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            if (channelEnergy == null)
                return;
            if (channelEnergy.Equals(sheet.channelEnergy))
                return;
            sheet.channelEnergy.Fill(channelEnergy);
            UIMediator.OnCharacterSheetChanged?.Invoke();
        }

        private void Cancel_Clicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        private void Save_Clicked(object sender, EventArgs e)
        {
            EditToView();
            Navigation.PopAsync();
        }
    }
}