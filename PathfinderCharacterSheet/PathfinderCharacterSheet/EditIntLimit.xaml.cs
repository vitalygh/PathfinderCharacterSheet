using PathfinderCharacterSheet.CharacterSheets.V1;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PathfinderCharacterSheet
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditIntLimit : ContentPage, ISheetView
    {
        private Page pushedPage = null;
        private IntLimit source = null;
        private IntLimit limit = null;

        public EditIntLimit()
        {
            InitializeComponent();
        }

        public void Init(IntLimit limit)
        {
            if (limit == null)
                return;
            source = limit;
            this.limit = source.Clone;
            UpdateView();
        }

        public void UpdateView()
        {
            pushedPage = null;
            MinLimit.IsChecked = limit.MinLimit;
            MinValue.Text = limit.MinValue.ToString();
            MaxLimit.IsChecked = limit.MaxLimit;
            MaxValue.Text = limit.MaxValue.ToString();
            UpdateMinValue();
            UpdateMaxValue();
        }

        private void EditToView()
        {
            if (limit == null)
                return;
            var minValue = limit.MinValue;
            UIHelpers.StrToInt(MinValue.Text, ref minValue);
            limit.MinValue = minValue;
            var maxValue = limit.MaxValue;
            UIHelpers.StrToInt(MaxValue.Text, ref maxValue);
            limit.MaxValue = maxValue;
            if (!source.Equals(limit))
            {
                source.Fill(limit);
                //UIMediator.OnCharacterSheetChanged?.Invoke();
            }
        }

        private void UpdateMinValue()
        {
            var min = MinLimit.IsChecked;
            limit.MinLimit = min;
            MinValue.IsEnabled = min;
        }

        private void MinLimit_Changed(object sender, CheckedChangedEventArgs e)
        {
            UpdateMinValue();
        }

        private void UpdateMaxValue()
        {
            var max = MaxLimit.IsChecked;
            limit.MaxLimit = max;
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