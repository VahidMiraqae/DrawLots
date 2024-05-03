using EncryptStringSample;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace DrawLots
{
    internal class DrawingDataContext
    {
        private DataObject _obj = new DataObject();
        private string _dbFilename = "data.json";
        private const string _encryption = "cd5b0aa6-2ee2-4b73-afb4-3e849892fc45";

        public DrawingDataContext()
        {
            ReadData();
        }

        public List<SessionViewModel> Sessions { get; internal set; } = new List<SessionViewModel>();

        private void ReadData()
        {
            if (File.Exists(_dbFilename))
            {
                var content = File.ReadAllText(_dbFilename);
                content = Decrypt(content);
                _obj = JsonConvert.DeserializeObject<DataObject>(content);
                Sessions = ToViewModel(_obj.Sessions);
            }
        }

        private string Decrypt(string content)
        {
            return StringCipher.Decrypt(content, _encryption);
        }

        private List<SessionViewModel> ToViewModel(List<DrawingSession> sessions)
        {
            return sessions.Select(a =>
            {
                return new SessionViewModel()
                {
                    Title = string.Copy(a.Title),
                    Participants = new ObservableCollection<ParticipantViewModel>(a.Participants.Select(b =>
                    {
                        return new ParticipantViewModel()
                        {
                            Name = string.Copy(b.Name),
                            Id = b.Id,
                            DateWon = b.DateWon
                        };
                    })),
                };
            }).ToList();
        }



        internal void Save()
        {
            var model = ToModel(Sessions);
            _obj.Sessions = model;
            var json = JsonConvert.SerializeObject(_obj, Formatting.Indented);
            json = Encrypt(json);
            File.WriteAllText(_dbFilename, json);
        }

        private string Encrypt(string json)
        {
            return StringCipher.Encrypt(json, _encryption);
        }

        private List<DrawingSession> ToModel(List<SessionViewModel> sessions)
        {
            return sessions.Select(a =>
            {
                return new DrawingSession()
                {
                    Title = string.Copy(a.Title),
                    Participants = a.Participants.Select(b => new Participant()
                    {
                        Name = string.Copy(b.Name),
                        Id = b.Id,
                        DateWon = b.DateWon
                    }).ToList()
                };
            }).ToList();
        }
    }
}