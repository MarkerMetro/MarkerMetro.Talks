using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using MetroTalks.Win81.PetesEats.Annotations;
using MetroTalks.Win81.PetesEats.Services;

namespace MetroTalks.Win81.PetesEats.ViewModels
{
    public class EatItemViewModel : INotifyPropertyChanged
    {

        private string _name;
        private string _description;
        private string _imageUri;
        private readonly PetesEatsService _peteEatsService;


        public EatItemViewModel()
        {
            _peteEatsService = new PetesEatsService();
        }

        public int Id { get; set; }

        public string Name
        {
            get { return _name; }
            set
            {
                if (value == _name) return;
                _name = value;
                OnPropertyChanged();
            }
        }

        public string Description
        {
            get { return _description; }
            set
            {
                if (value == _description) return;
                _description = value;
                OnPropertyChanged();
            }
        }

        public string ImageUri
        {
            get { return _imageUri; }
            set
            {
                if (value == _imageUri) return;
                _imageUri = value;
                OnPropertyChanged();
            }
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public async Task BindItem(int id)
        {
            Id = id;

            var eat = await _peteEatsService.GetEat(Id);
            Name = eat.Name;
            Description = eat.Description;
            ImageUri = eat.Image500.Url;
        }
    }
}