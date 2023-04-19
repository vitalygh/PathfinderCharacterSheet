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
        public class RoundingTypesPickerItem
        {
            public string Name { set; get; }
            public IntMultiplier.RoundingTypes Value { set; get; }
        }

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
            this.multiplier = source.Clone as IntMultiplier;

            AdditionalBefore.Text = multiplier.additionalBefore.ToString();
            Multiplier.Text =  multiplier.multiplier.ToString();
            Divider.Text = multiplier.divider.ToString();
            AdditionalAfter.Text = multiplier.additionalAfter.ToString();

            var roundingTypes = new List<RoundingTypesPickerItem>();
            var roundingValues = Enum.GetValues(typeof(IntMultiplier.RoundingTypes));
            var roundingIndex = -1;
            var roundingSelectedIndex = -1;
            var roundingSelectedValue = multiplier != null ? multiplier.RoundingType : IntMultiplier.DefaultRounding;
            foreach (var v in roundingValues)
            {
                var value = (IntMultiplier.RoundingTypes)v;
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
            Limit.Text = multiplier.limit.AsString();
        }

        private void Limit_Tapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var el = new EditIntLimit();
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

            if (!source.Equals(multiplier))
            {
                source.Fill(multiplier);
                //CharacterSheetStorage.Instance.SaveCharacter();
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