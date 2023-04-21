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
	public partial class EditLanguages : ContentPage, ISheetView
    {
        public class LanguageRow
        {
            public Frame frame = null;
            public Label name = null;
        }
        private Page pushedPage = null;
        private readonly List<LanguageRow> languageRows = new List<LanguageRow>();
        private List<string> languages = null;

        public EditLanguages()
        {
            InitializeComponent();
        }

        public void InitEditor()
        {
            var sheet = MainPage.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            languages = new List<string>();
            if (sheet.languages != null)
                foreach (var l in sheet.languages)
                    languages.Add(l);
            UpdateView();
        }

        public void UpdateView()
        {
            pushedPage = null;
            var sheet = MainPage.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            var languagesCount = languages.Count;
            var rowsCount = languageRows.Count;
            var updateCount = Math.Min(languagesCount, rowsCount);
            for (var i = 0; i < updateCount; i++)
            {
                var languageIndex = i;
                var row = languageRows[i];
                var language = languages[i];
                row.name.Text = language;
                MainPage.SetTapHandler(row.frame, (s, e) => Language_Tap(language, languageIndex), 1);
            }
            var count = languagesCount - rowsCount;
            if (count > 0)
            {
                for (var i = 0; i < count; i++)
                {
                    var languageIndex = updateCount + i;
                    var language = languages[languageIndex];
                    var row = new LanguageRow()
                    {
                        frame = CreateFrame(language),
                    };
                    row.frame  = CreateFrame(language);
                    row.name = row.frame.Content as Label;
                    row.name.TextDecorations = TextDecorations.Underline;
                    MainPage.SetTapHandler(row.frame, (s, e) => Language_Tap(language, languageIndex), 1);
                    languageRows.Add(row);
                    var rowIndex = languageIndex;
                    Languages.Children.Add(row.frame, 0, rowIndex);
                }
            }
            else if (count < 0)
            {
                while (languageRows.Count > languages.Count)
                {
                    var last = languageRows.Count - 1;
                    var row = languageRows[last];
                    languageRows.RemoveAt(last);
                    Languages.Children.Remove(row.frame);
                }
            }
            rowsCount = languageRows.Count;
            if ((Languages.RowDefinitions == null) || (Languages.RowDefinitions.Count != rowsCount))
            {
                Languages.RowDefinitions = new RowDefinitionCollection();
                for (int i = 0; i < rowsCount; i++)
                {
                    Languages.RowDefinitions.Add(new RowDefinition()
                    {
                        Height = GridLength.Auto,
                    });
                }
            }
        }

        private Frame CreateFrame(string text)
        {
            return MainPage.CreateFrame(text);
        }

        private void EditToView()
        {
            var sheet = MainPage.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            if ((languages is null) && (sheet.languages is null))
                return;
            if (!(languages is null) && !(sheet.languages is null) && ReferenceEquals(sheet.languages, languages))
                return;
            sheet.languages = languages;
            MainPage.SaveSelectedCharacter?.Invoke();
        }

        public void Language_Tap(string language = null, int index = -1)
        {
            if (pushedPage != null)
                return;
            var sheet = MainPage.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            var el = new EditLanguage();
            el.InitEditor(languages, language, index);
            pushedPage = el;
            Navigation.PushAsync(pushedPage);
        }

        private void AddLanguage_Clicked(object sender, EventArgs e)
        {
            Language_Tap();
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