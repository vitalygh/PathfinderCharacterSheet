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
	public partial class EditSpeed : ContentPage, ISheetView
	{
        private Page pushedPage = null;
        private CharacterSheet.Speed speed = null;

		public EditSpeed ()
		{
			InitializeComponent ();
            InitEditor();
            UpdateView();
        }

        private void InitEditor()
        {
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            speed = sheet.speed.Clone as CharacterSheet.Speed;
        }

        public void UpdateView()
        {
            pushedPage = null;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var bs = speed.baseSpeed.GetTotal(sheet);
            BaseSpeed.Text = bs + " ft (" + CharacterSheet.Speed.InSquares(bs) + " sq)";
            var ars = speed.armorSpeed.GetTotal(sheet);
            SpeedWithArmor.Text = ars + " ft (" + CharacterSheet.Speed.InSquares(ars) + " sq)";
            FlySpeed.Text = speed.flySpeed.GetTotal(sheet) + " ft";
            Maneuverability.Text = speed.maneuverability.GetTotal(sheet).ToString();
            DefaultSwim.IsChecked = speed.defaultSwimSpeed;
            UpdateSwimSpeed();
            DefaultClimb.IsChecked = speed.defaultClimbSpeed;
            UpdateClimbSpeed();
            BurrowSpeed.Text = speed.burrowSpeed.GetTotal(sheet) + " ft";
       }

        private void EditToView()
        {
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            if (!speed.Equals(sheet.speed))
            {
                sheet.speed = speed;
                CharacterSheetStorage.Instance.SaveCharacter();
            }
        }

        private void BaseSpeed_Tapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, speed.baseSpeed, "Edit Base Speed", "Base Speed: ", false);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        private void SpeedWithArmor_Tapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, speed.armorSpeed, "Edit Speed With Armor", "Speed With Armor: ", false);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        private void FlySpeed_Tapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, speed.flySpeed, "Edit Fly Speed", "Fly Speed: ", false);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        private void Maneuverability_Tapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, speed.maneuverability, "Edit Maneuverability", "Maneuverability: ", false);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        private void SwimSpeed_Tapped(object sender, EventArgs e)
        {
            if (speed.defaultSwimSpeed)
                return;
            if (pushedPage != null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, speed.swimSpeed, "Edit Swim Speed", "Swim Speed: ", false);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        private void ClimbSpeed_Tapped(object sender, EventArgs e)
        {
            if (speed.defaultClimbSpeed)
                return;
            if (pushedPage != null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, speed.climbSpeed, "Edit Climb Speed", "Climb Speed: ", false);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        private void BurrowSpeed_Tapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, speed.burrowSpeed, "Edit Burrow Speed", "Burrow Speed: ", false);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        private void UpdateSwimSpeed()
        {
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var dss = DefaultSwim.IsChecked;
            speed.defaultSwimSpeed = dss;
            SwimSpeedFrame.BackgroundColor = dss ? Color.LightGray : Color.White;
            SwimSpeed.TextDecorations = dss ? TextDecorations.None : TextDecorations.Underline;
            SwimSpeed.Text = speed.GetSwimSpeed(sheet) + " ft";
        }

        private void UpdateClimbSpeed()
        {
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var dcs = DefaultClimb.IsChecked;
            speed.defaultClimbSpeed = dcs;
            ClimbSpeedFrame.BackgroundColor = dcs ? Color.LightGray : Color.White;
            ClimbSpeed.TextDecorations = dcs ? TextDecorations.None : TextDecorations.Underline;
            ClimbSpeed.Text = speed.GetClimbSpeed(sheet) + " ft";
        }

        private void DefaultSwim_Changed(object sender, CheckedChangedEventArgs e)
        {
            UpdateSwimSpeed();
        }

        private void DefaultClimb_Changed(object sender, CheckedChangedEventArgs e)
        {
            UpdateClimbSpeed();
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
            EditToView();
            Navigation.PopAsync();
        }
    }
}