﻿using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace PathfinderCharacterSheet
{
    public class LongPressedEffect : RoutingEffect
    {
        public LongPressedEffect() : base("PathfinderCharacterSheet.LongPressedEffect")
        {
        }

        public static readonly BindableProperty ActionProperty = BindableProperty.CreateAttached("Action", typeof(Action), typeof(LongPressedEffect), (object)null);
        public static Action GetAction(BindableObject view)
        {
            return (Action)view.GetValue(ActionProperty);
        }

        public static void SetAction(BindableObject view, Action value)
        {
            view.SetValue(ActionProperty, value);
        }

        public static readonly BindableProperty CommandProperty = BindableProperty.CreateAttached("Command", typeof(ICommand), typeof(LongPressedEffect), (object)null);
        public static ICommand GetCommand(BindableObject view)
        {
            return (ICommand)view.GetValue(CommandProperty);
        }

        public static void SetCommand(BindableObject view, ICommand value)
        {
            view.SetValue(CommandProperty, value);
        }


        public static readonly BindableProperty CommandParameterProperty = BindableProperty.CreateAttached("CommandParameter", typeof(object), typeof(LongPressedEffect), (object)null);
        public static object GetCommandParameter(BindableObject view)
        {
            return view.GetValue(CommandParameterProperty);
        }

        public static void SetCommandParameter(BindableObject view, object value)
        {
            view.SetValue(CommandParameterProperty, value);
        }
    }
}
