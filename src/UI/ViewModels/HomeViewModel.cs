using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        public HomeViewModel()
        {
            List<List<int>> Lessons = new List<List<int>>();

            for (int i = 0; i < 5; i++)
            {
                Lessons.Add(new List<int>());

                for (int j = 0; j < 5; j++)
                {
                    Lessons[i].Add(i * 10 + j);
                }
            }

            OnPropertyChanged(nameof(Lessons));
        }
    }
}
