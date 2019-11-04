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
	public partial class EditLanguages : ContentPage, ISheetView
    {
        public class LanguageRow
        {
            public Frame frame = null;
            public Label name = null;
        }
        private Page pushedPage = null;
        private List<LanguageRow> languageRows = new List<LanguageRow>();
        private List<string> languages = null;

        public EditLanguages()
        {
            InitializeComponent();
        }

        public void InitEditor()
        {
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            languages = new List<string>();
            foreach (var l in sheet.languages)
                languages.Add(l);
            UpdateView();
        }

        public void UpdateView()
        {
            pushedPage = null;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
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
                EventHandler handler = (s, e) => Language_Tap(language, languageIndex);
                row.frame.GestureRecognizers.Clear();
                MainPage.AddTapHandler(row.frame, handler, 1);
            }
            var count = languagesCount - rowsCount;
            if (count > 0)
            {
                for (var i = 0; i < count; i++)
                {
                    var languageIndex = updateCount + i;
                    var language = languages[languageIndex];
                    var row = new LanguageRow();
                    row.frame  = CreateFrame(language);
                    row.name = row.frame.Content as Label;
                    EventHandler handler = (s, e) => Language_Tap(language, languageIndex);
                    MainPage.AddTapHandler(row.frame, handler, 2);
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

        private Label CreateLabel(string text, TextAlignment horz = TextAlignment.Center)
        {
            return MainPage.CreateLabel(text, horz);
        }

        private Frame CreateFrame(string text)
        {
            return MainPage.CreateFrame(text);
        }

        private void EditToView()
        {
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var hasChanges = !CharacterSheet.IsEqual(sheet.languages, languages);
            if (hasChanges)
            {
                sheet.languages = languages;
                CharacterSheetStorage.Instance.SaveCharacter();
            }
        }

        public void Language_Tap(string language = null, int index = -1)
        {
            if (pushedPage != null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
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
            Navigation.PopAsync();
        }

        private void Save_Clicked(object sender, EventArgs e)
        {
            EditToView();
            Navigation.PopAsync();
        }
    }
}