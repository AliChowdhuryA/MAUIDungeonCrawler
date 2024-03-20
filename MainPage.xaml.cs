namespace DC3
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
        }

        private void OnCounterClicked(object sender, EventArgs e)
        { 

            Navigation.PushAsync(new GamePage());

            SemanticScreenReader.Announce(CounterBtn.Text);
        }
    }

}
