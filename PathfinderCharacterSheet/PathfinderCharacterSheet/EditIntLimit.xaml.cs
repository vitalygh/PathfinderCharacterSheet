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
    public partial class EditIntLimit : ContentPage, ISheetView
    {
        private Page pushedPage = null;
        private CharacterSheet.IntLimit source = null;
        private CharacterSheet.IntLimit limit = null;

        public EditIntLimit()
        {
            InitializeComponent();
        }

        public void Init(CharacterSheet.IntLimit limit)
        {
            if (limit == null)
                return;
            source = limit;
            this.limit = source.Clone as CharacterSheet.IntLimit;
            UpdateView();
        }

        public void UpdateView()
        {
            pushedPage = null;
            MinLimit.IsChecked = limit.minLimit;
            MinValue.Text = limit.minValue.ToString();
            MaxLimit.IsChecked = limit.maxLimit;
            MaxValue.Text = limit.maxValue.ToString();
            UpdateMinValue();
            UpdateMaxValue();
        }

        private void EditToView()
        {
            if (limit == null)
                return;
            MainPage.StrToInt(MinValue.Text, ref limit.minValue);
            MainPage.StrToInt(MaxValue.Text, ref limit.maxValue);
            if (!source.Equals(limit))
            {
                source.Fill(limit);
                //CharacterSheetStorage.Instance.SaveCharacter();
            }
        }

        private void UpdateMinValue()
        {
            var min = MinLimit.IsChecked;
            limit.minLimit = min;
            MinValue.IsEnabled = min;
        }

        private void MinLimit_Changed(object sender, CheckedChangedEventArgs e)
        {
            UpdateMinValue();
        }

        private void UpdateMaxValue()
        {
            var max = MaxLimit.IsChecked;
            limit.maxLimit = max;
            MaxValue.IsEnabled = max;
        }

        private void MaxLimit_Changed(object sender, CheckedChangedEventArgs e)
        {
            UpdateMaxValue();
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