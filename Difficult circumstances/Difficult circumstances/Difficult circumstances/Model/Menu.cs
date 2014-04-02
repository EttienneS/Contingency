using System.Collections.Generic;
using System.Linq;

namespace Difficult_circumstances.Model
{
    public class Menu
    {
        private int _selectedIndex;

        public Menu(Menu parent, string name, MenuAction action, object context = null)
        {
            MenuOptions = new List<Menu>();
            Name = name;
            Action = action;
            Context = context;
            SelectedIndex = 0;
            Parent = parent;
        }

        public Menu()
        {
            MenuOptions = new List<Menu>();
            SelectedIndex = 0;
            Parent = null;
        }

        public delegate void MenuAction(object context);

        public MenuAction Action { get; set; }

        public object Context { get; set; }

        public List<Menu> MenuOptions { get; set; }

        public string Name { get; set; }

        public Menu Selected
        {
            get
            {
                return MenuOptions[_selectedIndex];
            }
        }

        public int SelectedIndex
        {
            get
            {
                return _selectedIndex;
            }
            set
            {
                if (value > MenuOptions.Count - 1)
                {
                    _selectedIndex = 0;
                }
                else if (value < 0)
                {
                    _selectedIndex = MenuOptions.Count - 1;
                }
                else
                {
                    _selectedIndex = value;
                }
            }
        }

        internal void AddOption(string name, MenuAction action, object context = null)
        {
            MenuOptions.Add(new Menu(this,name, action, context));
        }

        public Menu Parent { get; set; }
    }
}