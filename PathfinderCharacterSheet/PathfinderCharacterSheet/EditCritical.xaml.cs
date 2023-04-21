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
    public partial class EditCritical : ContentPage, ISheetView
    {
        private Page pushedPage = null;
        private CharacterSheet sheet = null;
        private CriticalHit source = null;
        private CriticalHit critical = null;

        public EditCritical()
        {
            InitializeComponent();
        }

        public void UpdateView()
        {
            pushedPage = null;
            if (sheet == null)
                return;
            CriticalMin.Text = critical.min.GetValue(sheet).ToString();
            CriticalMax.Text = critical.max.GetValue(sheet).ToString();
            Multiplier.Text = critical.multiplier.GetValue(sheet).ToString();
        }

        private void EditToView()
        {
            if (!critical.Equals(source))
            {
                source.Fill(critical);
                MainPage.SaveSelectedCharacter?.Invoke();
            }
        }

        public void Init(CharacterSheet sheet, CriticalHit critical)
        {
            if (sheet == null)
                return;
            if (critical == null)
                return;
            this.sheet = sheet;
            source = critical;
            this.critical = critical.Clone as CriticalHit;
            UpdateView();
        }

        private void Min_Tapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            if (sheet == null)
                return;
            if (critical == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, critical.min, "Edit Critical Min", "Min", false);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        private void Max_Tapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            if (sheet == null)
                return;
            if (critical == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, critical.max, "Edit Critical Max", "Max", false);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        private void Multiplier_Tapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            if (sheet == null)
                return;
            if (critical == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, critical.multiplier, "Edit Critical Multiplier", "Multiplier", false);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
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