using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace DrawLots
{
    internal class WindowService : IWindowService
    {
        private Dictionary<Type, Type> _dic;
        private Dictionary<Type, Window> _windows = new Dictionary<Type, Window>();

        public WindowService()
        {
            _dic = new Dictionary<Type, Type>()
            {
                { typeof(NewSessionViewModel), typeof(NewSessionWindow) }
            };
        }

        public void Close(object vm, bool? result = null)
        {
            var win = _windows[vm.GetType()];
            win.DialogResult = result;
            win.Close();
        }

        public bool? OpenDialog(object vm)
        {
            var windowType = _dic[vm.GetType()];
            Window win = (Window)Activator.CreateInstance(windowType);
            win.Owner = App.Current.Windows.OfType<Window>().FirstOrDefault(a => a.IsActive);
            _windows.Add(vm.GetType(), win);
            win.Closed += Win_Closed;
            win.DataContext = vm;
            return win.ShowDialog();
        }

        private void Win_Closed(object sender, EventArgs e)
        {
            _windows.Remove((sender as Window).DataContext.GetType());
        }
    }
}
