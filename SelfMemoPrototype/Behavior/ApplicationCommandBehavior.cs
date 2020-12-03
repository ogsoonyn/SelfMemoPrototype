using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

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
}
