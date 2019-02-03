using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

using BoardGameTimer.Core;
using BoardGameTimer.Model;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BoardGameTimer.ViewModel.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly Timer m_timer;

        public MainViewModel()
        {
            Users = LoadUsers();

            m_timer = new Timer(Users.Select(user => user.Model).ToArray());

            ResetTimer = new RelayCommand(OnResetTimer);
            ToggleTimer = new RelayCommand(OnToggleTimer);
            MoveToNextUser = new RelayCommand(OnMoveToNextUser);
            MoveToPreviousUser = new RelayCommand(OnMoveToPreviousUser);

            Poll();
        }

        #region public methods

        public IReadOnlyList<UserViewModel> Users { get; }

        public ICommand ResetTimer { get; }

        public ICommand ToggleTimer { get; }

        public ICommand MoveToNextUser { get; }

        public ICommand MoveToPreviousUser { get; }

        #endregion

        #region private methods

        // TODO:
        private IReadOnlyList<UserViewModel> LoadUsers()
        {
            string timerSettingsText = File.ReadAllText("TimerSettings.json");
            dynamic timerSettingsJson = JsonConvert.DeserializeObject<dynamic>(timerSettingsText);

            UserModel[] userModels = ((JArray)timerSettingsJson.users)
                .Select(userName => new UserModel((string) userName))
                .ToArray();

            string gameText = File.ReadAllText((string)timerSettingsJson.game);
            dynamic gameJson = JsonConvert.DeserializeObject<dynamic>(gameText);

            var colorConverter = new ColorConverter();
            Color[] colors = ((JArray)gameJson.colors)
                .Select(colorName => (Color)colorConverter.ConvertFrom((string)colorName))
                .OrderBy(color => Guid.NewGuid())
                .ToArray();

            var random = new Random();
            int firstPlayerIndex = random.Next(userModels.Length);

            return userModels
                .Skip(firstPlayerIndex)
                .Concat(userModels.Take(firstPlayerIndex))
                .Select((userModel, index) => new UserViewModel(userModel, colors[index]))
                .ToArray();
        }

        private async void Poll()
        {
            OnPoll();

            await Task.Delay(50);

            Poll();
        }

        private void OnPoll()
        {
            m_timer.Update();

            foreach (UserViewModel user in Users)
            {
                user.IsSelected = (m_timer.GetCurrentUser() == user.Model);
                user.SpentTime = m_timer.GetUserSpentTime(user.Model);
            }
        }

        private void OnResetTimer()
        {
            m_timer.Reset();
        }

        private void OnToggleTimer()
        {
            if (m_timer.IsStarted)
            {
                m_timer.Stop();
            }
            else
            {
                m_timer.Start();
            }
        }

        private void OnMoveToNextUser()
        {
            m_timer.Next();
        }

        private void OnMoveToPreviousUser()
        {
            m_timer.Previous();
        }

        #endregion
    }
}