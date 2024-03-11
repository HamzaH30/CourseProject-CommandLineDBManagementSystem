using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MenuClasses
{
    public interface IMenuItem
    {
        string SelectionText { get; init; }
        public void PerformMenuAction();
    }
}
