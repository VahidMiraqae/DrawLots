using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DrawLots
{
    internal class MainViewModel : BaseViewModel
    {
        private ObservableCollection<SessionViewModel> _sessions = new ObservableCollection<SessionViewModel>();
        private IWindowService _windowService;
        private SessionViewModel _selectedSession;
        private DrawingDataContext _dataContext;
        private string _drawLotsCaption = DRAW_LOTS_CAPTIONS;
        private PerformingDrawingLotsStage _stage;
        private CancellationTokenSource cancellationTokenSource;
        private const string DRAW_LOTS_CAPTIONS = "DRAW";
        private bool _showClickToCancel = false;

        public MainViewModel(IWindowService windowService, DrawingDataContext dataContext)
        {
            _windowService = windowService;
            _dataContext = dataContext;
            Sessions = new ObservableCollection<SessionViewModel>(dataContext.Sessions);
            CreateNewSessionCommand = new CommandBase(CreateNewSession, CanCreateNewSession);
            RemoveNewSessionCommand = new CommandBase(RemoveSession, a => SelectedSession != null);
            DrawLotsCommand = new AsyncCommand(PerformDrawingLots, CanPerformDrawingLots);
            RepeatSessionCommand = new CommandBase(RepeatSession, CanRepeatSession);
        }

        public ObservableCollection<SessionViewModel> Sessions { get => _sessions; set => OnPropChanged(ref _sessions, value); }
        public SessionViewModel SelectedSession
        {
            get => _selectedSession; set
            {
                OnPropChanged(ref _selectedSession, value);
                RemoveNewSessionCommand.OnCanExecuteChanged();
                DrawLotsCommand.OnCanExecuteChanged();
                RepeatSessionCommand.OnCanExecuteChanged();
            }
        }
        public CommandBase CreateNewSessionCommand { get; }
        public CommandBase RemoveNewSessionCommand { get; }
        public AsyncCommand DrawLotsCommand { get; }
        public CommandBase RepeatSessionCommand { get; }
        public string DrawLotsCaption { get => _drawLotsCaption; set => OnPropChanged(ref _drawLotsCaption, value); }
        public bool ShowClickToCancel { get => _showClickToCancel; set => OnPropChanged(ref _showClickToCancel, value); }

        public PerformingDrawingLotsStage DrawingLotsStage { get => _stage; set => OnPropChanged(ref _stage, value); }

        private void RepeatSession(object obj)
        {
            foreach (var item in SelectedSession.Participants)
            {
                item.DateWon = null;
            }
            SelectedSession.PropsChanged();
            DrawLotsCommand.OnCanExecuteChanged();
            RepeatSessionCommand.OnCanExecuteChanged();
            Save();
        }

        private bool CanRepeatSession(object arg)
        {
            return SelectedSession == null ? false : SelectedSession.RemainingCount == 0;
        }

        private async Task PerformDrawingLots(object arg)
        {
            if (DrawingLotsStage == PerformingDrawingLotsStage.None)
            {
                cancellationTokenSource = new CancellationTokenSource();
                DrawingLotsStage = PerformingDrawingLotsStage.Suspension;
                ShowClickToCancel = true;
                var task = Task.Run(() =>
                {
                    var remaining = 3;
                    while (remaining > 0)
                    {
                        App.Current.Dispatcher.Invoke(() => { DrawLotsCaption = remaining.ToString(); });
                        Thread.Sleep(1000);
                        cancellationTokenSource.Token.ThrowIfCancellationRequested();
                        remaining--;
                    }
                }, cancellationTokenSource.Token);
                try
                {
                    await task;
                    ShowClickToCancel = false;
                }
                catch
                {
                }
                if (task.IsCanceled)
                {
                    DrawingLotsStage = PerformingDrawingLotsStage.None;
                    DrawLotsCaption = DRAW_LOTS_CAPTIONS;
                    ShowClickToCancel = false;
                    return;
                }

                var array = SelectedSession.Remaining.ToArray();
                var now = DateTime.Now;
                var rnd = new Random(Guid.NewGuid().GetHashCode());
                var ind = rnd.Next(array.Length);
                array[ind].DateWon = now;
                if (array.Length == 2)
                {
                    array[ind == 0 ? 1 : 0].DateWon = now.AddSeconds(1);
                }

                SelectedSession.PropsChanged();
                DrawLotsCaption = DRAW_LOTS_CAPTIONS;
                DrawingLotsStage = PerformingDrawingLotsStage.None;
                DrawLotsCommand.OnCanExecuteChanged();
                RepeatSessionCommand.OnCanExecuteChanged();
                Save();
            }
            else if (DrawingLotsStage == PerformingDrawingLotsStage.Suspension)
            {
                cancellationTokenSource.Cancel();
                ShowClickToCancel = false;
                DrawLotsCaption = "Cancelling ...";
            }
        }

        private bool CanPerformDrawingLots(object arg)
        {
            return SelectedSession != null
                && SelectedSession.RemainingCount > 0;
        }

        private void RemoveSession(object obj)
        {
            Sessions.Remove(SelectedSession);
            Save();
        }

        private void Save()
        {
            _dataContext.Sessions = Sessions.ToList();
            _dataContext.Save();
        }

        private void CreateNewSession(object obj)
        {
            var session = new NewSessionViewModel(Sessions, _windowService);
            var result = _windowService.OpenDialog(session);
            if (result == true)
            {
                Sessions.Add(session.NewSession);
                Save();
            }
        }

        private bool CanCreateNewSession(object arg)
        {
            return true;
        }
    }
}
