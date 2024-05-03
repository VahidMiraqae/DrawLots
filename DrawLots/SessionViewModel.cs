using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DrawLots
{
    internal class SessionViewModel : BaseViewModel
    {
        private string _title;
        private ObservableCollection<ParticipantViewModel> _participants;

        public SessionViewModel()
        {
            Participants = new ObservableCollection<ParticipantViewModel>();
        }

        private void Drawings_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropChanged(nameof(Progress));
        }

        public string Title { get => _title; set => OnPropChanged(ref _title, value); }
        public ObservableCollection<ParticipantViewModel> Participants { get => Order(_participants); set => OnPropChanged(ref _participants, value); }

        private ObservableCollection<ParticipantViewModel> Order(ObservableCollection<ParticipantViewModel> value)
        {
            return new ObservableCollection<ParticipantViewModel>(value.OrderBy(a => a.DateWon == null).ThenBy(a => a.DateWon).ThenBy(a => a.Name));
        }
        public int DrawnCount => Participants.Count(a => a.DateWon != null);
        public int RemainingCount => Participants.Count - DrawnCount;
        public string Progress => $"{DrawnCount} / {Participants.Count} ({RemainingCount}) - {DrawnCount * 100 / (double)Participants.Count:F0}%";
        public IEnumerable<ParticipantViewModel> Remaining => Participants.Where(a => a.DateWon == null);

        public override void PropsChanged()
        {
            OnPropChanged(nameof(Participants));
            OnPropChanged(nameof(Progress));
        }
    }
}
