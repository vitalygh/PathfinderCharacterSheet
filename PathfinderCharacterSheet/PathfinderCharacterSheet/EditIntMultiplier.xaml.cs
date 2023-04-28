using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using PathfinderCharacterSheet.CharacterSheets.V1;
using RoundingTypesPickerItem = System.Tuple<string, PathfinderCharacterSheet.CharacterSheets.V1.RoundingType>;

namespace PathfinderCharacterSheet
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditIntMultiplier : ContentPage, ISheetView
    {
        private Page pushedPage = null;
        private IntMultiplier source = null;
        private IntMultiplier multiplier = null;

        public EditIntMultiplier()
        {
            InitializeComponent();
        }

        public void Init(IntMultiplier multiplier)
        {
            source = multiplier;
            this.multiplier = source.Clone;

            AdditionalBefore.Text = multiplier.AdditionalBefore.ToString();
            Multiplier.Text =  multiplier.Multiplier.ToString();
            Divider.Text = multiplier.Divider.ToString();
            AdditionalAfter.Text = multiplier.AdditionalAfter.ToString();

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
                roundingTypes.Add(new RoundingTypesPickerItem(v.ToString(), value));
            }
            Rounding.ItemsSource = roundingTypes;
            Rounding.SelectedIndex = roundingSelectedIndex;

            UpdateView();
        }

        public void UpdateView()
        {
            pushedPage = null;
            Limit.Text = multiplier.Limit?.AsString() ?? string.Empty;
        }

        private void Limit_Tapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var el = new EditIntLimit();
            if (multiplier.Limit == null)
                multiplier.Limit = new IntLimit();
            el.Init(multiplier.Limit);
            pushedPage = el;
            Navigation.PushAsync(pushedPage);
        }

        private void EditToView()
        {
            var additionalBefore = multiplier.AdditionalBefore;
            UIHelpers.StrToInt(AdditionalBefore.Text, ref additionalBefore);
            multiplier.AdditionalBefore = additionalBefore;
            var multi = multiplier.Multiplier;
            UIHelpers.StrToInt(Multiplier.Text, ref multi);
            multiplier.Multiplier = multi;
            var divider = multiplier.Divider;
            UIHelpers.StrToInt(Divider.Text, ref divider);
            multiplier.Divider = divider;
            var additionalAfter = multiplier.AdditionalAfter;
            UIHelpers.StrToInt(AdditionalAfter.Text, ref additionalAfter);
            multiplier.AdditionalAfter = additionalAfter;
            var currentRoundingType = IntMultiplier.DefaultRounding;
            if (Rounding != null)
            {
                if (Rounding.SelectedItem is RoundingTypesPickerItem item)
                    currentRoundingType = item.Item2;
            }
            multiplier.RoundingType = currentRoundingType;

            if (multiplier.Limit.Equals(new IntLimit()))
                multiplier.Limit = null;

            if (!source.Equals(multiplier))
            {
                source.Fill(multiplier);
                //UIMediator.OnCharacterSheetChanged?.Invoke();
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