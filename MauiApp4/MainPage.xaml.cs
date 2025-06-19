namespace MauiApp4
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }

        
        private void OnTemperatureChanged(object sender, ValueChangedEventArgs e)
        {
            int temp = (int)e.NewValue;
            TemperatureLabel.Text = $"현재 온도: {temp} K";

            // 여기서 추가로 물리 계산이나 시뮬레이션에 반영해도 됨
            // 예: UpdateParticleSpeed(temp);
        }
        private void OnValueChanged(object sender, ValueChangedEventArgs e)
        {
            int val = (int)e.NewValue;
            ValueLabel.Text = $"현재 부피: {val} m^3";

            // 여기서 추가로 물리 계산이나 시뮬레이션에 반영해도 됨
            // 예: UpdateParticleSpeed(temp);
        }
        private void OnPressureChanged(object sender, ValueChangedEventArgs e)
        {
            int press = (int)e.NewValue;
            PressureLabel.Text = $"현재 압력: {press} hpa";

            // 여기서 추가로 물리 계산이나 시뮬레이션에 반영해도 됨
            // 예: UpdateParticleSpeed(temp);
        }
        private double n = 1.0; // 기본값

        private void OnEntryTextChanged(object sender, TextChangedEventArgs e)
        {
            // 입력값을 int로 변환
            if (int.TryParse(e.NewTextValue, out int result))
            {
                MyResultLabel.Text = $"입력한 숫자: {result}";
            }
            else
            {
                MyResultLabel.Text = "정수를 입력하세요!";
            }
        }
        private void OnCheckBoxChanged(object sender, CheckedChangedEventArgs e)
        {
            if (sender is CheckBox selected && selected.IsChecked)
            {
                // 모든 체크박스를 배열로 묶고
                var checkBoxes = new[] { Chk1, Chk2, Chk3, Chk4 };

                foreach (var cb in checkBoxes)
                {
                    // 선택된 체크박스만 제외하고 전부 false로
                    if (cb != selected)
                        cb.IsChecked = false;
                }
            }
        }

    }

}
