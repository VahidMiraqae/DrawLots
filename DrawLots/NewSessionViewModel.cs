using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace DrawLots
{
    internal class NewSessionViewModel : BaseViewModel
    {
        private string _title;
        private ObservableCollection<ParticipantViewModel> _participants = new ObservableCollection<ParticipantViewModel>();
        private IWindowService _windowService;
        private SessionViewModel _sessionToCopy;
        public SessionViewModel NewSession { get; set; }

        public NewSessionViewModel(IEnumerable<SessionViewModel> session, IWindowService windowService)
        {
            _windowService = windowService;
            Sessions = session.ToArray();
            AcceptNewSessionCommand = new CommandBase(AcceptNewSession, CanAcceptNewSession);
            CancelNewSessionCommand = new CommandBase(a => _windowService.Close(this), a => true);
            ResetNewSessionCommand = new CommandBase(ResetNewSession, a => true);
            CopySessionToNewSessionCommand = new CommandBase(CopySessionToNewSession, a => true);
            Participants.CollectionChanged += Participants_CollectionChanged;
        }

        public string Title { get => _title; set => OnPropChanged(ref _title, value); }
        public ObservableCollection<ParticipantViewModel> Participants { get => _participants; set => OnPropChanged(ref _participants, value); }

        private void AcceptNewSession(object obj)
        {
            NewSession = new SessionViewModel
            {
                Title = string.Copy(Title),
                Participants = new ObservableCollection<ParticipantViewModel>(Participants)
            };
            _windowService.Close(this, true);
        }

        private void CopySessionToNewSession(object obj)
        {
            if (SelectedSessionToCopy is null)
            {
                return;
            }

            Participants.Clear();
            foreach (var item in SelectedSessionToCopy.Participants)
            {
                Participants.Add(new ParticipantViewModel()
                {
                    Name = string.Copy(item.Name),
                });
            }
        }

        private void ResetNewSession(object obj)
        {
            Title = string.Empty;
            Participants.Clear();
        }

        private void Participants_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            AcceptNewSessionCommand.OnCanExecuteChanged();
        }

        private bool CanAcceptNewSession(object arg)
        {
            return !string.IsNullOrWhiteSpace(Title)
                && !Sessions.Any(a => string.Equals(a.Title, Title, StringComparison.OrdinalIgnoreCase))
                && !Participants.GroupBy(a => a.Name).Any(a => a.Count() > 1)
                && Participants.Count > 1;
        }

        protected override void OnPropChanged<T>(ref T obj, T value, [CallerMemberName] string propertyName = null)
        {
            base.OnPropChanged(ref obj, value, propertyName);
            AcceptNewSessionCommand?.OnCanExecuteChanged();
        }

        public CommandBase AcceptNewSessionCommand { get; }
        public CommandBase CancelNewSessionCommand { get; }
        public CommandBase ResetNewSessionCommand { get; }
        public CommandBase CopySessionToNewSessionCommand { get; }
        public SessionViewModel SelectedSessionToCopy { get => _sessionToCopy; set => OnPropChanged(ref _sessionToCopy, value); }

        public SessionViewModel[] Sessions { get; set; }
    }
}
