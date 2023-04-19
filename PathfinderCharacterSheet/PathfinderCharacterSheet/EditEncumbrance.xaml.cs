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
	public partial class EditEncumbrance : ContentPage, ISheetView
	{
        private Page pushedPage = null;
        private Encumbrance encumbrance = null;

        public EditEncumbrance()
        {
            InitializeComponent();
            InitEditor();
            UpdateView();
        }

        private void InitEditor()
        {
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            encumbrance = sheet.encumbrance.Clone as Encumbrance;
        }

        public void UpdateView()
        {
            pushedPage = null;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            LightLoad.Text = encumbrance.LightLoad(sheet);
            MediumLoad.Text = encumbrance.MediumLoad(sheet);
            HeavyLoad.Text = encumbrance.HeavyLoad(sheet);
            DefaultLiftOverHead.IsChecked = encumbrance.defaultLiftOverHead;
            UpdateLiftOverHead();
            DefaultLiftOffGround.IsChecked = encumbrance.defaultLiftOffGround;
            UpdateLiftOffGround();
            DefaultDragOrPush.IsChecked = encumbrance.defaultDragOrPush;
            UpdateDragOrPush();
        }

        private void EditToView()
        {
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            if (!encumbrance.Equals(sheet.encumbrance))
            {
                sheet.encumbrance = encumbrance;
                CharacterSheetStorage.Instance.SaveCharacter();
            }
        }

        private void UpdateLiftOverHead()
        {
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var dloh = DefaultLiftOverHead.IsChecked;
            encumbrance.defaultLiftOverHead = dloh;
            LiftOverHeadFrame.BackgroundColor = dloh ? Color.LightGray : Color.White;
            LiftOverHead.TextDecorations = dloh ? TextDecorations.None : TextDecorations.Underline;
            LiftOverHead.Text = encumbrance.LiftOverHead(sheet) + " lbs";
        }

        private void UpdateLiftOffGround()
        {
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var dlog = DefaultLiftOffGround.IsChecked;
            encumbrance.defaultLiftOffGround = dlog;
            LiftOffGroundFrame.BackgroundColor = dlog ? Color.LightGray : Color.White;
            LiftOffGround.TextDecorations = dlog ? TextDecorations.None : TextDecorations.Underline;
            LiftOffGround.Text = encumbrance.LiftOffGround(sheet) + " lbs";
        }

        private void UpdateDragOrPush()
        {
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var ddop = DefaultDragOrPush.IsChecked;
            encumbrance.defaultDragOrPush = ddop;
            DragOrPushFrame.BackgroundColor = ddop ? Color.LightGray : Color.White;
            DragOrPush.TextDecorations = ddop ? TextDecorations.None : TextDecorations.Underline;
            DragOrPush.Text = encumbrance.DragOrPush(sheet) + " lbs";
        }

        private void LightLoad_Tapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, encumbrance.lightLoad, "Edit Light Load", "Light Load", false);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        private void MediumLoad_Tapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, encumbrance.mediumLoad, "Edit Medium Load", "Medium Load", false);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        private void HeavyLoad_Tapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, encumbrance.heavyLoad, "Edit Heavy Load", "Heavy Load", false);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        private void LiftOverHead_Tapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            if (encumbrance.defaultLiftOverHead)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, encumbrance.liftOverHead, "Edit Lift Over Head", "Lift Over Head", false);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        private void LiftOffGround_Tapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            if (encumbrance.defaultLiftOffGround)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, encumbrance.liftOffGround, "Edit Lift Off Ground", "Lift Off Ground", false);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        private void DragOrPush_Tapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            if (encumbrance.defaultDragOrPush)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, encumbrance.dragOrPush, "Edit Drag Or Push", "Drag Or Push", false);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        private void DefaultLiftOverHead_Changed(object sender, CheckedChangedEventArgs e)
        {
            UpdateLiftOverHead();
        }

        private void DefaultLiftOffGround_Changed(object sender, CheckedChangedEventArgs e)
        {
            UpdateLiftOffGround();
        }

        private void DefaultDragOrPush_Changed(object sender, CheckedChangedEventArgs e)
        {
            UpdateDragOrPush();
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