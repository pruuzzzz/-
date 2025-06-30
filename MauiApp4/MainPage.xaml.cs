namespace MauiApp4
{
    public partial class MainPage : ContentPage
    {
        public static class SharedData
        {
            public static int T = 300; // 온도 초기값
            public static int V = 30;  // 부피 초기값
            public static int P = 1000; // 압력 초기값
            public static int Q = 0;  // 열량 초기값
            public static double t = SharedData.T;  // t는 T와 동일 초기화
        }

        private double adiabaticInitialT;
        private double adiabaticInitialV;
        private double adiabaticInitialP;
        private bool adiabaticInitialized = false;

        public MainPage()
        {
            InitializeComponent();
        }

        private void OnTemperatureChanged(object sender, ValueChangedEventArgs e)
        {
            int T = (int)e.NewValue;
            TemperatureLabel.Text = $"현재 온도: {T} K";
            SharedData.T = T;
            UpdateThermodynamicDisplay();

            // SharedData.t가 변경될 때마다 MessagingCenter로 데이터를 전송
            MessagingCenter.Send(this, "UpdateTemperature", SharedData.T); // MainPage에서 CustomMoleculeComponent로 전송
        }

        private void OnValueChanged(object sender, ValueChangedEventArgs e)
        {
            int V = (int)e.NewValue;
            ValueLabel.Text = $"현재 부피: {V} m³";
            SharedData.V = V;
            UpdateThermodynamicDisplay();
        }

        private void OnPressureChanged(object sender, ValueChangedEventArgs e)
        {
            int P = (int)e.NewValue;
            PressureLabel.Text = $"현재 압력: {P} hpa";
            SharedData.P = P;
            UpdateThermodynamicDisplay();
        }

        private void OnEntryTextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(e.NewTextValue, out int Q))
            {
                MyResultLabel.Text = $"입력한 숫자: {Q}";
                SharedData.Q = Q;
                UpdateThermodynamicDisplay();
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
                var checkBoxes = new[] { Chk1, Chk2, Chk3, Chk4 };
                foreach (var cb in checkBoxes)
                {
                    if (cb != selected)
                        cb.IsChecked = false;
                }
            }
            if (Chk4.IsChecked) // 단열 과정 체크 시 초기값 저장
            {
                adiabaticInitialT = SharedData.T;
                adiabaticInitialV = SharedData.V;
                adiabaticInitialP = SharedData.P;
            }

            UpdateThermodynamicDisplay();
        }

        private void UpdateThermodynamicDisplay()
        {
            // 표에 표시된 값을 사용하여 속도 계산
            double temperature = SharedData.T; // T 사용
            double volume = SharedData.V;
            double pressure = SharedData.P;

            // 예시로 속도를 계산하는 방식: 온도에 비례한 속도 계산
            double calculatedSpeedX = temperature / 100.0;  // 예시로 온도에 비례한 속도
            double calculatedSpeedY = temperature / 200.0;  // 예시로 온도에 비례한 속도

            // 공에 계산된 속도를 전달
            MessagingCenter.Send(this, "UpdateSpeed", new { VX = calculatedSpeedX, VY = calculatedSpeedY });

            // 나머지 계산은 그대로 진행
            if (Chk1.IsChecked)
            {
                double P = SharedData.P * 100;
                double V = SharedData.V;
                double T = SharedData.T;
                double Q = SharedData.Q;
                double N = P * V / T;

                double x = ((N * T + 2 * Q / 3) - P * V) / (5 * P / 3);
                double V_prime = V + x;
                double T_prime = T + 2 * Q / (3 * N) - 2 * P * x / (3 * N);

                // UI 갱신을 UI 스레드에서 실행
                Dispatcher.Dispatch(() =>
                {
                    VolumeLabelGrid.Text = $"{Math.Round(V_prime)} m³";
                    PressureLabelGrid.Text = $"{SharedData.P} hPa";
                    TemperatureLabelGrid.Text = $"{Math.Round(T_prime)} K";
                });

                SharedData.t = T_prime;

                // SharedData.t 값이 변경될 때마다 MessagingCenter로 전송
                MessagingCenter.Send(this, "UpdateTemperature", SharedData.t); // 변경된 t 값을 CustomMoleculeComponent로 전달
            }
            else if (Chk2.IsChecked)
            {
                double P = SharedData.P * 100;
                double V = SharedData.V;
                double T = SharedData.T;
                double Q = SharedData.Q;
                double N = P * V / T;

                double T_prime = T + 2 * Q / (3 * N);
                double P_prime = N * T_prime / V;

                // UI 갱신을 UI 스레드에서 실행
                Dispatcher.Dispatch(() =>
                {
                    VolumeLabelGrid.Text = $"{Math.Round(V)} m³";
                    PressureLabelGrid.Text = $"{Math.Round(P_prime)} hPa";
                    TemperatureLabelGrid.Text = $"{Math.Round(T_prime)} K";
                });

                SharedData.t = T_prime;

                // SharedData.t 값이 변경될 때마다 MessagingCenter로 전송
                MessagingCenter.Send(this, "UpdateTemperature", SharedData.t); // 변경된 t 값을 CustomMoleculeComponent로 전달
            }
            else if (Chk3.IsChecked) // 등온 과정
            {
                double P = SharedData.P * 100;
                double V = SharedData.V;
                double T = SharedData.T;
                double Q = SharedData.Q;
                double N = P * V / T;

                double x = ((N * T + 2 * Q / 3) - P * V) / (5 * P / 3);
                double V_prime = V + x;
                double y = P - N * T / (V + x);
                double P_prime = P - y;

                // UI 갱신을 UI 스레드에서 실행
                Dispatcher.Dispatch(() =>
                {
                    VolumeLabelGrid.Text = $"{Math.Round(V_prime)} m³";
                    PressureLabelGrid.Text = $"{Math.Round(P_prime / 100)} hPa";
                    TemperatureLabelGrid.Text = $"{Math.Round(T)} K";
                });
            }
            else if (Chk4.IsChecked) // 단열 과정
            {
                double V = SharedData.V;
                double T = SharedData.T;
                double P = SharedData.P * 100;
                double gamma = 5.0 / 3.0;

                double V0 = adiabaticInitialV;
                double T0 = adiabaticInitialT;
                double P0 = adiabaticInitialP * 100;

                double V_ratio = V / V0;

                double T_prime = T0 * Math.Pow(V_ratio, 1 - gamma);
                double P_prime = P0 * Math.Pow(V0 / V, gamma);

                double x = V - V0;
                double y = P0 - P_prime;

                // UI 갱신을 UI 스레드에서 실행
                Dispatcher.Dispatch(() =>
                {
                    VolumeLabelGrid.Text = $"{Math.Round(V)} m³";
                    PressureLabelGrid.Text = $"{Math.Round(P_prime / 100)} hPa";
                    TemperatureLabelGrid.Text = $"{Math.Round(T_prime)} K";
                });

                SharedData.t = T_prime;

                // SharedData.t 값이 변경될 때마다 MessagingCenter로 전송
                MessagingCenter.Send(this, "UpdateTemperature", SharedData.t); // 변경된 t 값을 CustomMoleculeComponent로 전달
            }
        }
    }
}
