//#define LONG_TAP_INSTEAD_OF_DOUBLE_TAP
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using System.Reflection;
using PathfinderCharacterSheet.CharacterSheets.V1;

namespace PathfinderCharacterSheet
{
    public static class UIHelpers
    {
        public static string GetBuildVersion()
        {
            return App.PlatformProxy?.GetVersionNumber + " (" + App.PlatformProxy?.GetBuildNumber + ") " + GetBuildDateTime()?.ToString("yyyy.MM.dd HH:mm:ss");
        }

        public static DateTime? GetBuildDateTime()
        {
            return GetBuildDateTime(Assembly.GetExecutingAssembly());
        }

        private static DateTime? GetBuildDateTime(Assembly assembly)
        {
            var attribute = assembly?.GetCustomAttribute<BuildDateAttribute>();
            return attribute?.DateTime;
        }

        public static bool StrToInt(string from, ref int to)
        {
            if (int.TryParse(from, out int i))
            {
                var changed = to != i;
                to = i;
                return changed;
            }
            return false;
        }

        public static Label CreateLabel(string text, TextAlignment horz = TextAlignment.Start)
        {
            return new Label()
            {
                Text = text,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                TextColor = Color.Black,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = horz,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Center,
                Margin = new Thickness(5, 0, 5, 0),
            };
        }

        public static Frame CreateFrame(string text)
        {
            return new Frame()
            {
                Content = CreateLabel(text, TextAlignment.Center),
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                BorderColor = Color.Black,
                Padding = 5,
            };
        }

        public static void SetTapHandler(View view, Action handler, int tapCount = 1)
        {
            SetTapHandler(view, (s, e) => handler?.Invoke(), tapCount);
        }

        public static void SetTapHandler(View view, EventHandler handler, int tapCount = 1)
        {
            view.GestureRecognizers.Clear();
#if LONG_TAP_INSTEAD_OF_DOUBLE_TAP
            view.Effects.Clear();
#endif
            AddTapHandler(view, handler, tapCount);
        }

        public static void AddTapHandler(View view, Action handler, int tapCount = 1)
        {
            AddTapHandler(view, (s, e) => handler?.Invoke(), tapCount);
        }

        public static void AddTapHandler(View view, EventHandler handler, int tapCount = 1)
        {
            if (view == null)
                return;
#if LONG_TAP_INSTEAD_OF_DOUBLE_TAP
            if (tapCount > 1)
            {
                AddLongTapHandler(view, () => handler?.Invoke(view, new EventArgs()));
                return;
            }
#endif
            var tgr = new TapGestureRecognizer()
            {
                NumberOfTapsRequired = tapCount,
            };
            tgr.Tapped += handler;
            view.GestureRecognizers.Add(tgr);
        }

        public static void SetLongTapHandler(View view, Action handler)
        {
            view.Effects.Clear();
            AddLongTapHandler(view, handler);
        }

        public static void AddLongTapHandler(View view, Action handler)
        {
            var lte = new LongPressedEffect();
            view.Effects.Add(lte);
            LongPressedEffect.SetAction(view, handler);
        }

        public static void FillIntMLGrid(Grid grid, CharacterSheet sheet, IntModifiersList modifiers, string title,
                                                Action<IntModifiersList> addModifier,
                                                Action<IntModifiersList, IntModifier> editModifier,
                                                Action<IntModifiersList> reorderModifiers,
                                                Action<IntModifiersList, IntModifier> activateModifier)
        {
            grid.Children.Clear();
            var stack = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };
            if ((reorderModifiers != null) && modifiers.Count > 1)
            {
                var reorderModifiersButton = new Button()
                {
                    FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                    TextColor = Color.Black,
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    HorizontalOptions = LayoutOptions.Start,
                    Text = "Reorder",
                };
                reorderModifiersButton.Clicked += (s, e) => reorderModifiers(modifiers);
                stack.Children.Add(reorderModifiersButton);
            }
            var stackTitle = CreateLabel(title, TextAlignment.Center);
            stack.Children.Add(stackTitle);
            if (addModifier != null)
            {
                var addModifierButton = new Button()
                {
                    FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                    TextColor = Color.Black,
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    HorizontalOptions = LayoutOptions.End,
                    Text = "Add",
                };
                addModifierButton.Clicked += (s, e) => addModifier(modifiers);
                stack.Children.Add(addModifierButton);
            }
            grid.Children.Add(stack, 0, 3, 0, 1);
            var count = modifiers.Count;
            if (count <= 0)
                return;
            grid.Children.Add(CreateLabel("Active", TextAlignment.Center), 0, 1);
            grid.Children.Add(CreateLabel("Value", TextAlignment.Center), 1, 1);
            grid.Children.Add(CreateLabel("Name", TextAlignment.Center), 2, 1);
            for (var i = 0; i < count; i++)
            {
                var index = i + 2;
                var t = modifiers[i];
                var active = new CheckBox()
                {
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                    IsChecked = t.active,
                };
                active.CheckedChanged += (s, e) =>
                {
                    t.active = active.IsChecked;
                    if (activateModifier != null)
                        activateModifier.Invoke(modifiers, t);
                };
                grid.Children.Add(active, 0, index);
                var value = CreateFrame(t.GetValue(sheet).ToString());
                if (editModifier != null)
                {
                    var valueTapped = new TapGestureRecognizer();
                    valueTapped.Tapped += (s, e) => editModifier.Invoke(modifiers, t);
                    value.GestureRecognizers.Add(valueTapped);
                }
                grid.Children.Add(value, 1, index);
                var name = CreateFrame(t.AsString(sheet));
                if (editModifier != null)
                {
                    var nameTapped = new TapGestureRecognizer();
                    nameTapped.Tapped += (s, e) => editModifier.Invoke(modifiers, t);
                    name.GestureRecognizers.Add(nameTapped);
                }
                grid.Children.Add(name, 2, index);
            }
        }

        public static void UpdateParentGrid(View view)
        {
            if (view == null)
                return;
            var grid = view.Parent as Grid;
            if (grid != null)
            {
                var r = Grid.GetRow(view);
                var c = Grid.GetColumn(view);
                var rs = Grid.GetRowSpan(view);
                var cs = Grid.GetColumnSpan(view);
                grid.Children.Remove(view);
                grid.Children.Add(view, c, c + cs, r, r + rs);
            }
            else
            {
                if (view.Parent is StackLayout sl)
                {
                    var index = sl.Children.IndexOf(view);
                    sl.Children.Remove(view);
                    sl.Children.Insert(index, view);
                }
            }
            UpdateParentGrid(view.Parent as View);
        }
    }
}
