namespace MauiApp4
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            count++;

            if (count == 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                CounterBtn.Text = $"Clicked {count} times";

            SemanticScreenReader.Announce(CounterBtn.Text);
        }
        private void OnTemperatureChanged(object sender, ValueChangedEventArgs e)
        {
            int temp = (int)e.NewValue;
            TemperatureLabel.Text = $"현재 온도: {temp} K";

            // 여기서 추가로 물리 계산이나 시뮬레이션에 반영해도 됨
            // 예: UpdateParticleSpeed(temp);
        }
    }

}
