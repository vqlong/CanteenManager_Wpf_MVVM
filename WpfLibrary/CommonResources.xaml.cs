using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfLibrary
{
    public partial class CommonResources : ResourceDictionary
    {
        CommonResources()
        {
            InitializeComponent();
        }

        public static CommonResources Default { get; } = new CommonResources();
    }
}
