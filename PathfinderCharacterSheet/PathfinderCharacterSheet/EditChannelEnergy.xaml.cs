using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using PathfinderCharacterSheet.CharacterSheets.V1;

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
            var sheet = MainPage.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            channelEnergy = sheet.channelEnergy.Clone as ChannelEnergy;
            UpdateView();
        }

        public void UpdateView()
        {
            pushedPage = null;
            var sheet = MainPage.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            if (channelEnergy == null)
                return;
            UpdateText(ChannelsLeft, channelEnergy.left.GetValue(sheet).ToString());
            UpdateText(ChannelsTotal, channelEnergy.total.GetValue(sheet).ToString());
            UpdateText(ChannelPoints, channelEnergy.points.AsString(sheet));
        }

        private void UpdateText(Label label, string text)
        {
            if (label.Text != text)
                label.Text = text;
        }

        private void ChannelsLeft_Tapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var sheet = MainPage.GetSelectedCharacter?.Invoke();
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
            var sheet = MainPage.GetSelectedCharacter?.Invoke();
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
            var sheet = MainPage.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            var ed = new EditDiceRoll();
            ed.Init(sheet, channelEnergy.points, null, true);
            pushedPage = ed;
            Navigation.PushAsync(ed);
        }

        private void EditToView()
        {
            var sheet = MainPage.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            if (channelEnergy == null)
                return;
            if (channelEnergy.Equals(sheet.channelEnergy))
                return;
            sheet.channelEnergy.Fill(channelEnergy);
            MainPage.OnCharacterSheetChanged?.Invoke();
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