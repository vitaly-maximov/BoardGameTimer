using System;
using System.Windows.Media;
using BoardGameTimer.Model;
using GalaSoft.MvvmLight;

namespace BoardGameTimer.ViewModel
{
    public class UserViewModel : ViewModelBase
    {
        public UserViewModel(UserModel userModel, Color color)
        {
            Model = userModel;
            Color = color;
        }

        #region public methods

        public UserModel Model { get; }

        public Color Color { get; }

        public bool IsSelected { get; set; }

        public TimeSpan SpentTime { get; set; }

        #endregion
    }
}
