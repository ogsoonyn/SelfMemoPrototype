using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Input;

namespace SelfMemoPrototype.Behavior
{
    class ApplicationCommandBehavior : Behavior<Window>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            CommandBinding CloseCommandBinding = new CommandBinding(
                ApplicationCommands.Close,
                CloseCommandExecuted,
                CloseCommandCanExecute);
            AssociatedObject.CommandBindings.Add(CloseCommandBinding);
        }

        private void CloseCommandExecuted(object target, ExecutedRoutedEventArgs e)
        {
            AssociatedObject.Close();
            e.Handled = true;
        }

        private void CloseCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }
    }

    public static class FocusExtension
    {
        public static bool GetIsFocused(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsFocusedProperty);
        }

        public static void SetIsFocused(DependencyObject obj, bool value)
        {
            obj.SetValue(IsFocusedProperty, value);
        }

        public static readonly DependencyProperty IsFocusedProperty =
            DependencyProperty.RegisterAttached(
                "IsFocused", typeof(bool), typeof(FocusExtension),
                new UIPropertyMetadata(false, OnIsFocusedPropertyChanged));

        private static void OnIsFocusedPropertyChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var uie = (UIElement)d;
            if ((bool)e.NewValue)
            {
                uie.Focus();
            }
        }
    }
}
