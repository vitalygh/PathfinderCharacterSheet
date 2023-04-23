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
    public partial class EditIntMultiplier : ContentPage, ISheetView
    {
        private Page pushedPage = null;
        private IntMultiplier source = null;
        private IntMultiplier multiplier = null;
        private static readonly IntLimit emptyLimit = new IntLimit();

        public EditIntMultiplier()
        {
            InitializeComponent();
        }

        public void Init(IntMultiplier multiplier)
        {
            source = multiplier;
            this.multiplier = source.Clone;

            AdditionalBefore.Text = multiplier.additionalBefore.ToString();
            Multiplier.Text =  multiplier.multiplier.ToString();
            Divider.Text = multiplier.divider.ToString();
            AdditionalAfter.Text = multiplier.additionalAfter.ToString();

            var roundingTypes = new List<RoundingTypesPickerItem>();
            var roundingValues = Enum.GetValues(typeof(RoundingType));
            var roundingIndex = -1;
            var roundingSelectedIndex = -1;
            var roundingSelectedValue = multiplier.RoundingType;
            foreach (var v in roundingValues)
            {
                var value = (RoundingType)v;
                roundingIndex += 1;
                if (roundingSelectedValue == value)
                    roundingSelectedIndex = roundingIndex;
                roundingTypes.Add(new RoundingTypesPickerItem()
                {
                    Name = v.ToString(),
                    Value = value,
                });
            }
            Rounding.ItemsSource = roundingTypes;
            Rounding.SelectedIndex = roundingSelectedIndex;

            UpdateView();
        }

        public void UpdateView()
        {
            pushedPage = null;
            Limit.Text = multiplier.limit?.AsString() ?? string.Empty;
        }

        private void Limit_Tapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var el = new EditIntLimit();
            if (multiplier.limit == null)
                multiplier.limit = emptyLimit.Clone;
            el.Init(multiplier.limit);
            pushedPage = el;
            Navigation.PushAsync(pushedPage);
        }

        private void EditToView()
        {
            MainPage.StrToInt(AdditionalBefore.Text, ref multiplier.additionalBefore);
            MainPage.StrToInt(Multiplier.Text, ref multiplier.multiplier);
            MainPage.StrToInt(Divider.Text, ref multiplier.divider);
            MainPage.StrToInt(AdditionalAfter.Text, ref multiplier.additionalAfter);
            var currentRoundingType = IntMultiplier.DefaultRounding;
            if (Rounding != null)
            {
                var item = (Rounding.SelectedItem as RoundingTypesPickerItem);
                if (item != null)
                    currentRoundingType = item.Value;
            }
            multiplier.RoundingType = currentRoundingType;

            if (multiplier.limit == emptyLimit)
                multiplier.limit = null;

            if (!source.Equals(multiplier))
            {
                source.Fill(multiplier);
                //MainPage.OnCharacterSheetChanged?.Invoke();
            }
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